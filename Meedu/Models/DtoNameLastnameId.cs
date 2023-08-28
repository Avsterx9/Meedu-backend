using Meedu.Models.Auth;

namespace Meedu.Models;

public class DtoNameLastnameId
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public ImageDto? ImageDto { get; set; }
}
