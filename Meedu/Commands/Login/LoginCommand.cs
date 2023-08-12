using MediatR;

namespace Meedu.Commands.Login;

public record LoginCommand(
    string Email, 
    string Password
    ) : IRequest<string>;
