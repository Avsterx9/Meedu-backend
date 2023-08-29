using MediatR;
using Meedu.Models;
using Meedu.Services;

namespace Meedu.Queries.GetUserStudents;

public class GetUserStudentsQueryHandler 
    : IRequestHandler<GetUserStudentsQuery, IReadOnlyList<DtoNameLastnameId>>
{
    private readonly IDashboardService _dashboardService;

    public GetUserStudentsQueryHandler(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IReadOnlyList<DtoNameLastnameId>> Handle(GetUserStudentsQuery request,
        CancellationToken cancellationToken)
    {
        return await _dashboardService.GetUserStudentsAsync(request);
    }
}
