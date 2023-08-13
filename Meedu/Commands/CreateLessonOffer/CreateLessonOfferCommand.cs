using MediatR;
using Meedu.Entities.Enums;
using Meedu.Models;
using Meedu.Models.PrivateLessonOffer;

namespace Meedu.Commands.CreateLessonOffer;

public record CreateLessonOfferCommand(
    string LessonTitle,
    string City,
    decimal Price,
    bool isOnline,
    Place Place,
    DtoNameId Subject,
    string Description,
    TeachingRange TeachingRange,
    Guid UserId
    ) : IRequest<PrivateLessonOfferDto>;
