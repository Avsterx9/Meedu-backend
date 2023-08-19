using MediatR;
using Meedu.Commands.AddSubject;
using Meedu.Models;
using Microsoft.AspNetCore.Mvc;

namespace Meedu.Controllers;

[Route("api/subjects")]
[ApiController]
public class SubjectController : ControllerBase
{
    private readonly ISender _sender;

    public SubjectController(ISender sender)
    {
        _sender = sender;
    }
    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<SubjectDto>>> GetAll()
    {
        return Ok(await _sender.Send(new GetAllSubjectsQuery()));
    }

    [HttpPost("add")]
    public async Task<ActionResult> Add([FromBody] AddSubjectCommand command)
    {
        return Ok(await _sender.Send(command));
    }
}
