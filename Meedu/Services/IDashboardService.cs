using Meedu.Models.Reservations.UserReservations;
using Meedu.Models;
using Meedu.Queries.GetUserStudents;

namespace Meedu.Services;

public interface IDashboardService
{
    Task<IReadOnlyList<UserReservationDataDto>> GetTodaysUserLessonsAsync();
    Task<IReadOnlyList<UserReservationDataDto>> GetUsersLessonsReservationsAsync();
    Task<IReadOnlyList<DtoNameLastnameId>> GetUserStudentsAsync(GetUserStudentsQuery query);
}