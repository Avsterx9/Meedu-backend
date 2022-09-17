using Meedu.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meedu.Entities
{
    public class PrivateLessonOffer
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string LessonTitle { get; set; }
        public string City { get; set; }
        public decimal Price { get; set; }
        public bool OnlineLessonsPossible { get; set; }
        public Place Place { get; set; }
        public virtual User CreatedBy { get; set; }
        public virtual Subject Subject { get; set; }
        public string? Description { get; set; }
        public TeachingRange? TeachingRange { get; set; }
    }
}
