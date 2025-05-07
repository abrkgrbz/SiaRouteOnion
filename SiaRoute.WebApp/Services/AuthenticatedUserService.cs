using Application.Interfaces;
using System.Security.Claims;

namespace SiaRoute.WebApp.Services
{
    public class AuthenticatedUserService : IAuthenticatedUserService
    {
        public AuthenticatedUserService(IHttpContextAccessor httpContextAccessor)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
            UserRoles = httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role)
                .Select(role => role.Value)
                .ToList();
        }

        public string UserId { get; }
        public List<string> UserRoles { get; }
    }
}
