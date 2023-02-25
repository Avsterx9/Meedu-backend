using System.ComponentModel.DataAnnotations.Schema;

namespace Meedu.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string PasswordHash { get; set; }
        public string PhoneNumber { get; set; }

        public int RoleId { get; set; }
        public virtual Role Role { get; set; }
        public virtual Image? Image { get; set; }
    }
}
