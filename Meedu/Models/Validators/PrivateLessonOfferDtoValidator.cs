using FluentValidation;
using Meedu.Entities;

namespace Meedu.Models.Validators
{
    public class PrivateLessonOfferDtoValidator : AbstractValidator<PrivateLessonOfferDto>
    {
        public PrivateLessonOfferDtoValidator(MeeduDbContext dbContext)
        {

        }
    }
}
