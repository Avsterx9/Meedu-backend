using MediatR;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Services;

namespace Meedu.Queries.GetTodaysLessons;

public sealed class GetTodaysLessonsQueryHandler 
    : IRequestHandler<GetTodaysLessonsQuery, IReadOnlyList<UserReservationDataDto>>
{
    private readonly IDashboardService _dashboardService;

    public GetTodaysLessonsQueryHandler(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IReadOnlyList<UserReservationDataDto>> Handle(
        GetTodaysLessonsQuery request, CancellationToken cancellationToken)
    {
        return await _dashboardService.GetTodaysUserLessonsAsync();
    }
}
