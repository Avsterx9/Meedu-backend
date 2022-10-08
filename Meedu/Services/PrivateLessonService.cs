using Meedu.Entities;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IPrivateLessonService
    {
        Task AddPrivateLesson(PrivateLessonOfferDto dto);
        Task<List<PrivateLessonOfferDto>> GetAllLessonOffers();
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

        public async Task AddPrivateLesson(PrivateLessonOfferDto dto)
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

        public async Task<List<PrivateLessonOfferDto>> GetAllLessonOffers()
        {
            var lessonOffers = await dbContext.PrivateLessonOffers
                .Include(s => s.Subject)
                .Include(s => s.CreatedBy)
                .ToListAsync();

            var lessonOfferDtoList = lessonOffers.Select(o => CreateLessonOfferDto(o)).ToList();
            return lessonOfferDtoList;
        }

        private PrivateLessonOfferDto CreateLessonOfferDto(PrivateLessonOffer offer)
        {
            return new PrivateLessonOfferDto()
            {
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
