using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.GetReservationsByTimestamp;

public sealed class GetReservationsByTimestampQueryHandler 
    : IRequestHandler<GetReservationsByTimestampQuery, IReadOnlyList<LessonReservationDto>>
{
    private readonly IScheduleService _scheduleService;

    public GetReservationsByTimestampQueryHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<IReadOnlyList<LessonReservationDto>> Handle(GetReservationsByTimestampQuery request,
        CancellationToken cancellationToken)
    {
        return await _scheduleService.GetReservationsByTimestampIdAsync(request);
    }
}
