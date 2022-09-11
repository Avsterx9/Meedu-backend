using System.Security.Claims;

namespace Meedu.Services
{
    public interface IUserContextService
    {
        ClaimsPrincipal User { get; }
        Guid? GetUserId { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => httpContextAccessor.HttpContext?.User;
        public Guid? GetUserId =>
            User is null ? null : (Guid?)new Guid(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
