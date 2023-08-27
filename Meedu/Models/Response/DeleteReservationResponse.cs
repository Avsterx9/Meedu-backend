namespace Meedu.Models.Response;

public record DeleteReservationResponse(bool Success, string Message)
    : BaseResponse(Success, Message);
