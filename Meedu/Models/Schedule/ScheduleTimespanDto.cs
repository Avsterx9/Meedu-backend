using Meedu.Models.PrivateLessonOffer;
using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Schedule;

public class ScheduleTimespanDto
{
    public Guid Id { get; set; }
    public string AvailableFrom { get; set; } = string.Empty;
    public string AvailableTo { get; set; } = string.Empty;
    public List<LessonReservationDto> LessonReservations { get; set; } = null!;
}
