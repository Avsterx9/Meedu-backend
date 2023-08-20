using MediatR;
using Meedu.Models.Schedule;

namespace Meedu.Commands.AddTimestamp;

public record AddTimestampCommand(
    Guid ScheduleId,
    string AvailableFrom,
    string AvailableTo
    ) : IRequest<ScheduleDto>;