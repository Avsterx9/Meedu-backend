using Meedu.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        //private readonly ISubjectService subjectService;

        [HttpGet("list")]
        public async Task<ActionResult> AddPrivateLesson()
        {
            return Ok();
        }
    }
}
