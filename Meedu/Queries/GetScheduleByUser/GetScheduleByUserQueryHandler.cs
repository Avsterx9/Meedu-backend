using MediatR;
using Meedu.Models.Schedule;
using Meedu.Services;

namespace Meedu.Queries.GetScheduleByUser;

public sealed class GetScheduleByUserQueryHandler 
    : IRequestHandler<GetScheduleByUserQuery, IReadOnlyList<ScheduleDto>>
{
    private readonly IScheduleService _scheduleService;

    public GetScheduleByUserQueryHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<IReadOnlyList<ScheduleDto>> Handle(
        GetScheduleByUserQuery request, CancellationToken cancellationToken)
    {
        return await _scheduleService.GetScheduleByUserAsync(request);
    }
}
