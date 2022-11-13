namespace Meedu.Models
{
    public class LessonReservationDto
    {        
        public DtoNameId ReservedBy { get; set; }
        public DateTime ReservationDate { get; set; }
    }
}