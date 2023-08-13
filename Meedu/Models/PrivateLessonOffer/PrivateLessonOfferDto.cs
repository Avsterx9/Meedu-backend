using Meedu.Entities.Enums;

namespace Meedu.Models.PrivateLessonOffer;

public class PrivateLessonOfferDto
{
    public string Id { get; set; }
    public string LessonTitle { get; set; }
    public string City { get; set; }
    public decimal Price { get; set; }
    public bool isOnline { get; set; }
    public Place Place { get; set; }
    public DtoNameId Subject { get; set; }
    public string? Description { get; set; }
    public TeachingRange TeachingRange { get; set; }
    public DtoNameLastnameId? User { get; set; }
}
