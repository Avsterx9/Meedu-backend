﻿using AutoMapper;
using Meedu.Commands.AddReservation;
using Meedu.Commands.AddSchedule;
using Meedu.Commands.AddTimestamp;
using Meedu.Commands.DeleteReservation;
using Meedu.Commands.DeleteSchedule;
using Meedu.Commands.DeleteTimestamp;
using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Helpers;
using Meedu.Models;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Response;
using Meedu.Models.Schedule;
using Meedu.Queries.GetReservationsByTimestamp;
using Meedu.Queries.GetScheduleByUser;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Meedu.Services;

public class ScheduleService : IScheduleService
{
    private readonly MeeduDbContext _context;
    private readonly ILogger<ScheduleService> _logger;
    private readonly IUserContextService _userContextService;
    private readonly IMapper _mapper;

    public ScheduleService(
        MeeduDbContext dbContext, 
        ILogger<ScheduleService> logger, 
        IUserContextService userContextService,
        IMapper mapper)
    {
        _context = dbContext;
        _logger = logger;
        _userContextService = userContextService;
        _mapper = mapper;
    }

    public async Task<ScheduleDto> AddScheduleAsync(AddScheduleCommand command)
    {
        var userId = _userContextService.GetUserIdFromToken();

        var schedule = await _context.DaySchedules
            .FirstOrDefaultAsync(x =>
                x.User.Id == userId 
                && x.DayOfWeek == command.DayOfWeek);

        if (schedule != null)
            throw new BadRequestException(ExceptionMessages.ScheduleAlreadyExists);

        var newSchedule = new DaySchedule
        {
            Created = DateTime.Now,
            DayOfWeek = command.DayOfWeek,
            UserId = userId
        };

        await _context.DaySchedules.AddAsync(newSchedule);
        await _context.SaveChangesAsync();

        return _mapper.Map<ScheduleDto>(newSchedule);   
    }

    public async Task<DeleteScheduleResponse> DeleteScheduleAsync(DeleteScheduleCommand command)
    {
        var schedule = await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .ThenInclude(x => x.LessonReservations)
            .FirstOrDefaultAsync(x => x.Id == command.ScheduleId)
            ?? throw new NotFoundException(ExceptionMessages.ScheduleNotFound);

        foreach(var timestamp in schedule.ScheduleTimestamps)
        {
            if(timestamp.LessonReservations != null && timestamp.LessonReservations.Count > 0)
                _context.LessonReservations.RemoveRange(timestamp.LessonReservations);
        }

        _context.ScheduleTimestamps.RemoveRange(schedule.ScheduleTimestamps);
        _context.DaySchedules.Remove(schedule);

        await _context.SaveChangesAsync();
        return new DeleteScheduleResponse(true, "Schedule deleted successfully");
    }

    public async Task<IReadOnlyList<ScheduleDto>> GetScheduleByUserAsync(GetScheduleByUserQuery query)
    {
        return await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .ThenInclude(x => x.LessonReservations)
            .Where(x => x.UserId == query.userId)
            .Select(x => _mapper.Map<ScheduleDto>(x))
            .ToListAsync(); 
    }

    public async Task<ScheduleDto> AddTimestampToScheduleAsync(AddTimestampCommand command)
    {
        var schedule = await _context.DaySchedules
            .Include(d => d.ScheduleTimestamps)
            .FirstOrDefaultAsync(d => d.Id == command.ScheduleId)
            ?? throw new BadRequestException(ExceptionMessages.ScheduleNotFound);

        if (!ValidateTimestampHour(command.AvailableFrom) && !ValidateTimestampHour(command.AvailableTo))
            throw new BadRequestException(ExceptionMessages.InvalidTimestamp);

        if(schedule.ScheduleTimestamps.Any(x => !AreHouresOverlapping(x.AvailableFrom, command.AvailableFrom)))
            throw new BadRequestException(ExceptionMessages.TimestampNotAvailable);

        schedule.ScheduleTimestamps.Add(new ScheduleTimestamp()
        {
            AvailableFrom = command.AvailableFrom,
            AvailableTo = command.AvailableTo,
        });

        await _context.SaveChangesAsync();
        return _mapper.Map<ScheduleDto>(schedule);
    }

    public bool AreHouresOverlapping(string first, string second)
    {
        var firstHours = first.Split(":");
        var secondHours = second.Split(":");

        var firstHour = Int32.Parse(firstHours[0]);
        var secondHour = Int32.Parse(secondHours[0]);

        var firstMinutes = Int32.Parse(firstHours[1]);
        var secondMinutes = Int32.Parse(secondHours[1]);

        if (secondHour > firstHour || secondHour < firstHour)
            return true;

        if(firstHour == secondHour)
            return firstMinutes < secondMinutes ? true : false;

        return false;
    }

    private bool ValidateTimestampHour(string timestamp)
    {
        var regex = new Regex("^(0?[1-9]|1[0-2]):[0-5][0-9]$");

        return regex.Match(timestamp).Success;
    }

