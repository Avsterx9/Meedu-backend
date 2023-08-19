using MediatR;
using Meedu.Models.Schedule;

namespace Meedu.Commands.AddTimestamp;

public record AddTimestampCommand(
    Guid ScheduleId,
    DateTime AvailableFrom,
    DateTime AvailableTo
    ) : IRequest<ScheduleDto>;