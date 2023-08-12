using MediatR;
using Meedu.Models.Auth;

namespace Meedu.Commands.RegisterUser;

public record RegisterUserCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    DateTime? DateOfBirth,
    string FirstName,
    string LastName,
    string PhoneNumber,
    int RoleId = 4
    ) : IRequest<UserInfoDto>;