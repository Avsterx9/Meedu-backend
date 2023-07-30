using AutoMapper;
using Meedu.Entities;
using Meedu.Models;
using Meedu.Models.Reservations.UserReservations;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly MeeduDbContext _dbContext;
        private readonly ILogger<ScheduleService> _logger;
        private readonly IUserContextService _userContextService;
        private readonly IMapper _mapper;

        public DashboardService(
            MeeduDbContext dbContext, 
            ILogger<ScheduleService> logger, 
            IUserContextService userContextService,
            IMapper mapper)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userContextService = userContextService;
            _mapper = mapper;
        }

        public async Task<List<UserReservationDataDto>> GetTodaysUserLessonsAsync()
        {
            var userId = _userContextService.GetUserIdFromToken();

            var todaysLessons = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .ThenInclude(x => x.DaySchedule)
                .ThenInclude(x => x.PrivateLessonOffer)
                .ThenInclude(x => x.CreatedBy)
                .Where(x => x.ReservationDate == DateTime.Today && x.ReservedBy.Id == userId)
                .ToListAsync();

            return todaysLessons
                .Select(x => _mapper.Map<UserReservationDataDto>(x))
                .OrderBy(x => Int32.Parse(x.AvailableFrom.Split(":")[0]))
                .ToList();

            //var reservations = todaysLessons
            //    .Select(x => _mapper.Map<UserReservationDataDto>(x))
            //    .ToList();

            //reservations
            //    .Sort((x, y) => Int32.Parse(x.AvailableFrom.Split(":")[0]));
            //return reservations;
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

            var reservations = todaysLessons.Select(x => CreateUserReservationDto(x, x.ReservedBy)).ToList();

            reservations
                .Sort((x, y) => Int32.Parse(x.AvailableFrom.Split(":")[0]));
            return reservations;
        }

        public async Task<List<DtoNameLastnameId>> GetUserStudentsAsync(int amount)
        {
            var userId = _userContextService.GetUserId;

            var reservations = await _dbContext.LessonReservations
                .Include(x => x.ReservedBy)
                .Include(x => x.ScheduleTimespan)
                .ThenInclude(x => x.DaySchedule)
                .ThenInclude(x => x.PrivateLessonOffer)
                .ThenInclude(x => x.CreatedBy)
                .Where(x => x.ScheduleTimespan.DaySchedule.PrivateLessonOffer.CreatedBy.Id == userId)
                .ToListAsync();

            var userList = reservations.Select(x => x.ReservedBy)
                .Take(amount)
                .Distinct();

            return userList.Select(x => new DtoNameLastnameId
            {
                FirstName = x.FirstName,
                Id = x.Id.ToString(),
                LastName = x.LastName,
                PhoneNumber = x.PhoneNumber
            }).ToList();
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
    }
}
