using MediatR;
using Meedu.Commands.CreateLessonOffer;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Response;
using Meedu.Queries.GetAllLessonOffers;
using Meedu.Queries.GetLessonOffersByUser;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    //[HttpGet("getById")]
    //[Authorize]
    //public async Task<ActionResult<PrivateLessonOfferDto>> GetById(string id)
    //{
    //    return Ok(await privateLessonService.GetByIdAsync(id));
    //}

    //[HttpPut("update")]
    //[Authorize]
    //public async Task<ActionResult> GetById([FromBody] PrivateLessonOfferDto dto)
    //{
    //    await privateLessonService.UpdateLessonOfferAsync(dto);
    //    return Ok();
    //}

    //[HttpGet("search")]
    //public async Task<ActionResult<List<PrivateLessonOfferDto>>> SimpleSearchByName(string? searchValue = "")
    //{
    //    if (searchValue == null)
    //        searchValue = String.Empty;

    //    return Ok(await privateLessonService.SimpleSearchByNameAsync(searchValue));
    //}

    //[HttpPost("advancedSearch")]
    //public async Task<ActionResult<List<PrivateLessonOfferDto>>> AdvancedSearch(LessonOfferAdvancedSearchDto dto)
    //{
    //    return Ok(await privateLessonService.AdvancedSearchAsync(dto));
    //}
}
