using System.ComponentModel.DataAnnotations;

namespace Meedu.Models
{
    public class ScheduleTimespanDto
    {
        [Required]
        public string AvailableFrom { get; set; }
        [Required]
        public string AvailableTo { get; set; }
        public List<LessonReservationDto>? LessonReservations { get; set; }
    }
}
