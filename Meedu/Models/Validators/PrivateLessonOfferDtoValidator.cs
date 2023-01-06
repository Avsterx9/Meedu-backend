using FluentValidation;
using Meedu.Entities;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Models.Validators
{
    public class PrivateLessonOfferDtoValidator : AbstractValidator<PrivateLessonOfferDto>
    {
        public PrivateLessonOfferDtoValidator(MeeduDbContext dbContext)
        {

        }
    }
}
