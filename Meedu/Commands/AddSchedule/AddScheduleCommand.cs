using MediatR;
using Meedu.Models.Schedule;

namespace Meedu.Commands.AddSchedule;

public class AddScheduleCommand : IRequest<ScheduleDto>
{
    public DayOfWeek DayOfWeek { get; set; }
}
