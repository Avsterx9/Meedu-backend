using MediatR;
using Meedu.Models.Schedule;
using Meedu.Services;

namespace Meedu.Commands.AddTimestamp;

public sealed class AddTimestampCommandHandler : IRequestHandler<AddTimestampCommand, ScheduleDto>
{
    private readonly IScheduleService _scheduleService;

    public AddTimestampCommandHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<ScheduleDto> Handle(AddTimestampCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleService.AddTimestampToScheduleAsync(request);
    }
}
