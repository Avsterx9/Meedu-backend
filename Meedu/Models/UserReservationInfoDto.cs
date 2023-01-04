using Meedu.Entities.Enums;

namespace Meedu.Models
{
    public class UserReservationInfoDto
    {
        public int ScheduleId { get; set; }
        public int ReservationId { get; set; }
        public int TimespanId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string LessonTitle { get; set; }
        public string StartTime { get; set; }
        public bool isOnline { get; set; }
        public Place Place { get; set; }
    }
}
