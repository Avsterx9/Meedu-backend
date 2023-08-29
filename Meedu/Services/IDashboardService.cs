using Meedu.Models.Reservations.UserReservations;
using Meedu.Models;

namespace Meedu.Services;

public interface IDashboardService
{
    Task<IReadOnlyList<UserReservationDataDto>> GetTodaysUserLessonsAsync();
    Task<List<UserReservationDataDto>> GetUsersLessonsReservationsAsync();
    Task<List<DtoNameLastnameId>> GetUserStudentsAsync(int amount);
}