using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IScheduleService
    {
        Task AddScheduleAsync(ScheduleDto dto);
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
            var userID = _userContextService.GetUserId;

            var schedule = await _dbContext.DaySchedules
                .FirstOrDefaultAsync(ds => ds.User.Id == userID && ds.Subject.Name == dto.Subject.Name);

            if (schedule != null)
                throw new BadRequestException("ScheduleAlreadyExists");

            Subject subject = await CreateNewSubjectIfNotExists(dto.Subject.Name);
            var timestamps = new List<ScheduleTimespan>();

            if (dto.ScheduleTimespans != null)
            {
                foreach (var timespan in dto.ScheduleTimespans)
                {
                    timestamps.Add(new ScheduleTimespan()
                    {
                        AvailableFrom = DateTime.Parse(timespan.AvailableFrom),
                        AvailableTo = DateTime.Parse(timespan.AvailableTo),
                        LessonReservations = new List<LessonReservation>()
                    });
                }
            }

            schedule = new DaySchedule()
            {
                DayOfWeek = dto.DayOfWeek,
                Subject = subject,
                Created = DateTime.Now,
                User = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userID),
                ScheduleTimestamps = timestamps
            };

            await _dbContext.AddAsync(schedule);
            await _dbContext.SaveChangesAsync();
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
    }
}
