using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.GetLessonOfferById;

public sealed class GetLessonOfferByIdQueryHandler : IRequestHandler<GetLessonOfferByIdQuery, PrivateLessonOfferDto>
{
    private readonly IPrivateLessonService _privateLessonService;

    public GetLessonOfferByIdQueryHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<PrivateLessonOfferDto> Handle(GetLessonOfferByIdQuery request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.GetByIdAsync(request);
    }
}
