using Meedu.Models;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Authorize]
    [Route("api/lessons")]
    [ApiController]
    public class PrivateLessonController : ControllerBase
    {
        private readonly IPrivateLessonService privateLessonService;

        public PrivateLessonController(IPrivateLessonService privateLessonService)
        {
            this.privateLessonService = privateLessonService;
        }

        [HttpPost("add")]
        public async Task<ActionResult> AddPrivateLesson([FromBody] PrivateLessonOfferDto dto)
        {
            await privateLessonService.AddPrivateLesson(dto);
            return Ok();
        }
    }
}
