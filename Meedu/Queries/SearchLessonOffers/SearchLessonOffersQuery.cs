using MediatR;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Queries.LessonOffersSimpleSearch;

public record SearchLessonOffersQuery(
    string Value
    ) : IRequest<IReadOnlyList<PrivateLessonOfferDto>>; 
