using MediatR;
using Meedu.Models.Response;

namespace Meedu.Commands.DeleteReservation;

public record DeleteReservationCommand(Guid ReservationId)
    : IRequest<DeleteReservationResponse>;
