using Meedu.Entities.Enums;

namespace Meedu.Models.Reservations.UserReservations
{
    public class UserPrivateLessonReservationsDto
    {
        public DateTime ReservationDate { get; set; }
        public List<UserReservationDataDto> DayReservations { get; set; }
        public int Day { get; set; }
    }
}
