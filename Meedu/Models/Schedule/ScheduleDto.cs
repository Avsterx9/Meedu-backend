using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Schedule;

public class ScheduleDto
{
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public List<ScheduleTimespanDto> ScheduleTimestamps { get; set; } = null!;
}
