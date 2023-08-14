using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.LessonOffersSimpleSearch;

public sealed class LessonOfferSearchQueryHandler
    : IRequestHandler<SearchLessonOffersQuery, IReadOnlyList<PrivateLessonOfferDto>>
{
    private readonly IPrivateLessonService _privateLessonService;

    public LessonOfferSearchQueryHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> Handle(
        SearchLessonOffersQuery request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.SimpleSearchByNameAsync(request);
    }
}
