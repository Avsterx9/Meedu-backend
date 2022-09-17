using Meedu.Entities;
using Meedu.Models;
using Microsoft.EntityFrameworkCore;

namespace Meedu.Services
{
    public interface IPrivateLessonService
    {
        Task AddPrivateLesson(PrivateLessonOfferDto dto);
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
    }
}
