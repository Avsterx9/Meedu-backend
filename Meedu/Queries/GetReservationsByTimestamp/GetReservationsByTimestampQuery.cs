using MediatR;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Queries.GetReservationsByTimestamp;

public record GetReservationsByTimestampQuery(Guid TimestampId) 
    : IRequest<IReadOnlyList<LessonReservationDto>>;