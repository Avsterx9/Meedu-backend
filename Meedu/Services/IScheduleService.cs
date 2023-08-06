using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;

namespace Meedu.Services;

public interface IScheduleService
{
    Task AddScheduleAsync(ScheduleDto dto);
    Task DeleteScheduleAsync(Guid scheduleId);
    Task<List<ScheduleDto>> GetScheduleByLessonOfferId(Guid lessonId);
    Task AddTimestampToScheduleAsync(ScheduleTimespanDto dto, Guid scheduleId);
    Task DeleteTimespanFromScheduleAsync(Guid timespanId);
    Task AddReservationAsync(LessonReservationDto dto, Guid timespanId);
    Task DeleteReservationAsync(Guid reservationId);
    Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(Guid scheduleId, Guid timespanId);
    Task<List<UserPrivateLessonReservationsDto>> GetReservationsByUserAsync(int days);
    Task<List<UserPrivateLessonReservationsDto>> GetUserLessonReservationsAsync(int days);
}
