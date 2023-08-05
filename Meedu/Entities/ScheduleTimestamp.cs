namespace Meedu.Entities;

public class ScheduleTimespan
{
    public Guid Id { get; set; }
    public DateTime AvailableFrom { get; set; }
    public DateTime AvailableTo { get; set; }
    public Guid DayScheduleId { get; set; }

    public virtual List<LessonReservation> LessonReservations { get; set; } = null!;
    public virtual DaySchedule DaySchedule { get; set; } = null!;
}