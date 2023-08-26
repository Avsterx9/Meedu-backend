namespace Meedu.Models.Response;

public record DeleteTimestampResponse(bool Success, string Message) 
    : BaseResponse(Success, Message);