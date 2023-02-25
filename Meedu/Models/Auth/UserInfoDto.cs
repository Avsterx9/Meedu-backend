namespace Meedu.Models.Auth
{
    public class UserInfoDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int RoleId { get; set; } = 4;
        public ImageDto ImageDto { get; set; }
    }
}
