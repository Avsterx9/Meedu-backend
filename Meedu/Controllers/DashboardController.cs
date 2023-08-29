using MediatR;
using Meedu.Models;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Queries.GetReservationsByTimestamp;
using Meedu.Queries.GetTodaysLessons;
using Meedu.Queries.GetUsersLessonsReservations;
using Meedu.Queries.GetUserStudents;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers;

[Route("api/dashboard")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly ISender _sender;

    public DashboardController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("getTodaysUserLessons")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<UserReservationDataDto>>> GetTodaysUserLessons()
    {
        return Ok(await _sender.Send(new GetTodaysLessonsQuery()));
    }

    [HttpGet("getTodayUsersLessonsReservations")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<UserReservationDataDto>>> GetTodayUsersLessonsReservationsAsync()
    {
        return Ok(await _sender.Send(new GetUsersLessonsReservationsQuery()));
    }

    [HttpGet("getUserStudents")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<DtoNameLastnameId>>> GetUserStudentsAsync(
        [FromQuery] GetUserStudentsQuery query)
    {
        return Ok(await _sender.Send(query));
    }
}
