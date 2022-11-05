using Meedu.Entities;

namespace Meedu.Models
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
