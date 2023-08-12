using MediatR;
using Meedu.Models.Auth;
using Meedu.Services;

namespace Meedu.Commands.RegisterUser;

public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, UserInfoDto>
{
    private readonly IAccountService _accountService;

    public RegisterUserCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    public async Task<UserInfoDto> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.RegisterUserAsync(request);
    }
}
