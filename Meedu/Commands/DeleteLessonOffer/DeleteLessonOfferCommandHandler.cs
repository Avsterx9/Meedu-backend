using MediatR;
using Meedu.Models.Response;
using Meedu.Services;

namespace Meedu.Commands.DeleteLessonOffer;

public sealed class DeleteLessonOfferCommandHandler
    : IRequestHandler<DeleteLessonOfferCommand, DeleteLessonOfferResponse>
{
    private readonly IPrivateLessonService _privateLessonService;

    public DeleteLessonOfferCommandHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<DeleteLessonOfferResponse> Handle(DeleteLessonOfferCommand request,
        CancellationToken cancellationToken)
    {
        return await _privateLessonService.DeleteLessonOfferAsync(request);
    }
}
