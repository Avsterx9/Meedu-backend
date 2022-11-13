using System.ComponentModel.DataAnnotations.Schema;

namespace Meedu.Entities
{
    public class DaySchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Entities.Enums.DayOfWeek DayOfWeek { get; set; }
        public User User { get; set; }
        public DateTime Created { get; set; }
        public virtual List<ScheduleTimespan> ScheduleTimestamps { get; set; }
        public virtual Subject Subject { get; set; }
    }
}
