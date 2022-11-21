namespace Meedu.Models
{
    public class LessonReservationDto
    {
        public string? Id { get; set; }
        public DtoNameId ReservedBy { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}