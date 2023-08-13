using MediatR;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Queries.GetLessonOffersByUser;

public record GetLessonOffersByUserQuery : IRequest<IReadOnlyList<PrivateLessonOfferDto>>;
