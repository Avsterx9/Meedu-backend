using Meedu.Models;
using Meedu.Services;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Route("api/accounts")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto registerUserDto)
        {
            accountService.RegisterUser(registerUserDto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto loginDto)
        {
            string token = accountService.GenerateJwtToken(loginDto);
            return null;
        }
    }
}
