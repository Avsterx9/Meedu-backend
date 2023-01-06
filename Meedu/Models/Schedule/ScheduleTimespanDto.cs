using Meedu.Models.PrivateLessonOffer;
using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Schedule
{
    public class ScheduleTimespanDto
    {
        public string? Id { get; set; }
        [Required]
        public string AvailableFrom { get; set; }
        [Required]
        public string AvailableTo { get; set; }
        public List<LessonReservationDto>? LessonReservations { get; set; }
    }
}
