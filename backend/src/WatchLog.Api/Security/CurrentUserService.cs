using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Security;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public bool IsAuthenticated => httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public Guid? UserId
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(sub, out var id) ? id : null;
        }
    }
}
