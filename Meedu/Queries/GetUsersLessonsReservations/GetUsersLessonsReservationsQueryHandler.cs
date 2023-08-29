using MediatR;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Services;

namespace Meedu.Queries.GetUsersLessonsReservations;

public sealed class GetUsersLessonsReservationsQueryHandler 
    : IRequestHandler<GetUsersLessonsReservationsQuery, IReadOnlyList<UserReservationDataDto>>
{
    private readonly IDashboardService _dashboardService;

    public GetUsersLessonsReservationsQueryHandler(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IReadOnlyList<UserReservationDataDto>> Handle(
        GetUsersLessonsReservationsQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardService.GetUsersLessonsReservationsAsync();
    }
}
