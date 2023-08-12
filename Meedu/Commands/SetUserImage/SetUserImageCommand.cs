using MediatR;
using Meedu.Models.Auth;

namespace Meedu.Commands.SetUserImage;

public record SetUserImageCommand(
    IFormFile file
    ) : IRequest<UserInfoDto>;