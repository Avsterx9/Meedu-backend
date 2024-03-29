﻿using MediatR;
using Meedu.Commands.AddReservation;
using Meedu.Commands.AddSchedule;
using Meedu.Commands.AddTimestamp;
using Meedu.Commands.DeleteReservation;
using Meedu.Commands.DeleteSchedule;
using Meedu.Commands.DeleteTimestamp;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Response;
using Meedu.Models.Schedule;
using Meedu.Queries.GetReservationsByTimestamp;
using Meedu.Queries.GetReservationsByUser;
using Meedu.Queries.GetReservationsForUsersLessons;
using Meedu.Queries.GetScheduleByUser;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers;

[Route("api/schedule")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly ISender _sender;

    public ScheduleController(ISender sender)
    {
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
    public async Task<ActionResult<LessonReservationDto>> AddReservation([FromBody] AddReservationCommand command)
    {
        return Ok(await _sender.Send(command));
    }

    [HttpDelete("reservations/delete")]
    [Authorize]
    public async Task<ActionResult<DeleteReservationResponse>> RemoveReservation([FromQuery] DeleteReservationCommand command)
    {
        return Ok(await _sender.Send(command));
    }

    [HttpGet("reservations/getReservations")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<LessonReservationDto>>> GetReservationsByTimespanId(
        [FromQuery] GetReservationsByTimestampQuery query)
    {
        return Ok(await _sender.Send(query));
    }

    [HttpGet("reservations/getReservationsByUser")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<UserPrivateLessonReservationsDto>>> GetReservationsByUser(
        [FromQuery] GetReservationsByUserQuery query)
    {
        return Ok(await _sender.Send(query));
    }

    [HttpGet("reservations/getUserLessonReservations")]
    [Authorize]
    public async Task<ActionResult<List<UserPrivateLessonReservationsDto>>> GetUserLessonReservations(
        [FromQuery] GetReservationsForUsersLessonsQuery query)
    {
        return Ok(await _sender.Send(query));
    }
}