    public async Task<DeleteTimestampResponse> DeleteTimespanFromScheduleAsync(DeleteTimestampCommand command)
    {
        var timespan = await _context.ScheduleTimestamps
            .Include(x => x.LessonReservations)
            .FirstOrDefaultAsync(d => d.Id == command.TimestampId)
            ?? throw new NotFoundException(ExceptionMessages.TimestampNotFound);

        _context.LessonReservations.RemoveRange(timespan.LessonReservations);
        _context.ScheduleTimestamps.Remove(timespan);
        await _context.SaveChangesAsync();

        return new DeleteTimestampResponse(true, "Timestamps and reservations deleted successfully");
    }

    public async Task<LessonReservationDto> AddReservationAsync(AddReservationCommand command)
    {
        var userId = _userContextService.GetUserIdFromToken();

        var timestamp = await _context.ScheduleTimestamps
            .Include(x => x.LessonReservations)
            .Include(x => x.DaySchedule)
            .FirstOrDefaultAsync(t => t.Id == command.TimestampId)
            ?? throw new NotFoundException(ExceptionMessages.TimestampNotFound);

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new BadRequestException(ExceptionMessages.UserNotFound);

        var existingReservation = timestamp.LessonReservations
            .FirstOrDefault(x => x.ReservationDate == command.ReservationDate);

        if (existingReservation is not null)
            throw new BadRequestException(ExceptionMessages.DateIsAlreadyReserved);

        var userReservations = await _context.LessonReservations
            .Where(x => x.ReservedById == userId && x.ReservationDate == command.ReservationDate)
            .ToListAsync();

        if (userReservations.Any())
            throw new BadRequestException(ExceptionMessages.YouHaveLessonReservedAtThisTime);

        var newReservation = new LessonReservation
        {
            PrivateLessonOfferId = command.LessonOfferId,
            ReservedById = userId,
            ScheduleTimespanId = timestamp.Id,
            ReservationDate = command.ReservationDate,
        };

        await _context.LessonReservations.AddAsync(newReservation);
        await _context.SaveChangesAsync();

        return new LessonReservationDto
        {
            ReservationDate = command.ReservationDate,
            ReservedBy = new DtoNameId { Id = userId, Name = $"{user.FirstName} {user.LastName}" }
        };
    }

    public async Task<DeleteReservationResponse> DeleteReservationAsync(DeleteReservationCommand command)
    {
        var reservation = await _context.LessonReservations
            .FirstOrDefaultAsync(r => r.Id == command.ReservationId)
            ?? throw new BadRequestException(ExceptionMessages.ReservationNotFound);

        _context.LessonReservations.Remove(reservation);
        await _context.SaveChangesAsync();

        return new DeleteReservationResponse(true, "Reservation deleted successfully");
    }

    public async Task<IReadOnlyList<LessonReservationDto>> GetReservationsByTimestampIdAsync(
        GetReservationsByTimestampQuery query)
    {
        return await _context.LessonReservations
            .Include(x => x.ReservedBy)
            .Where(x => x.ScheduleTimespanId == query.TimestampId
                && x.ReservationDate > DateTime.Now)
            .AsNoTracking()
            .Select(x => _mapper.Map<LessonReservationDto>(x))
            .ToListAsync();
    }

    public async Task<List<UserPrivateLessonReservationsDto>> GetReservationsByUserAsync(int days)
    {
        var userId = _userContextService.GetUserId;

        var reservations = await _context.LessonReservations
            .Include(x => x.ReservedBy)
            .Include(x => x.ScheduleTimespan)
            .ThenInclude(x => x.DaySchedule)
            //.ThenInclude(x => x.PrivateLessonOffer)
            //.ThenInclude(x => x.CreatedBy)
            //.Where(x => x.ReservedBy.Id == userId && x.ReservationDate >= DateTime.Today && x.ReservationDate <= DateTime.Now.AddDays(days))
            .ToListAsync();

        var dates = reservations
            .Select(x => x.ReservationDate)
            .Distinct()
            .ToList();

        var userLessonReservationsList = new List<UserPrivateLessonReservationsDto>();

        foreach (var date in dates)
        {
            var reservation = new UserPrivateLessonReservationsDto
            {
                ReservationDate = date,
                DayReservations = new List<UserReservationDataDto>(),
                Day = (int)date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1
            };

            //reservation.DayReservations = reservations
            //    .Where(x => x.ReservationDate == date)
            //    .Select(x => CreateUserReservationDto(x, x.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy))
            //    .ToList();

            userLessonReservationsList.Add(reservation);
        }

        foreach (var x in userLessonReservationsList)
        {
            x.DayReservations = x.DayReservations
                .OrderBy(y => Int32.Parse(y.AvailableFrom.Split(":")[0]))
                .ToList();
        }

        return userLessonReservationsList.OrderBy(x => x.ReservationDate).ToList();
    }

