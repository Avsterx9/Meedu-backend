using System.ComponentModel.DataAnnotations;

namespace Meedu.Models
{
    public class ScheduleDto
    {
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
    }
}
