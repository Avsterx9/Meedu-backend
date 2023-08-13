using MediatR;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Queries.GetAllLessonOffers;

public record GetAllLessonOffersQuery() : IRequest<IReadOnlyList<PrivateLessonOfferDto>>;
