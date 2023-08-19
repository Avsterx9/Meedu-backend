using MediatR;
using Meedu.Models.Response;
using Meedu.Models.Schedule;

namespace Meedu.Commands.DeleteSchedule;

public record DeleteScheduleCommand(
    Guid ScheduleId
    ) : IRequest<DeleteScheduleResponse>;