﻿using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers;

[Route("api/schedule")]
[ApiController]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult> AddSchedule(ScheduleDto dto)
    {
        await _scheduleService.AddScheduleAsync(dto);
        return Ok();
    }

    [HttpDelete("delete")]
    [Authorize]
    public async Task<ActionResult> DeleteSchedule(string scheduleId)
    {
        await _scheduleService.DeleteScheduleAsync(scheduleId);
        return Ok();
    }

    [HttpGet("getByLessonOffer")]
    public async Task<ActionResult<List<ScheduleDto>>> GetSchedule(string lessonOfferId)
    {
        return Ok(await _scheduleService.GetScheduleByLessonOfferId(lessonOfferId));
    }

    [HttpPost("timespans/add")]
    [Authorize]
    public async Task<ActionResult> AddTimespanToSchedule(
        [FromBody] ScheduleTimespanDto scheduleTimespanDto, Guid scheduleId)
    {
        await _scheduleService.AddTimestampToScheduleAsync(scheduleTimespanDto, scheduleId);
        return Ok();
    }

    [HttpDelete("timespans/delete")]
    [Authorize]
    public async Task<ActionResult> DeleteTimespanFromSchedule(Guid timespanId)
    {
        await _scheduleService.DeleteTimespanFromScheduleAsync(timespanId);
        return Ok();
    }

    [HttpPost("reservations/add")]
    [Authorize]
    public async Task<ActionResult> AddReservation([FromBody] LessonReservationDto reservation, Guid scheduleId, Guid timespanId)
    {
        await _scheduleService.AddReservationAsync(reservation, scheduleId, timespanId);
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
