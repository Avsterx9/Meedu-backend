using Meedu.Models;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
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
        [Authorize]
        public async Task<ActionResult> AddPrivateLesson([FromBody] PrivateLessonOfferDto dto)
        {
            await privateLessonService.AddPrivateLesson(dto);
            return Ok();
        }

        [HttpGet("list")]
        public async Task<ActionResult> GetAllLessonOffers()
        {
            return Ok(await privateLessonService.GetAllLessonOffers());
        }

        [HttpGet("getByUser")]
        public async Task<ActionResult> GetLessonOffersByUser()
        {
            return Ok(await privateLessonService.GetLessonOffersByUser());
        }
    }
}
