using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Schedule;

public class ScheduleDto
{
    public string Id { get; set; }
    [Required]
    public Entities.Enums.DayOfWeek DayOfWeek { get; set; }
    public List<ScheduleTimespanDto> ScheduleTimespans { get; set; } = null!;
}
