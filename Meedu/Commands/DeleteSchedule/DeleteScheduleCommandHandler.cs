using MediatR;
using Meedu.Models.Response;
using Meedu.Services;

namespace Meedu.Commands.DeleteSchedule;

public sealed class DeleteScheduleCommandHandler : IRequestHandler<DeleteScheduleCommand, DeleteScheduleResponse>
{
    private readonly IScheduleService _scheduleService;

    public DeleteScheduleCommandHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<DeleteScheduleResponse> Handle(DeleteScheduleCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleService.DeleteScheduleAsync(request);
    }
}
