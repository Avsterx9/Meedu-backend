using Meedu.Entities.Enums;
using Meedu.Models.Auth;

namespace Meedu.Models.PrivateLessonOffer
{
    public class LessonOfferAdvancedSearchDto
    {
        public string? Subject { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? IsOnline { get; set; }
        public string? City { get; set; }
        public TeachingRange? TeachingRange { get; set; }
        public decimal? PriceFrom { get; set; }
        public decimal? PriceTo { get; set; }
    }
}
