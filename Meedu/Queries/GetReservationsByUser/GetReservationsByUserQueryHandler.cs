using MediatR;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Services;

namespace Meedu.Queries.GetReservationsByUser;

public sealed class GetReservationsByUserQueryHandler
    : IRequestHandler<GetReservationsByUserQuery, IReadOnlyList<UserPrivateLessonReservationsDto>>
{
    private readonly IScheduleService _scheduleService;

    public GetReservationsByUserQueryHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<IReadOnlyList<UserPrivateLessonReservationsDto>> Handle(
        GetReservationsByUserQuery request, CancellationToken cancellationToken)
    {
        return await _scheduleService.GetReservationsByUserAsync(request);
    }
}
