using Meedu.Models.PrivateLessonOffer;
using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Schedule;

public class ScheduleTimespanDto
{
    public Guid Id { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableTo { get; set; }
    public List<LessonReservationDto> LessonReservations { get; set; } = null!;
}
