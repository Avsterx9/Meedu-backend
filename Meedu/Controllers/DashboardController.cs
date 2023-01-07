using Meedu.Models.Reservations.UserReservations;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
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
        public async Task<ActionResult<List<UserReservationDataDto>>> GetUserInfo()
        {
            return Ok(await _dashboardService.GetTodaysUserLessons());
        }
    }
}
