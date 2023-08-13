using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.GetLessonOffersByUser;

public sealed class GetLessonOffersByUserQueryHandler 
    : IRequestHandler<GetLessonOffersByUserQuery, IReadOnlyList<PrivateLessonOfferDto>>
{
    private readonly IPrivateLessonService _privateLessonService;

    public GetLessonOffersByUserQueryHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> Handle(
        GetLessonOffersByUserQuery request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.GetLessonOffersByUserAsync();
    }
}
