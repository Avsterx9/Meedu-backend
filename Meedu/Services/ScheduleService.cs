using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Models;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IScheduleService
    {
        Task AddScheduleAsync(ScheduleDto dto);
        Task DeleteScheduleAsync(string scheduleId);
        Task<List<ScheduleDto>> GetScheduleByLessonOfferId(string lessonId);
        Task AddTimespanToScheduleAsync(ScheduleTimespanDto dto, string scheduleId);
        Task DeleteTimespanFromScheduleAsync(string timespanId);
        Task AddReservationAsync(LessonReservationDto dto, string scheduleId, string timespanId);
        Task DeleteReservationAsync(string reservationId);
        Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(string scheduleId, string timespanId);
        Task<List<UserPrivateLessonReservationsDto>> GetReservationsByUserAsync(int days);
        Task<List<UserPrivateLessonReservationsDto>> GetUserLessonReservationsAsync(int days);
    }

    public class ScheduleService : IScheduleService
    {
        private readonly MeeduDbContext _dbContext;
        private readonly ILogger<ScheduleService> _logger;
        private readonly IUserContextService _userContextService;

        public ScheduleService(MeeduDbContext _dbContext, ILogger<ScheduleService> _logger, IUserContextService _userContextService)
        {
            this._dbContext = _dbContext;
            this._logger = _logger;
            this._userContextService = _userContextService;
        }

        public async Task AddScheduleAsync(ScheduleDto dto)
        {
            var userId = _userContextService.GetUserId;

            var schedule = await _dbContext.DaySchedules
                .Include(x => x.ScheduleTimestamps)
                .FirstOrDefaultAsync(ds => ds.User.Id == userId && ds.Subject.Name == dto.Subject.Name);

            var lessonOffer = await _dbContext.PrivateLessonOffers
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
                    Subject = subject,
                    Created = DateTime.Now,
                    User = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId),
                    ScheduleTimestamps = timespans,
                    PrivateLessonOffer = lessonOffer
                };
                await _dbContext.AddAsync(schedule);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteScheduleAsync(string scheduleId)
        {
            var scheduleGuid = ValidateGuid(scheduleId);
            var schedule = await _dbContext.DaySchedules
                .Include(x => x.ScheduleTimestamps)
                .ThenInclude(x => x.LessonReservations)
                .FirstOrDefaultAsync(x => x.Id == scheduleGuid)
                ?? throw new NotFoundException("ScheduleNotFound");

            foreach(var timestamp in schedule.ScheduleTimestamps)
            {
                if(timestamp.LessonReservations != null && timestamp.LessonReservations.Count > 0)
                    _dbContext.LessonReservations.RemoveRange(timestamp.LessonReservations);
            }

            _dbContext.ScheduleTimespans.RemoveRange(schedule.ScheduleTimestamps);
            _dbContext.DaySchedules.Remove(schedule);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<ScheduleDto>> GetScheduleByLessonOfferId(string lessonId)
        {
            var lessonGuid = ValidateGuid(lessonId);

            var schedules = await _dbContext.DaySchedules
                .Include(u => u.User)
                .Include(u => u.Subject)
                .Include(u => u.ScheduleTimestamps)
                .Where(ds => ds.PrivateLessonOffer.Id == lessonGuid)
                .ToListAsync()
                ?? throw new NotFoundException("ScheduleNotFound");

            return schedules.Select(x => CreateDtoFromEntity(x)).OrderBy(x => x.DayOfWeek).ToList();
        }

        public async Task AddTimespanToScheduleAsync(ScheduleTimespanDto dto, string scheduleId)
        {
            var scheduleGuid = ValidateGuid(scheduleId);

            var schedule = await _dbContext.DaySchedules
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
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteTimespanFromScheduleAsync(string timespanId)
        {
            var timespanGuid = ValidateGuid(timespanId);

            var timespan = await _dbContext.ScheduleTimespans
                .Include(x => x.LessonReservations)
                .FirstOrDefaultAsync(d => d.Id == timespanGuid)
                ?? throw new BadRequestException("TimespanNotFound");

            _dbContext.LessonReservations.RemoveRange(timespan.LessonReservations);
            _dbContext.ScheduleTimespans.Remove(timespan);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddReservationAsync(LessonReservationDto dto, string scheduleId, string timespanId)
        {
            var userId = _userContextService.GetUserId;

            var timespanGuid = ValidateGuid(timespanId);
            var scheduleGuid = ValidateGuid(scheduleId);

            var schedule = await _dbContext.DaySchedules
                .Include(x => x.ScheduleTimestamps)
                .FirstOrDefaultAsync(t => t.Id == scheduleGuid)
                ?? throw new BadRequestException("ScheduleNotFound");

            var timespan = await _dbContext.ScheduleTimespans
                .Include(x => x.LessonReservations)
                .FirstOrDefaultAsync(t => t.Id == timespanGuid)
                ?? throw new BadRequestException("TimespanNotFound");

            if (timespan.LessonReservations == null)
                timespan.LessonReservations = new List<LessonReservation>();

            var existingReservations = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .Where(x => x.ReservedBy.Id == userId && x.ReservationDate == dto.ReservationDate)
                .ToListAsync();

            if(existingReservations.Any(x => TimeOnly.FromDateTime(x.ScheduleTimespan.AvailableFrom) == TimeOnly.FromDateTime(timespan.AvailableFrom)))
                throw new BadRequestException("YouHaveLessonReservedAtThisTime");

            if(timespan.LessonReservations.Any(r => r.ReservationDate == dto.ReservationDate))
                throw new BadRequestException("DateIsAlreadyReserved");

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == new Guid(dto.ReservedBy.Id))
                ?? throw new BadRequestException("UserNotFound");

            if (dto.ReservationDate.DayOfWeek.ToString() != schedule.DayOfWeek.ToString())
                throw new BadRequestException("ReservationDayIsIncorrect");

            timespan.LessonReservations.Add(new LessonReservation()
            {
                ReservedBy = user,
                ReservationDate = dto.ReservationDate
            });

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteReservationAsync(string reservationId)
        {
            var reservationGuid = ValidateGuid(reservationId);

            var reservation = await _dbContext.LessonReservations
                .FirstOrDefaultAsync(r => r.Id == reservationGuid)
                ?? throw new BadRequestException("ReservationNotFound");

            _dbContext.LessonReservations.Remove(reservation);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(string scheduleId, string timespanId)
        {
            var scheduleGuid = ValidateGuid(scheduleId);
            var timespanGuid = ValidateGuid(timespanId);

            var schedule = await _dbContext.DaySchedules
                .Include(x => x.ScheduleTimestamps)
                .FirstOrDefaultAsync(t => t.Id == scheduleGuid)
                ?? throw new BadRequestException("ScheduleNotFound");

            var timespan = await _dbContext.ScheduleTimespans
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

            var reservations = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .ThenInclude(x => x.DaySchedule)
                .ThenInclude(x => x.PrivateLessonOffer)
                .ThenInclude(x => x.CreatedBy)
                .Where(x => x.ReservedBy.Id == userId && x.ReservationDate >= DateTime.Now && x.ReservationDate <= DateTime.Now.AddDays(days))
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
                    .Select(x => CreateUserReservationDto(x, x.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy))
                    .ToList();

                userLessonReservationsList.Add(reservation);
            }
            userLessonReservationsList
                .ForEach(x => x.DayReservations
                .Sort((x, y) => Int32.Parse(x.AvailableFrom.Split(":")[0])));
            return userLessonReservationsList.OrderBy(x => x.ReservationDate).ToList();
        }

        public async Task<List<UserPrivateLessonReservationsDto>> GetUserLessonReservationsAsync(int days)
        {
            var userId = _userContextService.GetUserId;

            var reservations = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .ThenInclude(x => x.DaySchedule)
                .ThenInclude(x => x.PrivateLessonOffer)
                .ThenInclude(x => x.CreatedBy)
                .Where(x => x.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.Id == userId && x.ReservationDate >= DateTime.Today)
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
            userLessonReservationsList
                .ForEach(x => x.DayReservations
                .Sort((x, y) => Int32.Parse(x.AvailableFrom.Split(":")[0])));
            return userLessonReservationsList.OrderBy(x => x.ReservationDate).ToList();
        }

        private UserReservationDataDto CreateUserReservationDto(LessonReservation r, User userInfo)
        {
            return new UserReservationDataDto
            {
                AvailableFrom = r.ScheduleTimespan.AvailableFrom.ToString("HH:mm"),
                AvailableTo = r.ScheduleTimespan.AvailableTo.ToString("HH:mm"),
                isOnline = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.OnlineLessonsPossible,
                LessonTitle = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.LessonTitle,
                Place = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Place,
                ReservationId = r.Id.ToString(),
                ScheduleId = r.ScheduleTimespan.DaySchedule.Id.ToString(),
                TimespanId = r.ScheduleTimespan.Id.ToString(),
                lessonId = r.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Id.ToString(),
                User = SetUserInfo(userInfo)
            };
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
            var subject = await _dbContext.Subjects
                .FirstOrDefaultAsync(s => s.Name.Contains(SubjectName));

            if (subject == null)
            {
                subject = new Subject() { Name = SubjectName };
                _dbContext.Subjects.Add(subject);
            }

            return subject;
        }

        private ScheduleDto CreateDtoFromEntity(DaySchedule entity)
        {
            var scheduleDto = new ScheduleDto()
            {
                Id = entity.Id.ToString(),
                DayOfWeek = entity.DayOfWeek,
                Subject = new SubjectDto()
                {
                    Id = entity.Subject.Id.ToString(),
                    Name = entity.Subject.Name
                },
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
}
