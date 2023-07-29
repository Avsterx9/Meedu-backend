using Meedu.Models.Auth;
using Meedu.Models;

namespace Meedu.Services;

public interface IAccountService
{
    Task RegisterUserAsync(RegisterUserDto dto);
    Task<string> GenerateJwtTokenAsync(LoginUserDto loginDto);
    Task<UserInfoDto> GetUserInfoAsync();
    Task<UserInfoDto> UpdateUserDataAsync(UpdateUserDataRequest request);
    Task SetUserImageAsync(IFormFile file);
}

