using Meedu.Commands.AddReservation;
using Meedu.Commands.AddSchedule;
using Meedu.Commands.AddTimestamp;
using Meedu.Commands.DeleteSchedule;
using Meedu.Commands.DeleteTimestamp;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Models.Response;
using Meedu.Models.Schedule;
using Meedu.Queries.GetScheduleByUser;

namespace Meedu.Services;

public interface IScheduleService
{
    Task<ScheduleDto> AddScheduleAsync(AddScheduleCommand command);
    Task<DeleteScheduleResponse> DeleteScheduleAsync(DeleteScheduleCommand command);
    Task<IReadOnlyList<ScheduleDto>> GetScheduleByUserAsync(GetScheduleByUserQuery query);
    Task<ScheduleDto> AddTimestampToScheduleAsync(AddTimestampCommand command);
    Task<DeleteTimestampResponse> DeleteTimespanFromScheduleAsync(DeleteTimestampCommand command);
    Task<LessonReservationDto> AddReservationAsync(AddReservationCommand command);
    Task DeleteReservationAsync(Guid reservationId);
    Task<List<LessonReservationDto>> GetReservationsByTimespanIdAsync(Guid scheduleId, Guid timespanId);
    Task<List<UserPrivateLessonReservationsDto>> GetReservationsByUserAsync(int days);
    Task<List<UserPrivateLessonReservationsDto>> GetUserLessonReservationsAsync(int days);
}
