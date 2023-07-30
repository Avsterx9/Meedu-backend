using System.ComponentModel.DataAnnotations.Schema;

namespace Meedu.Entities;

public class Subject
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }
    public string Name { get; set; }
}
