using Meedu.Entities.Enums;

namespace Meedu.Models.Reservations.UserReservations;

public class UserReservationDataDto
{
    public Guid ScheduleId { get; set; }
    public Guid ReservationId { get; set; }
    public Guid TimespanId { get; set; }
    public Guid LessonId { get; set; }

    public string LessonTitle { get; set; } = string.Empty;
    public string AvailableFrom { get; set; } = string.Empty;
    public string AvailableTo { get; set; } = string.Empty;
    public bool isOnline { get; set; }
    public Place Place { get; set; }
    public DtoNameLastnameId User { get; set; } = null!;
}
