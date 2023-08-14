using MediatR;
using Meedu.Commands.CreateLessonOffer;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Commands.UpdateLessonOffer;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Response;
using Meedu.Queries.ExactSearchLessonOffers;
using Meedu.Queries.GetAllLessonOffers;
using Meedu.Queries.GetLessonOfferById;
using Meedu.Queries.GetLessonOffersByUser;
using Meedu.Queries.LessonOffersSimpleSearch;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Update.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Meedu.Controllers;

[Route("api/lessons")]
[ApiController]
public class PrivateLessonController : ControllerBase
{
    private readonly ISender _sender;

    public PrivateLessonController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("add")]
    [Authorize]
    public async Task<ActionResult> AddPrivateLessonAsync([FromBody] CreateLessonOfferCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpGet("list")]
    public async Task<ActionResult<IReadOnlyList<PrivateLessonOfferDto>>> GetAllLessonOffersAsync()
    {
        var res = await _sender.Send(new GetAllLessonOffersQuery());
        return Ok(res);
    }

    [HttpGet("getByUser")]
    [Authorize]
    public async Task<ActionResult<IReadOnlyList<PrivateLessonOfferDto>>> GetLessonOffersByUserAsync()
    {
        var res = await _sender.Send(new GetLessonOffersByUserQuery());
        return Ok(res);
    }

    [HttpDelete("delete")]
    [Authorize]
    public async Task<ActionResult<DeleteLessonOfferResponse>> DeleteLessonOffer(
        [FromQuery] DeleteLessonOfferCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpGet("getById")]
    [Authorize]
    public async Task<ActionResult<PrivateLessonOfferDto>> GetById(
        [FromQuery] GetLessonOfferByIdQuery query)
    {
        var res = await _sender.Send(query);
        return Ok(res);
    }

    [HttpPut("update")]
    [Authorize]
    public async Task<ActionResult> GetById([FromBody] UpdateLessonOfferCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IReadOnlyList<PrivateLessonOfferDto>>> SimpleSearchByNameAsync(
        [FromBody] SearchLessonOffersQuery query)
    {
        var res = await _sender.Send(query);
        return Ok(res);
    }

    [HttpPost("advancedSearch")]
    public async Task<ActionResult<IReadOnlyList<PrivateLessonOfferDto>>> AdvancedSearchAsync(
        [FromQuery] ExactSearchLessonOffersQuery query)
    {
        var res = await _sender.Send(query);
        return Ok(res);
    }
}
