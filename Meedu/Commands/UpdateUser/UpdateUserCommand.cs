using MediatR;
using Meedu.Models.Auth;
using System.Drawing;

namespace Meedu.Commands.UpdateUser;

public record UpdateUserCommand(
    string PhoneNumber,
    string FirstName,
    string LastName,
    DateTime DateOfBirth
    ) : IRequest<UserInfoDto>;
