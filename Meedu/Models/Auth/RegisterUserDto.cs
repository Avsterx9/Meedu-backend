using System.ComponentModel.DataAnnotations;

namespace Meedu.Models.Auth
{
    public class RegisterUserDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string ConfirmPassword { get; set; }
        [Required]
        public DateTime? DateOfBirth { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; } = 4;
    }
}
