namespace Meedu.Models
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int RoleId { get; set; } = 4;
    }
}
