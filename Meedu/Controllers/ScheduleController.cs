using MediatR;
using Meedu.Commands.AddSchedule;
using Meedu.Commands.AddTimestamp;
using Meedu.Commands.DeleteSchedule;
using Meedu.Commands.DeleteTimestamp;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;
using Meedu.Queries.GetScheduleByUser;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Meedu.Controllers;

[Route("api/schedule")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;
    private readonly ISender _sender;

    public ScheduleController(IScheduleService scheduleService, ISender sender)
    {
        _scheduleService = scheduleService;
        _sender = sender;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult> AddScheduleAsync([FromBody] AddScheduleCommand command)
    {
        return Ok(await _sender.Send(command));
    }

    [HttpDelete("delete")]
    [Authorize]
    public async Task<ActionResult> DeleteScheduleAsync([FromQuery] DeleteScheduleCommand command)
    {
        return Ok(await _sender.Send(command));
    }

    [HttpGet("getByUser")]
    public async Task<ActionResult<List<ScheduleDto>>> GetScheduleAsync([FromQuery] GetScheduleByUserQuery query)
    {
        return Ok(await _sender.Send(query));
    }

    [HttpPost("timespans/add")]
    [Authorize]
    public async Task<ActionResult> AddTimespanToScheduleAsync(
        [FromBody] AddTimestampCommand command)
    {
        return Ok(await _sender.Send(command));
    }

    [HttpDelete("timespans/delete")]
    [Authorize]
    public async Task<ActionResult> DeleteTimespanFromSchedule([FromQuery] DeleteTimestampCommand command)
    {
        return Ok(await _sender.Send(command));
    }

    [HttpPost("reservations/add")]
    [Authorize]
    public async Task<ActionResult> AddReservation([FromBody] LessonReservationDto reservation, Guid scheduleId, Guid timespanId)
    {
        await _scheduleService.AddReservationAsync(reservation, timespanId);
        return Ok();
    }

    [HttpDelete("reservations/delete")]
    [Authorize]
    public async Task<ActionResult> RemoveReservation(Guid reservationId)
    {
        await _scheduleService.DeleteReservationAsync(reservationId);
        return Ok();
    }

    [HttpGet("reservations/getReservations")]
    [Authorize]
    public async Task<ActionResult<LessonReservationDto>> GetReservationsByTimespanId(Guid scheduleId, Guid timespanId)
    {
        return Ok(await _scheduleService.GetReservationsByTimespanIdAsync(scheduleId, timespanId));
    }

    [HttpGet("reservations/getReservationsByUser")]
    [Authorize]
    public async Task<ActionResult<List<UserPrivateLessonReservationsDto>>> GetReservationsByUser(int days)
    {
        return Ok(await _scheduleService.GetReservationsByUserAsync(days));
    }

    [HttpGet("reservations/getUserLessonReservations")]
    [Authorize]
    public async Task<ActionResult<List<UserPrivateLessonReservationsDto>>> GetUserLessonReservations(int days)
    {
        return Ok(await _scheduleService.GetUserLessonReservationsAsync(days));
    }
}
