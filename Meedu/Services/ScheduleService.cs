using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Models;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services;

public class ScheduleService : IScheduleService
{
    private readonly MeeduDbContext _context;
    private readonly ILogger<ScheduleService> _logger;
    private readonly IUserContextService _userContextService;

    public ScheduleService(MeeduDbContext dbContext, ILogger<ScheduleService> logger, IUserContextService userContextService)
    {
        _context = dbContext;
        _logger = logger;
        _userContextService = userContextService;
    }

    public async Task AddScheduleAsync(ScheduleDto dto)
    {
        var userId = _userContextService.GetUserId;

        var schedule = await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .FirstOrDefaultAsync(ds =>
                ds.User.Id == userId &&
                ds.DayOfWeek == dto.DayOfWeek);

        var lessonOffer = await _context.PrivateLessonOffers
            .FirstOrDefaultAsync(x => x.Id == new Guid(dto.lessonOfferId))
            ?? throw new BadRequestException("LessonOfferNotExists");

        Subject subject = await CreateNewSubjectIfNotExists(dto.Subject.Name);
        var timespans = new List<ScheduleTimespan>();

        if (dto.ScheduleTimespans != null)
        {
            foreach (var timespan in dto.ScheduleTimespans)
            {
                timespans.Add(new ScheduleTimespan()
                {
                    AvailableFrom = DateTime.Parse(timespan.AvailableFrom),
                    AvailableTo = DateTime.Parse(timespan.AvailableTo),
                    LessonReservations = new List<LessonReservation>()
                });
            }
        }

        if (schedule != null)
        {
            foreach(var t in timespans)
            {
                if (!schedule.ScheduleTimestamps.Any(x => TimeOnly.FromDateTime(x.AvailableFrom) == TimeOnly.FromDateTime(t.AvailableFrom)))
                    schedule.ScheduleTimestamps.Add(t);
            }
        } 
        else
        {
            schedule = new DaySchedule()
            {
                DayOfWeek = dto.DayOfWeek,
                Created = DateTime.Now,
                User = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId),
                ScheduleTimestamps = timespans
            };
            await _context.AddAsync(schedule);
        }
        await _context.SaveChangesAsync();
    }

    public async Task DeleteScheduleAsync(string scheduleId)
    {
        var scheduleGuid = ValidateGuid(scheduleId);
        var schedule = await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .ThenInclude(x => x.LessonReservations)
            .FirstOrDefaultAsync(x => x.Id == scheduleGuid)
            ?? throw new NotFoundException("ScheduleNotFound");

        foreach(var timestamp in schedule.ScheduleTimestamps)
        {
            if(timestamp.LessonReservations != null && timestamp.LessonReservations.Count > 0)
                _context.LessonReservations.RemoveRange(timestamp.LessonReservations);
        }

        _context.ScheduleTimespans.RemoveRange(schedule.ScheduleTimestamps);
        _context.DaySchedules.Remove(schedule);

        await _context.SaveChangesAsync();
    }

    public async Task<List<ScheduleDto>> GetScheduleByLessonOfferId(string lessonId)
    {
        //var lessonGuid = ValidateGuid(lessonId);

        //var schedules = await _dbContext.DaySchedules
        //    .Include(u => u.User)
        //    .Include(u => u.ScheduleTimestamps)
        //    .Where(ds => ds.PrivateLessonOffer.Id == lessonGuid)
        //    .ToListAsync()
        //    ?? throw new NotFoundException("ScheduleNotFound");

        //return schedules.Select(x => CreateDtoFromEntity(x)).OrderBy(x => x.DayOfWeek).ToList();
        return new List<ScheduleDto?>();
    }

    public async Task AddTimespanToScheduleAsync(ScheduleTimespanDto dto, string scheduleId)
    {
        var scheduleGuid = ValidateGuid(scheduleId);

        var schedule = await _context.DaySchedules
            .Include(d => d.ScheduleTimestamps)
            .FirstOrDefaultAsync(d => d.Id == scheduleGuid)
            ?? throw new BadRequestException("ScheduleNotFound");

        var availableFrom = DateTime.Parse(dto.AvailableFrom);
        var availableTo = DateTime.Parse(dto.AvailableTo);

        if (schedule.ScheduleTimestamps.Any(x => x.AvailableFrom.Hour == availableFrom.Hour))
            throw new BadRequestException("TimestampNotAvailable");

        var timespan = new ScheduleTimespan()
        {
            AvailableFrom = availableFrom,
            AvailableTo = DateTime.Parse(dto.AvailableTo),
        };

        schedule.ScheduleTimestamps.Add(timespan);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteTimespanFromScheduleAsync(string timespanId)
    {
        var timespanGuid = ValidateGuid(timespanId);

        var timespan = await _context.ScheduleTimespans
            .Include(x => x.LessonReservations)
            .FirstOrDefaultAsync(d => d.Id == timespanGuid)
            ?? throw new BadRequestException("TimespanNotFound");

        _context.LessonReservations.RemoveRange(timespan.LessonReservations);
        _context.ScheduleTimespans.Remove(timespan);
        await _context.SaveChangesAsync();
    }

    public async Task AddReservationAsync(LessonReservationDto dto, string scheduleId, string timespanId)
    {
        var userId = _userContextService.GetUserId;

        var timespanGuid = ValidateGuid(timespanId);
        var scheduleGuid = ValidateGuid(scheduleId);

        var schedule = await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .FirstOrDefaultAsync(t => t.Id == scheduleGuid)
            ?? throw new BadRequestException("ScheduleNotFound");

        var timespan = await _context.ScheduleTimespans
            .Include(x => x.LessonReservations)
            .FirstOrDefaultAsync(t => t.Id == timespanGuid)
            ?? throw new BadRequestException("TimespanNotFound");

        if (timespan.LessonReservations == null)
            timespan.LessonReservations = new List<LessonReservation>();

        var existingReservations = await _context.LessonReservations
            .Include(x => x.ReservedBy)
            .Include(x => x.ScheduleTimespan)
            .Where(x => x.ReservedBy.Id == userId && x.ReservationDate == dto.ReservationDate)
            .ToListAsync();

        if(existingReservations.Any(x => TimeOnly.FromDateTime(x.ScheduleTimespan.AvailableFrom) == TimeOnly.FromDateTime(timespan.AvailableFrom)))
            throw new BadRequestException("YouHaveLessonReservedAtThisTime");

        if(timespan.LessonReservations.Any(r => r.ReservationDate == dto.ReservationDate))
            throw new BadRequestException("DateIsAlreadyReserved");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == new Guid(dto.ReservedBy.Id))
            ?? throw new BadRequestException("UserNotFound");

        if (dto.ReservationDate.DayOfWeek.ToString() != schedule.DayOfWeek.ToString())
            throw new BadRequestException("ReservationDayIsIncorrect");

        timespan.LessonReservations.Add(new LessonReservation()
        {
            ReservedBy = user,
            ReservationDate = dto.ReservationDate
        });

        await _context.SaveChangesAsync();
    }

    public async Task DeleteReservationAsync(string reservationId)
    {
        var reservationGuid = ValidateGuid(reservationId);

        var reservation = await _context.LessonReservations
            .FirstOrDefaultAsync(r => r.Id == reservationGuid)
            ?? throw new BadRequestException("ReservationNotFound");

        _context.LessonReservations.Remove(reservation);
        await _context.SaveChangesAsync();
    }

    public async Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(string scheduleId, string timespanId)
    {
        var scheduleGuid = ValidateGuid(scheduleId);
        var timespanGuid = ValidateGuid(timespanId);

        var schedule = await _context.DaySchedules
            .Include(x => x.ScheduleTimestamps)
            .FirstOrDefaultAsync(t => t.Id == scheduleGuid)
            ?? throw new BadRequestException("ScheduleNotFound");

        var timespan = await _context.ScheduleTimespans
            .Include(x => x.LessonReservations)
            .ThenInclude(x => x.ReservedBy)
            .FirstOrDefaultAsync(t => t.Id == timespanGuid)
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
                Id = x.ReservedBy.Id.ToString(),
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
        var scheduleDto = new ScheduleDto()
        {
            Id = entity.Id.ToString(),
            DayOfWeek = entity.DayOfWeek,
            Subject = new SubjectDto(),
            ScheduleTimespans = new List<ScheduleTimespanDto>()
        };

        if(entity.ScheduleTimestamps != null && entity.ScheduleTimestamps.Count != 0)
        {
            foreach(var entityTimestamp in entity.ScheduleTimestamps)
            {
                var timespanDto = new ScheduleTimespanDto()
                {
                    Id = entityTimestamp.Id.ToString(),
                    AvailableFrom = entityTimestamp.AvailableFrom.ToString("HH:mm"),
                    AvailableTo = entityTimestamp.AvailableTo.ToString("HH:mm"),
                    LessonReservations = new List<LessonReservationDto>()
                };

                if(entityTimestamp.LessonReservations != null && entityTimestamp.LessonReservations.Count != 0)
                {
                    foreach(var entityReservation in entityTimestamp.LessonReservations)
                    {
                        var reservationDto = new LessonReservationDto()
                        {
                            Id = entityReservation.Id.ToString(),
                            ReservationDate = entityReservation.ReservationDate,
                            ReservedBy = new DtoNameId()
                            {
                                Id = entityReservation.ReservedBy.Id.ToString(),
                                Name = entityReservation.ReservedBy.LastName
                            }
                        };
                        timespanDto.LessonReservations.Add(reservationDto);
                    }
                }
                scheduleDto.ScheduleTimespans.Add(timespanDto);
            }
        }
        return scheduleDto;
    }

    private Guid ValidateGuid(string guidToValidate)
    {
        if (!Guid.TryParse(guidToValidate, out var guid))
            throw new BadRequestException("InvalidGuid");

        return guid;
    }
}
