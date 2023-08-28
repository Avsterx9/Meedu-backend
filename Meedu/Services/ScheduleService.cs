using AutoMapper;
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
using Meedu.Queries.GetReservationsByUser;
using Meedu.Queries.GetReservationsForUsersLessons;
using Meedu.Queries.GetScheduleByUser;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        _context.ScheduleTimespans.RemoveRange(schedule.ScheduleTimestamps);
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
        var timespan = await _context.ScheduleTimespans
            .Include(x => x.LessonReservations)
            .FirstOrDefaultAsync(d => d.Id == command.TimestampId)
            ?? throw new NotFoundException(ExceptionMessages.TimestampNotFound);

        _context.LessonReservations.RemoveRange(timespan.LessonReservations);
        _context.ScheduleTimespans.Remove(timespan);
        await _context.SaveChangesAsync();

        return new DeleteTimestampResponse(true, "Timestamps and reservations deleted successfully");
    }

    public async Task<LessonReservationDto> AddReservationAsync(AddReservationCommand command)
    {
        var userId = _userContextService.GetUserIdFromToken();

        var timestamp = await _context.ScheduleTimespans
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

    public async Task<IReadOnlyList<UserPrivateLessonReservationsDto>> GetReservationsByUserAsync(
        GetReservationsByUserQuery query)
    {
        var userId = _userContextService.GetUserId;

        var reservations = await _context.LessonReservations
            .Include(x => x.ReservedBy)
            .Include(x => x.ScheduleTimespan)
            .ThenInclude(x => x.DaySchedule)
            .Include(x => x.PrivateLessonOffer)
            .Where(x => x.ReservedById == userId
                && x.ReservationDate >= DateTime.Today && x.ReservationDate <= DateTime.Now.AddDays(query.Days))
            .AsNoTracking()
            .ToListAsync();

        return CreateDto(reservations);
    }

    public async Task<IReadOnlyList<UserPrivateLessonReservationsDto>> GetReservationsForUsersLessonsAsync(
        GetReservationsForUsersLessonsQuery query)
    {
        var userId = _userContextService.GetUserId;

        var reservations = await _context.LessonReservations
            .Include(x => x.ReservedBy)
            .Include(x => x.ScheduleTimespan)
            .ThenInclude(x => x.DaySchedule)
            .Include(x => x.PrivateLessonOffer)
            .ThenInclude(x => x.CreatedBy)
            .Where(x => x.PrivateLessonOffer.CreatedById == userId
                && x.ReservationDate >= DateTime.Today)
            .AsNoTracking()
            .ToListAsync();

        return CreateDto(reservations);
    }

    private IReadOnlyList<UserPrivateLessonReservationsDto> CreateDto(List<LessonReservation> reservations)
    {
        return reservations
            .GroupBy(g => g.ReservationDate)
            .Select(x => new UserPrivateLessonReservationsDto
            {
                ReservationDate = x.Key,
                Day = x.First().ScheduleTimespan.DaySchedule.DayOfWeek,
                DayReservations = x.Where(d => d.ReservationDate == x.Key)
                    .Select(r => new UserReservationDataDto
                    {
                        AvailableFrom = r.ScheduleTimespan.AvailableFrom,
                        AvailableTo = r.ScheduleTimespan.AvailableTo,
                        isOnline = r.PrivateLessonOffer.IsRemote,
                        LessonId = r.PrivateLessonOfferId,
                        LessonTitle = r.PrivateLessonOffer.LessonTitle,
                        Place = r.PrivateLessonOffer.Place,
                        ReservationId = r.Id,
                        ScheduleId = r.ScheduleTimespan.DayScheduleId,
                        TimespanId = r.ScheduleTimespanId,
                        User = _mapper.Map<DtoNameLastnameId>(r.ReservedBy)
                    })
                    .OrderBy(x => Int32.Parse(x.AvailableFrom.Split(":")[0]))
                    .ToList()
            })
            .OrderBy(x => x.ReservationDate)
            .ToList();
    }
}
