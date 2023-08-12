using MediatR;
using Meedu.Commands.Login;
using Meedu.Commands.RegisterUser;
using Meedu.Commands.SetUserImage;
using Meedu.Commands.UpdateUser;
using Meedu.Models;
using Meedu.Models.Auth;
using Meedu.Queries.GetUserInfo;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Meedu.Controllers;

[Route("api/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly ISender _sender;

    public AccountController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserInfoDto>> RegisterUser([FromBody] RegisterUserCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpPost("login")]
    public async Task<ActionResult<string>> LoginAsync([FromBody] LoginCommand command)
    {
        string token = await _sender.Send(command);
        return Ok(token);
    }

    [HttpGet("getUserInfo")]
    [Authorize]
    public async Task<ActionResult<UserInfoDto>> GetUserInfoAsync()
    {
        var res = await _sender.Send(new GetUserInfoQuery());
        return Ok(res);
    }

    [HttpPut("updateUserData")]
    [Authorize]
    public async Task<ActionResult<UserInfoDto>> UpdateUserDataAsync(UpdateUserCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpPost("setUserImage")]
    [Authorize]
    public async Task<ActionResult> SetUserImageAsync(IFormFile file)
    {
        var res = await _sender.Send(new SetUserImageCommand(file));
        return Ok(res);
    }
}
