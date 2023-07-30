namespace Meedu.Entities;

public class LessonReservation
{
    public Guid Id { get; set; }
    public virtual User ReservedBy { get; set; }
    public DateTime ReservationDate { get; set; }
    public virtual ScheduleTimespan ScheduleTimespan { get; set; }
}
