using MediatR;
using Meedu.Models.Auth;
using Meedu.Services;

namespace Meedu.Queries.GetUserInfo;

public sealed class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserInfoDto>
{
    private readonly IAccountService _accountService;

    public GetUserInfoQueryHandler(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<UserInfoDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        return await _accountService.GetUserInfoAsync();
    }
}
