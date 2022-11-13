using Meedu.Models;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _scheduleService;

        public ScheduleController(IScheduleService _scheduleService)
        {
            this._scheduleService = _scheduleService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<ActionResult> AddSchedule(ScheduleDto dto)
        {
            await _scheduleService.AddScheduleAsync(dto);
            return Ok();
        }
    }
}
