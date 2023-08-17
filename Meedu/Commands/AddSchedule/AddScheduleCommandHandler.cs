using MediatR;
using Meedu.Models.Schedule;
using Meedu.Services;

namespace Meedu.Commands.AddSchedule;

public sealed class AddScheduleCommandHandler : IRequestHandler<AddScheduleCommand, ScheduleDto>
{
    private readonly IScheduleService _scheduleService;

    public AddScheduleCommandHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<ScheduleDto> Handle(AddScheduleCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleService.AddScheduleAsync(request);
    }
}
