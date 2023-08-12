using MediatR;
using Meedu.Services;

namespace Meedu.Commands.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, string>
{
    private readonly IAccountService _accountService;

    public LoginCommandHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<string> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        return await _accountService.GenerateJwtTokenAsync(request);
    }
}
