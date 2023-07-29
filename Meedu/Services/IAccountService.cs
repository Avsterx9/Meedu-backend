using Meedu.Models.Auth;
using Meedu.Models;

namespace Meedu.Services;

public interface IAccountService
{
    void RegisterUser(RegisterUserDto dto);
    string GenerateJwtToken(LoginUserDto loginDto);
    Task<UserInfoDto> GetUserInfo();
    Task<UserInfoDto> UpdateUserDataAsync(UpdateUserDataRequest request);
    Task SetUserImageAsync(IFormFile file);
}

