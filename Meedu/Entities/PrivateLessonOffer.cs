using Meedu.Entities.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Meedu.Entities;

public class PrivateLessonOffer
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsRemote { get; set; }
    public Place Place { get; set; } = Place.NotSpecified;
    public string Description { get; set; } = string.Empty;
    public TeachingRange TeachingRange { get; set; } = TeachingRange.Other;
    public Guid CreatedById { get; set; }
    public Guid SubjectId { get; set; }

    public virtual User CreatedBy { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
}
