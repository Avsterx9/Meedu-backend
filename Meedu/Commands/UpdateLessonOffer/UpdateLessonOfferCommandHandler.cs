using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Commands.UpdateLessonOffer;

public sealed class UpdateLessonOfferCommandHandler : IRequestHandler<UpdateLessonOfferCommand, PrivateLessonOfferDto>
{
    private readonly IPrivateLessonService _privateLessonService;

    public UpdateLessonOfferCommandHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<PrivateLessonOfferDto> Handle(UpdateLessonOfferCommand request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.DeleteLessonOfferAsync(request);
    }
}