    public async Task<List<UserPrivateLessonReservationsDto>> GetUserLessonReservationsAsync(int days)
    {
        var userId = _userContextService.GetUserId;

        var reservations = await _context.LessonReservations
            .Include(x => x.ReservedBy)
            .Include(x => x.ScheduleTimespan)
            .ThenInclude(x => x.DaySchedule)
            //.ThenInclude(x => x.PrivateLessonOffer)
            //.ThenInclude(x => x.CreatedBy)
            //.Where(x => x.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.Id == userId && x.ReservationDate >= DateTime.Today)
            .ToListAsync();

        var dates = reservations
            .Select(x => x.ReservationDate)
            .Distinct()
            .ToList();

        var userLessonReservationsList = new List<UserPrivateLessonReservationsDto>();

        foreach (var date in dates)
        {
            var reservation = new UserPrivateLessonReservationsDto
            {
                ReservationDate = date,
                DayReservations = new List<UserReservationDataDto>(),
                Day = (int)date.DayOfWeek == 0 ? 6 : (int)date.DayOfWeek - 1
            };
            reservation.DayReservations = reservations
                .Where(x => x.ReservationDate == date)
                .Select(x => CreateUserReservationDto(x, x.ReservedBy))
                .ToList();

            userLessonReservationsList.Add(reservation);
        }

        foreach (var x in userLessonReservationsList)
        {
            x.DayReservations = x.DayReservations
                .OrderBy(y => Int32.Parse(y.AvailableFrom.Split(":")[0]))
                .ToList();
        }
        return userLessonReservationsList.OrderBy(x => x.ReservationDate).ToList();
    }

    private UserReservationDataDto CreateUserReservationDto(LessonReservation r, User userInfo)
    {
        return new UserReservationDataDto();
        //return new UserReservationDataDto
        //{
        //    AvailableFrom = r.ScheduleTimespan.AvailableFrom.ToString("HH:mm"),
        //    AvailableTo = r.ScheduleTimespan.AvailableTo.ToString("HH:mm"),
        //    isOnline = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.OnlineLessonsPossible,
        //    LessonTitle = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.LessonTitle,
        //    Place = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Place,
        //    ReservationId = r.Id.ToString(),
        //    ScheduleId = r.ScheduleTimespan.DaySchedule.Id.ToString(),
        //    TimespanId = r.ScheduleTimespan.Id.ToString(),
        //    lessonId = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Id.ToString(),
        //    User = SetUserInfo(userInfo)
        //};
    }

    private DtoNameLastnameId SetUserInfo(User userInfo)
    {
        return new DtoNameLastnameId
        {
            Id = userInfo.Id.ToString(),
            FirstName = userInfo.FirstName,
            LastName = userInfo.LastName,
            PhoneNumber = userInfo.PhoneNumber
        };
    }

    private async Task<Subject> CreateNewSubjectIfNotExists(String SubjectName)
    {
        var subject = await _context.Subjects
            .FirstOrDefaultAsync(s => s.Name.Contains(SubjectName));

        if (subject == null)
        {
            subject = new Subject() { Name = SubjectName };
            _context.Subjects.Add(subject);
        }

        return subject;
    }

    private ScheduleDto CreateDtoFromEntity(DaySchedule entity)
    {
        //var scheduleDto = new ScheduleDto()
        //{
        //    Id = entity.Id,
        //    DayOfWeek = entity.DayOfWeek,
        //    //Subject = new SubjectDto(),
        //    ScheduleTimestamps = new List<ScheduleTimespanDto>()
        //};

        //if(entity.ScheduleTimestamps != null && entity.ScheduleTimestamps.Count != 0)
        //{
        //    foreach(var entityTimestamp in entity.ScheduleTimestamps)
        //    {
        //        var timespanDto = new ScheduleTimespanDto()
        //        {
        //            Id = entityTimestamp.Id,
        //            AvailableFrom = entityTimestamp.AvailableFrom.ToString("HH:mm"),
        //            AvailableTo = entityTimestamp.AvailableTo,
        //            LessonReservations = new List<LessonReservationDto>()
        //        };

        //        if(entityTimestamp.LessonReservations != null && entityTimestamp.LessonReservations.Count != 0)
        //        {
        //            foreach(var entityReservation in entityTimestamp.LessonReservations)
        //            {
        //                var reservationDto = new LessonReservationDto()
        //                {
        //                    Id = entityReservation.Id.ToString(),
        //                    ReservationDate = entityReservation.ReservationDate,
        //                    ReservedBy = new DtoNameId()
        //                    {
        //                        Id = entityReservation.ReservedBy.Id,
        //                        Name = entityReservation.ReservedBy.LastName
        //                    }
        //                };
        //                timespanDto.LessonReservations.Add(reservationDto);
        //            }
        //        }
        //        scheduleDto.ScheduleTimestamps.Add(timespanDto);
        //    }
        //}
        //return scheduleDto;
        return new ScheduleDto();
    }

    private Guid ValidateGuid(string guidToValidate)
    {
        if (!Guid.TryParse(guidToValidate, out var guid))
            throw new BadRequestException("InvalidGuid");

        return guid;
    }
}
