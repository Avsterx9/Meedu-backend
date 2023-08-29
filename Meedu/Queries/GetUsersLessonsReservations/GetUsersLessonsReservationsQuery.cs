using MediatR;
using Meedu.Models.Reservations.UserReservations;

namespace Meedu.Queries.GetUsersLessonsReservations;

public record GetUsersLessonsReservationsQuery
    : IRequest<IReadOnlyList<UserReservationDataDto>>;