using MediatR;
using Meedu.Entities.Enums;
using Meedu.Models;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Commands.UpdateLessonOffer;

public record UpdateLessonOfferCommand(
    Guid Id,
    string LessonTitle,
    string City,
    decimal Price,
    bool IsRemote,
    Place Place,
    string Description,
    TeachingRange TeachingRange,
    int UserId,
    SubjectDto Subject
    ) : IRequest<PrivateLessonOfferDto>;