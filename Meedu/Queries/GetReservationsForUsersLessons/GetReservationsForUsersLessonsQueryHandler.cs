using MediatR;
using Meedu.Models.Reservations.UserReservations;
using Meedu.Services;

namespace Meedu.Queries.GetReservationsForUsersLessons;

public sealed class GetReservationsForUsersLessonsQueryHandler 
    : IRequestHandler<GetReservationsForUsersLessonsQuery, IReadOnlyList<UserPrivateLessonReservationsDto>>
{
    private readonly IScheduleService _scheduleService;

    public GetReservationsForUsersLessonsQueryHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<IReadOnlyList<UserPrivateLessonReservationsDto>> Handle(
        GetReservationsForUsersLessonsQuery request, CancellationToken cancellationToken)
    {
        return await _scheduleService.GetReservationsForUsersLessonsAsync(request);
    }
}
