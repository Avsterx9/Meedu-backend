using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.GetAllLessonOffers;

public sealed class GetAllLessonOffersQueryHandler
    : IRequestHandler<GetAllLessonOffersQuery, IReadOnlyList<PrivateLessonOfferDto>>
{
    private readonly IPrivateLessonService _privateLessonService;

    public GetAllLessonOffersQueryHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<IReadOnlyList<PrivateLessonOfferDto>> Handle(GetAllLessonOffersQuery request,
        CancellationToken cancellationToken)
    {
        return await _privateLessonService.GetAllLessonOffersAsync();
    }
}
