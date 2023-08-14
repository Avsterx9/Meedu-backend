using MediatR;
using Meedu.Entities.Enums;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Queries.ExactSearchLessonOffers;

public record ExactSearchLessonOffersQuery(
    string? Subject, 
    string? FirstName,
    string? LastName,
    bool IsRemote, 
    string? City,
    TeachingRange? TeachingRange,
    decimal? PriceFrom,
    decimal? PriceTo
    ) : IRequest<IReadOnlyList<PrivateLessonOfferDto>>;
