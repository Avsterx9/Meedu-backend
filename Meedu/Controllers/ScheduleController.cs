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

        [HttpGet("getByUserAndSubject")]
        public async Task<ActionResult> GetSchedule(string subjectId, string userId)
        {
            return Ok(await _scheduleService.GetSubjectByUserAndSubjectAsync(subjectId, userId));
        }

        [HttpPost("timespans/add")]
        [Authorize]
        public async Task<ActionResult> AddTimespanToSchedule([FromBody] ScheduleTimespanDto scheduleTimespanDto, string scheduleId)
        {
            await _scheduleService.AddTimespanToScheduleAsync(scheduleTimespanDto, scheduleId);
            return Ok();
        }

        [HttpDelete("timespans/delete")]
        [Authorize]
        public async Task<ActionResult> DeleteTimespanFromSchedule(string timespanId)
        {
            await _scheduleService.DeleteTimespanFromScheduleAsync(timespanId);
            return Ok();
        }

        [HttpPost("reservations/add")]
        //[Authorize]
        public async Task<ActionResult> AddReservation([FromBody] LessonReservationDto reservation,string scheduleId, string timespanId)
        {
            await _scheduleService.AddReservationAsync(reservation, scheduleId, timespanId);
            return Ok();
        }

        [HttpDelete("reservations/delete")]
        //[Authorize]
        public async Task<ActionResult> RemoveReservation(string reservationId)
        {
            await _scheduleService.DeleteReservationAsync(reservationId);
            return Ok();
        }
    }
}
