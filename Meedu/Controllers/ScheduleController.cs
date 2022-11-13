using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult> AddSchedule()
        {
            //await privateLessonService.AddPrivateLessonAsync(dto);
            return Ok();
        }
    }
}
