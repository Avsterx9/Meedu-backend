using MediatR;
using Meedu.Models.Response;

namespace Meedu.Commands.DeleteLessonOffer;

public record DeleteLessonOfferCommand(
    Guid LessonId
    ) : IRequest<DeleteLessonOfferResponse>;