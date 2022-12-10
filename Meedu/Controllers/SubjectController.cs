using Meedu.Entities;
using Meedu.Models;
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
        public async Task<ActionResult<List<SubjectDto>>> GetAll()
        {
            return Ok(await _subjectService.GetAll());
        }

        [HttpPost("add")]
        public async Task<ActionResult> Add([FromBody] String name)
        {
            await _subjectService.AddSubject(name);
            return Ok();
        }
    }
}
