using MediatR;
using Meedu.Models.Auth;

namespace Meedu.Queries.GetUserInfo;

public record GetUserInfoQuery() : IRequest<UserInfoDto>;
