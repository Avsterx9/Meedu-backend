namespace Meedu.Models.Reservations.StudentReservations
{
    public class StudentReservationsDto
    {
        public DateTime ReservationDate { get; set; }
        public List<StudentDayReservationsDto> DayReservations { get; set; }
        public int Day { get; set; }
    }
}
