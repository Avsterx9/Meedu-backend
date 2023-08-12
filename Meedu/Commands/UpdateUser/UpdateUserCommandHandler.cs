using MediatR;
using Meedu.Models.Auth;
using Meedu.Services;

namespace Meedu.Commands.UpdateUser;

public sealed class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserInfoDto>
{
    private readonly IAccountService _accountService;

    public UpdateUserCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<UserInfoDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.UpdateUserDataAsync(request);
    }
}
