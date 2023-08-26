using MediatR;
using Meedu.Models.Response;

namespace Meedu.Commands.DeleteTimestamp;

public record DeleteTimestampCommand(Guid TimestampId)
    : IRequest<DeleteTimestampResponse>;
