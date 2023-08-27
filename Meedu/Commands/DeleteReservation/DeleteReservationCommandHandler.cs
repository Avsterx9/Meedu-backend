using MediatR;
using Meedu.Models.Response;
using Meedu.Services;

namespace Meedu.Commands.DeleteReservation;

public sealed class DeleteReservationCommandHandler 
    : IRequestHandler<DeleteReservationCommand, DeleteReservationResponse>
{
    private readonly IScheduleService _scheduleService;

    public DeleteReservationCommandHandler(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    public async Task<DeleteReservationResponse> Handle(DeleteReservationCommand request,
        CancellationToken cancellationToken)
    {
        return await _scheduleService.DeleteReservationAsync(request);
    }
}
