using AutoMapper;
using Meedu.Commands.AddSchedule;
using Meedu.Commands.AddTimestamp;
using Meedu.Commands.DeleteSchedule;
using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Helpers;
using Meedu.Models;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Response;
using Meedu.Models.Schedule;
using Microsoft.EntityFrameworkCore;

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

    public async Task<ScheduleDto> AddTimestampToScheduleAsync(AddTimestampCommand command)
    {
        //var schedule = await _context.DaySchedules
        //    .Include(d => d.ScheduleTimestamps)
        //    .FirstOrDefaultAsync(d => d.Id == command.ScheduleId)
        //    ?? throw new BadRequestException(ExceptionMessages.ScheduleNotFound);

        //var availableFrom = dto.AvailableFrom;
        //var availableTo = dto.AvailableTo;

        //if (schedule.ScheduleTimestamps.Any(x => x.AvailableFrom.Hour == availableFrom.Hour))
        //    throw new BadRequestException(ExceptionMessages.TimestampNotAvailable);

        //var timespan = new ScheduleTimespan()
        //{
        //    AvailableFrom = availableFrom,
        //    AvailableTo = dto.AvailableTo,
        //};

        //schedule.ScheduleTimestamps.Add(timespan);
        //await _context.SaveChangesAsync();
        return new ScheduleDto();
    }

    public async Task DeleteTimespanFromScheduleAsync(Guid timespanId)
    {
        var timespan = await _context.ScheduleTimespans
            .Include(x => x.LessonReservations)
            .FirstOrDefaultAsync(d => d.Id == timespanId)
            ?? throw new BadRequestException(ExceptionMessages.TimestampNotFound);

        _context.LessonReservations.RemoveRange(timespan.LessonReservations);
        _context.ScheduleTimespans.Remove(timespan);
        await _context.SaveChangesAsync();
    }

    public async Task AddReservationAsync(LessonReservationDto dto, Guid timespanId)
    {
        //var userId = _userContextService.GetUserIdFromToken();

        //var timestamp = await _context.ScheduleTimespans
        //    .Include(x => x.LessonReservations)
        //    .Include(x => x.DaySchedule)
        //    .FirstOrDefaultAsync(t => t.Id == timespanId)
        //    ?? throw new BadRequestException(ExceptionMessages.TimestampNotFound);

        //if (timestamp.LessonReservations == null)
        //    timestamp.LessonReservations = new List<LessonReservation>();

        //var existingReservations = timestamp.LessonReservations
        //    .Where(x => x.ReservedById == userId && x.ReservationDate == dto.ReservationDate)
        //    .ToList();

        //if(existingReservations.Any(x => 
        //    TimeOnly.FromDateTime(x.ScheduleTimespan.AvailableFrom) == TimeOnly.FromDateTime(timestamp.AvailableFrom)))
        //    throw new BadRequestException(ExceptionMessages.YouHaveLessonReservedAtThisTime);

        //if(timestamp.LessonReservations.Any(r => r.ReservationDate == dto.ReservationDate))
        //    throw new BadRequestException(ExceptionMessages.DateIsAlreadyReserved);

        //var user = await _context.Users
        //    .FirstOrDefaultAsync(u => u.Id == dto.ReservedBy.Id)
        //    ?? throw new BadRequestException(ExceptionMessages.UserNotFound);

        //if (dto.ReservationDate.DayOfWeek.ToString() != timestamp.DaySchedule.DayOfWeek.ToString())
        //    throw new BadRequestException(ExceptionMessages.ReservationDayIsIncorrect);

        //timestamp.LessonReservations.Add(new LessonReservation()
        //{
        //    ReservedBy = user,
        //    ReservationDate = dto.ReservationDate
        //});

        //await _context.SaveChangesAsync();
    }

    public async Task DeleteReservationAsync(Guid reservationId)
    {
        var reservation = await _context.LessonReservations
            .FirstOrDefaultAsync(r => r.Id == reservationId)
            ?? throw new BadRequestException("ReservationNotFound");

        _context.LessonReservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(Guid scheduleId, Guid timespanId)
    {
        var schedule = await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .FirstOrDefaultAsync(t => t.Id == scheduleId)
            ?? throw new BadRequestException("ScheduleNotFound");

        var timespan = await _context.ScheduleTimespans
            .Include(x => x.LessonReservations)
            .ThenInclude(x => x.ReservedBy)
            .FirstOrDefaultAsync(t => t.Id == timespanId)
            ?? throw new BadRequestException("TimespanNotFound");

        var reservations = timespan.LessonReservations
            .Where(x => x.ReservationDate > DateTime.Now)
            .ToList();

        return reservations.Select(x => new LessonReservationDto
        {
            Id = x.Id.ToString(),
            ReservationDate = x.ReservationDate,
            ReservedBy = new DtoNameId
            {
                Id = x.ReservedBy.Id,
                Name = $"{x.ReservedBy.FirstName} {x.ReservedBy.LastName}"
            }
        }).ToList();
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
