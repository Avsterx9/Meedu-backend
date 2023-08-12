namespace Meedu.Models.Auth;

public class UserInfoDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public int RoleId { get; set; } = 4;
    public ImageDto ImageDto { get; set; }
}
