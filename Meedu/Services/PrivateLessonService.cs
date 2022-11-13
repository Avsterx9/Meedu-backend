using Meedu.Entities;
using Meedu.Entities.Enums;
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
        Task UpdateLessonOffer(PrivateLessonOfferDto dto);
        Task<List<PrivateLessonOfferDto>> SimpleSearchByNameAsync(string searchValue);
        Task<List<PrivateLessonOfferDto>> AdvancedSearch(LessonOfferAdvancedSearchDto dto);
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
                    LessonTitle = dto.LessonTitle != null ? dto.LessonTitle : String.Empty,
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

        public async Task UpdateLessonOffer(PrivateLessonOfferDto dto)
        {
            if(dto.Id == null)
                throw new ArgumentNullException("OfferIdIsNull");

            var offerToEdit = await dbContext.PrivateLessonOffers.FirstOrDefaultAsync(o => o.Id == new Guid(dto.Id));

            if(offerToEdit == null)
                throw new BadRequestException("LessonOfferNotFound");

            var subject = await dbContext.Subjects.FirstOrDefaultAsync(s => s.Name.Contains(dto.Subject.Name));

            if (subject == null)
            {
                subject = new Subject()
                {
                    Name = dto.Subject.Name
                };
                await dbContext.Subjects.AddAsync(subject);
            }

            offerToEdit.LessonTitle = dto.LessonTitle != null ? dto.LessonTitle : String.Empty;
            offerToEdit.Price = dto.Price;
            offerToEdit.City = dto.City;
            offerToEdit.OnlineLessonsPossible = dto.isOnline;
            offerToEdit.Description = dto.Description;
            offerToEdit.Subject = subject;
            offerToEdit.Place = dto.Place;
            offerToEdit.TeachingRange = dto.TeachingRange;

            await dbContext.SaveChangesAsync();
        }

        public async Task<List<PrivateLessonOfferDto>> SimpleSearchByNameAsync(string searchValue)
        {
            var lessons = await dbContext.PrivateLessonOffers
                .Include(o => o.CreatedBy)
                .Include(o => o.Subject)
                .ToListAsync();

            if (!String.IsNullOrEmpty(searchValue))
                lessons = lessons.Where(o => o.LessonTitle.ToLower().Contains(searchValue.ToLower())).ToList();

            return lessons.Select(o => CreateLessonOfferDto(o)).ToList();
        }

        public async Task<List<PrivateLessonOfferDto>> AdvancedSearch(LessonOfferAdvancedSearchDto dto)
        {
            var lessons = await dbContext.PrivateLessonOffers
                .Include(o => o.CreatedBy)
                .Include(o => o.Subject)
                .ToListAsync();

            if (!String.IsNullOrEmpty(dto.Subject))
                lessons = lessons.Where(o => o.Subject.Name.Equals(dto.Subject)).ToList();

            if (!String.IsNullOrEmpty(dto.LastName))
                lessons = lessons.Where(o => o.CreatedBy.LastName.Equals(dto.LastName)).ToList();

            if (!String.IsNullOrEmpty(dto.FirstName))
                lessons = lessons.Where(o => o.CreatedBy.FirstName.Equals(dto.FirstName)).ToList();

            if (!String.IsNullOrEmpty(dto.City))
                lessons = lessons.Where(o => o.City.Equals(dto.City)).ToList();

            if(dto.TeachingRange != null)
                lessons = lessons.Where(o => o.TeachingRange == dto.TeachingRange).ToList();

            if(dto.IsOnline != null)
                lessons = lessons.Where(o => o.OnlineLessonsPossible == dto.IsOnline).ToList();

            if(dto.PriceFrom != null)
                lessons = lessons.Where(o => o.Price > dto.PriceFrom).ToList();

            if (dto.PriceTo != null)
                lessons = lessons.Where(o => o.Price < dto.PriceTo).ToList();

            return lessons.Select(o => CreateLessonOfferDto(o)).ToList();
        }

        private PrivateLessonOfferDto CreateLessonOfferDto(PrivateLessonOffer offer)
        {
            return new PrivateLessonOfferDto()
            {
                Id = offer.Id.ToString(),
                City = offer.City,
                Description = offer.Description != null ? offer.Description : "",
                isOnline = offer.OnlineLessonsPossible,
                LessonTitle = offer.LessonTitle,
                Place = offer.Place,
                Price = offer.Price,
                Subject = new DtoNameId()
                {
                    Id = offer.Subject.Id.ToString(),
                    Name = offer.Subject.Name
                },
                TeachingRange = offer.TeachingRange != null ? offer.TeachingRange.Value : TeachingRange.Other,
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
