using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Commands.CreateLessonOffer;

public sealed class CreateLessonOfferCommandHandler : IRequestHandler<CreateLessonOfferCommand, PrivateLessonOfferDto>
{
    private readonly IPrivateLessonService _privateLessonService;

    public CreateLessonOfferCommandHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<PrivateLessonOfferDto> Handle(CreateLessonOfferCommand request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.AddPrivateLessonAsync(request);
    }
}
