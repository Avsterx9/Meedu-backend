using System.ComponentModel.DataAnnotations;

namespace Meedu.Models
{
    public class ScheduleDto
    {
        public string Id { get; set; }
        [Required]
        public Entities.Enums.DayOfWeek DayOfWeek { get; set; }
        public List<ScheduleTimespanDto> ScheduleTimespans { get; set; }
        [Required]
        public SubjectDto Subject { get; set; }
        [Required]
        public String lessonOfferId { get; set; }
    }
}
