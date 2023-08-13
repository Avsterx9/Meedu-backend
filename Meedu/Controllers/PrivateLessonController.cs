using MediatR;
using Meedu.Commands.CreateLessonOffer;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult> AddPrivateLesson([FromBody] CreateLessonOfferCommand command)
    {
        var res = await _sender.Send(command);
        return Ok(res);
    }

    //[HttpGet("list")]
    //public async Task<ActionResult<List<PrivateLessonOfferDto>>> GetAllLessonOffers()
    //{
    //    return Ok(await privateLessonService.GetAllLessonOffersAsync());
    //}

    //[HttpGet("getByUser")]
    //[Authorize]
    //public async Task<ActionResult<List<PrivateLessonOfferDto>>> GetLessonOffersByUser()
    //{
    //    return Ok(await privateLessonService.GetLessonOffersByUserAsync());
    //}

    //[HttpDelete("delete")]
    //[Authorize]
    //public async Task<ActionResult> DeleteLessonOffer(string id)
    //{
    //    await privateLessonService.DeleteLessonOfferAsync(id);
    //    return Ok();
    //}

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
