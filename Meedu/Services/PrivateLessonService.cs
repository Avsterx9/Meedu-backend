﻿using Meedu.Entities;
using Meedu.Exceptions;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IPrivateLessonService
    {
        Task AddPrivateLessonAsync(PrivateLessonOfferDto dto);
        Task<List<PrivateLessonOfferDto>> GetAllLessonOffersAsync();
        Task<List<PrivateLessonOfferDto>> GetLessonOffersByUserAsync();
        Task DeleteLessonOfferAsync(string id);
        Task<PrivateLessonOfferDto> GetByIdAsync(string id);
    }

    public class PrivateLessonService : IPrivateLessonService
    {
        private readonly MeeduDbContext dbContext;
        private readonly ILogger<PrivateLessonService> logger;
        private readonly IUserContextService userContextService;

        public PrivateLessonService(MeeduDbContext dbContext, ILogger<PrivateLessonService> logger, IUserContextService userContextService)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.userContextService = userContextService;
        }

        public async Task AddPrivateLessonAsync(PrivateLessonOfferDto dto)
        {
            var userID = userContextService.GetUserId;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userID);

            var subject = await dbContext.Subjects
                .FirstOrDefaultAsync(s => s.Name.Contains(dto.Subject.Name));

            if (subject == null)
            {
                subject = new Subject() { Name = dto.Subject.Name };
                dbContext.Subjects.Add(subject);
            }

            if (user != null)
            {
                var newLesson = new PrivateLessonOffer()
                {
                    City = dto.City,
                    Description = dto.Description,
                    LessonTitle = dto.LessonTitle,
                    OnlineLessonsPossible = dto.isOnline,
                    Place = dto.Place,
                    Price = dto.Price,
                    TeachingRange = dto.TeachingRange,
                    CreatedBy = user,
                    Subject = subject
                };

                dbContext.PrivateLessonOffers.Add(newLesson);
            }
            await dbContext.SaveChangesAsync();
        }

        public async Task<List<PrivateLessonOfferDto>> GetAllLessonOffersAsync()
        {
            var lessonOffers = await dbContext.PrivateLessonOffers
                .Include(s => s.Subject)
                .Include(s => s.CreatedBy)
                .ToListAsync();

            var lessonOfferDtoList = lessonOffers.Select(o => CreateLessonOfferDto(o)).ToList();
            return lessonOfferDtoList;
        }

        public async Task<List<PrivateLessonOfferDto>> GetLessonOffersByUserAsync()
        {
            var userID = userContextService.GetUserId;
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userID);

            var userOffers = await dbContext.PrivateLessonOffers
                .Where(u => u.CreatedBy.Id == userID)
                .Include(s => s.Subject)
                .Include(s => s.CreatedBy)
                .ToListAsync();

            var userOfferDtos = userOffers.Select(o => CreateLessonOfferDto(o)).ToList();

            return userOfferDtos;
        }

        public async Task DeleteLessonOfferAsync(string id)
        {
            var lessonOffer = await dbContext.PrivateLessonOffers
                .FirstOrDefaultAsync(o => o.Id == new Guid(id));

            if (lessonOffer == null)
                throw new BadRequestException("OfferNotFound");

            dbContext.PrivateLessonOffers.Remove(lessonOffer);
            await dbContext.SaveChangesAsync();
        }


        public async Task<PrivateLessonOfferDto> GetByIdAsync(string id)
        {
            var lesson = await dbContext.PrivateLessonOffers
                .Include(x => x.CreatedBy)
                .Include(x => x.Subject)
                .FirstOrDefaultAsync(o => o.Id == new Guid(id));

            if (lesson == null)
                throw new BadRequestException("LessonOfferNotFound");

            var dto = CreateLessonOfferDto(lesson);
            return dto;
        }

        private PrivateLessonOfferDto CreateLessonOfferDto(PrivateLessonOffer offer)
        {
            return new PrivateLessonOfferDto()
            {
                Id = offer.Id.ToString(),
                City = offer.City,
                Description = offer.Description,
                isOnline = offer.OnlineLessonsPossible,
                LessonTitle = offer.LessonTitle,
                Place = offer.Place,
                Price = offer.Price,
                Subject = new DtoNameId()
                { 
                    Id = offer.Subject.Id.ToString(),
                    Name = offer.Subject.Name
                },
                TeachingRange = offer.TeachingRange.Value,
                User = new DtoNameLastnameId() 
                { 
                    Id = offer.CreatedBy.Id.ToString(),
                    FirstName = offer.CreatedBy.FirstName,
                    LastName = offer.CreatedBy.LastName,
                }
            };
        }
    }
}
