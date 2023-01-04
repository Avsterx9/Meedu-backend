using Meedu.Entities.Enums;

namespace Meedu.Models
{
    public class UserLessonReservationsDto
    {
        public DateTime ReservationDate { get; set; }
        public List<ReservationDataDto> DayReservations { get; set; }  
    }
}
