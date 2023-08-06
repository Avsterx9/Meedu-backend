using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Schedule;

namespace Meedu.Services;

public interface IScheduleService
{
    Task AddScheduleAsync(ScheduleDto dto);
    Task DeleteScheduleAsync(string scheduleId);
    Task<List<ScheduleDto>> GetScheduleByLessonOfferId(string lessonId);
    Task AddTimespanToScheduleAsync(ScheduleTimespanDto dto, string scheduleId);
    Task DeleteTimespanFromScheduleAsync(string timespanId);
    Task AddReservationAsync(LessonReservationDto dto, string scheduleId, string timespanId);
    Task DeleteReservationAsync(string reservationId);
    Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(string scheduleId, string timespanId);
    Task<List<UserPrivateLessonReservationsDto>> GetReservationsByUserAsync(int days);
    Task<List<UserPrivateLessonReservationsDto>> GetUserLessonReservationsAsync(int days);
}
