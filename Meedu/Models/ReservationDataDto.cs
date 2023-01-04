using Meedu.Entities.Enums;

namespace Meedu.Models
{
    public class ReservationDataDto
    {
        public string ScheduleId { get; set; }
        public string ReservationId { get; set; }
        public string TimespanId { get; set; }

        public string LessonTitle { get; set; }
        public string AvailableFrom { get; set; }
        public string AvailableTo { get; set; }
        public bool isOnline { get; set; }
        public Place Place { get; set; }
    }
}
