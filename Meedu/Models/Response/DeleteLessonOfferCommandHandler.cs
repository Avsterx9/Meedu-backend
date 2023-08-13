using MediatR;
using Meedu.Commands.DeleteLessonOffer;
using Meedu.Services;

namespace Meedu.Models.Response;

public sealed class DeleteLessonOfferCommandHandler 
    : IRequestHandler<DeleteLessonOfferCommand, DeleteLessonOfferResponse>
{
    private readonly IPrivateLessonService _privateLessonService;

    public DeleteLessonOfferCommandHandler(IPrivateLessonService privateLessonService)
    {
        _privateLessonService = privateLessonService;
    }

    public async Task<DeleteLessonOfferResponse> Handle(DeleteLessonOfferCommand request, CancellationToken cancellationToken)
    {
        return await _privateLessonService.DeleteLessonOfferAsync(request);
    }
}
