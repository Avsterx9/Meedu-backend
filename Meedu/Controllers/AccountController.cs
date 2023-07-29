using Meedu.Models;
using Meedu.Models.Auth;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            await _accountService.RegisterUserAsync(registerUserDto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginAsync([FromBody] LoginUserDto loginDto)
        {
            string token = await _accountService.GenerateJwtTokenAsync(loginDto);
            return Ok(token);
        }

        [HttpGet("getUserInfo")]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> GetUserInfoAsync()
        {
            return Ok(await _accountService.GetUserInfoAsync());
        }


        [HttpPut("updateUserData")]
        [Authorize]
        public async Task<ActionResult<UserInfoDto>> UpdateUserDataAsync(UpdateUserDataRequest request)
        {
            await _accountService.UpdateUserDataAsync(request);
            return Ok();
        }

        [HttpPost("setUserImage")]
        [Authorize]
        public async Task<ActionResult> SetUserImageAsync(IFormFile file)
        {
            await _accountService.SetUserImageAsync(file);
            return Ok();
        }
    }
}
