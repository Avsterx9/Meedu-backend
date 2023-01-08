using Meedu.Entities;
using Meedu.Models;
using Meedu.Models.Reservations.UserReservations;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IDashboardService
    {
        Task<List<UserReservationDataDto>> GetTodaysUserLessonsAsync();
        Task<List<UserReservationDataDto>> GetUsersLessonsReservationsAsync();
    }

    public class DashboardService : IDashboardService
    {
        private readonly MeeduDbContext _dbContext;
        private readonly ILogger<ScheduleService> _logger;
        private readonly IUserContextService _userContextService;

        public DashboardService(MeeduDbContext _dbContext, ILogger<ScheduleService> _logger, IUserContextService _userContextService)
        {
            this._dbContext = _dbContext;
            this._logger = _logger;
            this._userContextService = _userContextService;
        }

        public async Task<List<UserReservationDataDto>> GetTodaysUserLessonsAsync()
        {
            var userId = _userContextService.GetUserId;

            var todaysLessons = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .ThenInclude(x => x.DaySchedule)
                .ThenInclude(x => x.PrivateLessonOffer)
                .ThenInclude(x => x.CreatedBy)
                .Where(x => x.ReservationDate == DateTime.Today && x.ReservedBy.Id == userId)
                .ToListAsync();

            var reservations = new List<UserReservationDataDto>();

            foreach(var lesson in todaysLessons)
            {
                var userInfo = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy;

                reservations.Add(new UserReservationDataDto
                {
                    AvailableFrom = lesson.ScheduleTimespan.AvailableFrom.ToString("HH:mm"),
                    AvailableTo = lesson.ScheduleTimespan.AvailableTo.ToString("HH:mm"),
                    isOnline = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.OnlineLessonsPossible,
                    LessonTitle = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.LessonTitle,
                    lessonId = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Id.ToString(),
                    Place = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Place,
                    ReservationId = lesson.Id.ToString(),
                    ScheduleId = lesson.ScheduleTimespan.DaySchedule.Id.ToString(),
                    TimespanId = lesson.ScheduleTimespan.Id.ToString(),
                    User = SetUserInfo(userInfo)
                });
            }
            return reservations;
        }

        public async Task<List<UserReservationDataDto>> GetUsersLessonsReservationsAsync()
        {
            var userId = _userContextService.GetUserId;

            var todaysLessons = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .ThenInclude(x => x.DaySchedule)
                .ThenInclude(x => x.PrivateLessonOffer)
                .ThenInclude(x => x.CreatedBy)
                .Where(x => x.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.Id == userId
                    && x.ReservationDate == DateTime.Today)
                .ToListAsync();

            var reservations = new List<UserReservationDataDto>();

            foreach (var lesson in todaysLessons)
            {
                reservations.Add(new UserReservationDataDto
                {
                    AvailableFrom = lesson.ScheduleTimespan.AvailableFrom.ToString("HH:mm"),
                    AvailableTo = lesson.ScheduleTimespan.AvailableTo.ToString("HH:mm"),
                    isOnline = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.OnlineLessonsPossible,
                    LessonTitle = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.LessonTitle,
                    lessonId = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Id.ToString(),
                    Place = lesson.ScheduleTimespan.DaySchedule.PrivateLessonOffer.Place,
                    ReservationId = lesson.Id.ToString(),
                    ScheduleId = lesson.ScheduleTimespan.DaySchedule.Id.ToString(),
                    TimespanId = lesson.ScheduleTimespan.Id.ToString(),
                    User = SetUserInfo(lesson.ReservedBy)
                });
            }
            return reservations;
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
    }
}
