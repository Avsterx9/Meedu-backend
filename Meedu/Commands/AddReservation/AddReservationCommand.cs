using MediatR;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Commands.AddReservation;

public record AddReservationCommand(
    DateTime ReservationDate,
    Guid TimestampId,
    Guid LessonOfferId
    )
    : IRequest<LessonReservationDto>;
