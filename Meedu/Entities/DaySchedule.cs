﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Meedu.Entities;

public class DaySchedule
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateTime Created { get; set; }
    public Guid UserId { get; set; }

    public virtual User User { get; set; } = null!;
    public virtual List<ScheduleTimestamp> ScheduleTimestamps { get; set; } = null!;
}
