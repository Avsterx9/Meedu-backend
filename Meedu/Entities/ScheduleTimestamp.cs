namespace Meedu.Entities;

public class ScheduleTimestamp
{
    public Guid Id { get; set; }
    public string AvailableFrom { get; set; }
    public string AvailableTo { get; set; }
    public Guid DayScheduleId { get; set; }

    public virtual List<LessonReservation> LessonReservations { get; set; } = null!;
    public virtual DaySchedule DaySchedule { get; set; } = null!;
}