using Meedu.Models;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers;

[Route("api/dashboard")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    [HttpGet("getTodaysUserLessons")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<UserReservationDataDto>>> GetTodaysUserLessons()
    {
        return Ok(await _dashboardService.GetTodaysUserLessonsAsync());
    }

    [HttpGet("getTodayUsersLessonsReservations")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<UserReservationDataDto>>> GetTodayUsersLessonsReservationsAsync()
    {
        return Ok(await _dashboardService.GetUsersLessonsReservationsAsync());
    }

    [HttpGet("getUserStudents")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<DtoNameLastnameId>>> GetUserStudentsAsync(int amount)
    {
        return Ok(await _dashboardService.GetUserStudentsAsync(amount));
    }
}
