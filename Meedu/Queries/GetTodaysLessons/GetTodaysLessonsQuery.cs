using MediatR;
using Meedu.Models.Reservations.UserReservations;

namespace Meedu.Queries.GetTodaysLessons;

public record GetTodaysLessonsQuery() 
    : IRequest<IReadOnlyList<UserReservationDataDto>>;
