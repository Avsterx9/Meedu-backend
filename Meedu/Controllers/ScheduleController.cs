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

        [HttpPost("getByUserAndSubject")]
        public async Task<ActionResult> GetSchedule(string subjectId, string userId)
        {
            return Ok(await _scheduleService.GetSubjectByUserAndSubjectAsync(subjectId, userId));
        }

        [HttpPost("timespans/add")]
        //[Authorize]
        public async Task<ActionResult> AddTimespanToSchedule([FromBody] ScheduleTimespanDto scheduleTimespanDto, string scheduleId)
        {
            await _scheduleService.AddTimespanToScheduleAsync(scheduleTimespanDto, scheduleId);
            return Ok();
        }

        [HttpPost("timespans/delete")]
        //[Authorize]
        public async Task<ActionResult> DeleteTimespanFromSchedule(string timespanId, string scheduleId)
        {
            await _scheduleService.DeleteTimespanFromScheduleAsync(timespanId, scheduleId);
            return Ok();
        }
    }
}
