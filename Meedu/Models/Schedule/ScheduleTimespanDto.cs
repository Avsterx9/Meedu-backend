using Meedu.Models.PrivateLessonOffer;
using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Schedule;

public class ScheduleTimespanDto
{
    public string? Id { get; set; }
    [Required]
    public string AvailableFrom { get; set; } = string.Empty;
    [Required]
    public string AvailableTo { get; set; } = string.Empty;
    public List<LessonReservationDto>? LessonReservations { get; set; }
}
