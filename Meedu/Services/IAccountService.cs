using Meedu.Models.Auth;
using Meedu.Models;
using Meedu.Commands.RegisterUser;
using Meedu.Commands.Login;
using Meedu.Commands.UpdateUser;
using Meedu.Commands.SetUserImage;

namespace Meedu.Services;

public interface IAccountService
{
    Task<UserInfoDto> RegisterUserAsync(RegisterUserCommand command);
    Task<string> GenerateJwtTokenAsync(LoginCommand command);
    Task<UserInfoDto> GetUserInfoAsync();
    Task<UserInfoDto> UpdateUserDataAsync(UpdateUserCommand command);
    Task<UserInfoDto> SetUserImageAsync(SetUserImageCommand command);
}

