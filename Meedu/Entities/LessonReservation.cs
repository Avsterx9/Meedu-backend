namespace Meedu.Entities;

public class LessonReservation
{
    public Guid Id { get; set; }
    public Guid ReservedById { get; set; }
    public DateTime ReservationDate { get; set; }
    public Guid ScheduleTimespanId { get; set; }
    public Guid PrivateLessonOfferId { get; set; }

    public virtual ScheduleTimespan ScheduleTimespan { get; set; } = null!;
    public virtual User ReservedBy { get; set; } = null!;
    public virtual PrivateLessonOffer PrivateLessonOffer { get; set; } = null!;
}
