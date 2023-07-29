using Meedu.Exceptions;
using System.Security.Claims;

namespace Meedu.Services
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        Guid? GetUserId { get; }
        Guid GetUserIdFromToken();
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
        public Guid? GetUserId =>
            User is null ? null : (Guid?)new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);

        public Guid GetUserIdFromToken()
        {
            if (User == null)
                throw new BadRequestException("Missing user info");

            return new Guid(User.FindFirst(ClaimTypes.Name).Value);
        }
    }
}
