using FluentValidation;
using Meedu.Entities;

namespace Meedu.Models.Validators
{
    public class PrivateLessonOfferDtoValidator : AbstractValidator<PrivateLessonOfferDto>
    {
        public PrivateLessonOfferDtoValidator(MeeduDbContext dbContext)
        {
            RuleFor(x => x.User)
                .Custom((value, context) =>
                {
                    var user = dbContext.Users.FirstOrDefault(x => x.Id == value.Id);

                    if(user == null)
                    {
                        context.AddFailure("User", "User does not exist");
                    }
                });
        }
    }
}
