using Meedu.Entities;
using Meedu.Services;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers
{
    [Route("api/subjects")]
    [ApiController]
    public class SubjectController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService _subjectService)
        {
            this._subjectService = _subjectService;
        }
        [HttpGet("list")]
        public async Task<ActionResult> GetAll()
        {
            return Ok(await _subjectService.GetAll());
        }
    }
}
