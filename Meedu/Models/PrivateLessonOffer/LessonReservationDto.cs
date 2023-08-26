namespace Meedu.Models.PrivateLessonOffer;

public class LessonReservationDto
{
    public Guid Id { get; set; }
    public DtoNameId ReservedBy { get; set; } = null!;
    public DateTime ReservationDate { get; set; }
}