using MediatR;
using Meedu.Models.Auth;
using Meedu.Services;

namespace Meedu.Commands.SetUserImage;

public sealed class SetUserImageCommandHandler : IRequestHandler<SetUserImageCommand, UserInfoDto>
{
    private readonly IAccountService _accountService;

    public SetUserImageCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<UserInfoDto> Handle(SetUserImageCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.SetUserImageAsync(request);
    }
}
