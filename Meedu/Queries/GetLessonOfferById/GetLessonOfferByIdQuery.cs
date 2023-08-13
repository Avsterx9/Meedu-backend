using MediatR;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Queries.GetLessonOfferById;

public record GetLessonOfferByIdQuery(
    Guid LessonId
    ) : IRequest<PrivateLessonOfferDto>;