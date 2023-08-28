using MediatR;
using Meedu.Models.Reservations.UserReservations;

namespace Meedu.Queries.GetReservationsForUsersLessons;

public record GetReservationsForUsersLessonsQuery(int Days) 
    : IRequest<IReadOnlyList<UserPrivateLessonReservationsDto>>;
