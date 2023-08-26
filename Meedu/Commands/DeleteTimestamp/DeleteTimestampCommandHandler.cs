using MediatR;
using Meedu.Models.Response;
using Meedu.Services;

namespace Meedu.Commands.DeleteTimestamp;

public sealed class DeleteTimestampCommandHandler : IRequestHandler<DeleteTimestampCommand, DeleteTimestampResponse>
{
    private readonly IScheduleService _scheduleService;

    public DeleteTimestampCommandHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<DeleteTimestampResponse> Handle(DeleteTimestampCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleService.DeleteTimespanFromScheduleAsync(request);
    }
}
