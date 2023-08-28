using MediatR;
using Meedu.Models.Reservations.UserReservations;

namespace Meedu.Queries.GetReservationsByUser;

public record GetReservationsByUserQuery(int Days) 
    : IRequest<IReadOnlyList<UserPrivateLessonReservationsDto>>;
