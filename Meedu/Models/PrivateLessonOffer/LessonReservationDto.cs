namespace Meedu.Models.PrivateLessonOffer
{
    public class LessonReservationDto
    {
        public string? Id { get; set; }
        public DtoNameId ReservedBy { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}