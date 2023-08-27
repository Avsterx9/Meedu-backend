using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Queries.GetReservationsByTimestamp;

public sealed class GetReservationsByTimespanQueryHandler 
    : IRequestHandler<GetReservationsByTimestampQuery, IReadOnlyList<LessonReservationDto>>
{
    private readonly IScheduleService _scheduleService;

    public GetReservationsByTimespanQueryHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<IReadOnlyList<LessonReservationDto>> Handle(GetReservationsByTimestampQuery request,
        CancellationToken cancellationToken)
    {
        return await _scheduleService.GetReservationsByTimespanIdAsync(request);
    }
}
