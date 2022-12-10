﻿using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IScheduleService
    {
        Task AddScheduleAsync(ScheduleDto dto);
        Task<ScheduleDto> GetScheduleByLessonOfferId(string lessonId);
        Task AddTimespanToScheduleAsync(ScheduleTimespanDto dto, string scheduleId);
        Task DeleteTimespanFromScheduleAsync(string timespanId);
        Task AddReservationAsync(LessonReservationDto dto, string timespanId);
        Task DeleteReservationAsync(string reservationId);
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
                .FirstOrDefaultAsync(ds => ds.User.Id == userId && ds.Subject.Name == dto.Subject.Name);

            if (schedule != null)
                throw new BadRequestException("ScheduleAlreadyExists");

            var lessonOffer = await _dbContext.PrivateLessonOffers
                .FirstOrDefaultAsync(x => x.Id == new Guid(dto.lessonOfferId))
                ?? throw new BadRequestException("LessonOfferNotExists");

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
                User = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId),
                ScheduleTimestamps = timestamps,
                PrivateLessonOffer = lessonOffer
            };

            await _dbContext.AddAsync(schedule);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<ScheduleDto> GetScheduleByLessonOfferId(string lessonId)
        {
            var lessonGuid = ValidateGuid(lessonId);

            var schedule = await _dbContext.DaySchedules
                .Include(u => u.User)
                .Include(u => u.Subject)
                .Include(u => u.ScheduleTimestamps)
                .FirstOrDefaultAsync(ds => ds.PrivateLessonOffer.Id == lessonGuid)
                ?? throw new NotFoundException("ScheduleNotFound");

            return CreateDtoFromEntity(schedule);
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
                .FirstOrDefaultAsync(d => d.Id == timespanGuid)
                ?? throw new BadRequestException("TimespanNotFound");

            _dbContext.ScheduleTimespans.Remove(timespan);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddReservationAsync(LessonReservationDto dto, string timespanId)
        {
            var timespanGuid = ValidateGuid(timespanId);

            var timespan = _dbContext.ScheduleTimespans
                .FirstOrDefault(t => t.Id == timespanGuid)
                ?? throw new BadRequestException("TimespanNotFound");

            if (timespan.LessonReservations == null)
                timespan.LessonReservations = new List<LessonReservation>();

            if(timespan.LessonReservations.Any(r => r.ReservationDate == dto.ReservationDate))
                throw new BadRequestException("DateIsAlreadyReserved");

            var user = await _dbContext.Users
                .FirstOrDefaultAsync(u => u.Id == new Guid(dto.ReservedBy.Id))
                ?? throw new BadRequestException("UserNotFound");

            if(dto.ReservationDate.DayOfWeek.ToString() == schedule.DayOfWeek.ToString())
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
                        AvailableFrom = entityTimestamp.AvailableFrom.ToString(),
                        AvailableTo = entityTimestamp.AvailableTo.ToString(),
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
