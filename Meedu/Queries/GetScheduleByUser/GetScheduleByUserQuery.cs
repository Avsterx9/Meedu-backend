using MediatR;
using Meedu.Models.Schedule;

namespace Meedu.Queries.GetScheduleByUser;

public record GetScheduleByUserQuery(Guid userId)
    : IRequest<IReadOnlyList<ScheduleDto>>;
