﻿using FluentValidation;
using Meedu.Commands.RegisterUser;
using Meedu.Entities;
using Meedu.Models.Auth;

namespace Meedu.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserDtoValidator(MeeduDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailTaken = dbContext.Users.Any(x => x.Email == value);

                    if (emailTaken)
                    {
                        context.AddFailure("Email", "EmailIsTaken");
                    }
                });
        }
    }
}
