using MediatR;
using Meedu.Models.PrivateLessonOffer;
using Meedu.Services;

namespace Meedu.Commands.AddReservation;

public sealed class AddReservationCommandHandler : IRequestHandler<AddReservationCommand, LessonReservationDto>
{
    private readonly IScheduleService _scheduleService;

    public AddReservationCommandHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<LessonReservationDto> Handle(AddReservationCommand request, CancellationToken cancellationToken)
    {
        return await _scheduleService.AddReservationAsync(request);
    }
}
