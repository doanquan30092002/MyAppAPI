using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace MyApp.Application.Common.CurrentUserService
{
    public interface ICurrentUserService
    {
        string? GetUserId();
        string? GetRole();
    }

    public class CurrentUserService : ICurrentUserService
    {
        public readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string? GetUserId()
        {
            return _httpContextAccessor
                .HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)
                ?.Value;
        }

        public string? GetRole()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;
        }
    }
}
