using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.ExactSearchLessonOffers;

public sealed class ExactSearchLessonOffersQueryHandler 
    : IRequestHandler<ExactSearchLessonOffersQuery, IReadOnlyList<PrivateLessonOfferDto>>
{
    private readonly IPrivateLessonService _privateLessonService;

    public ExactSearchLessonOffersQueryHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> Handle(
        ExactSearchLessonOffersQuery request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.AdvancedSearchAsync(request);
    }
}
