using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase(ICurrentUserService currentUser) : ControllerBase
{
    /// <summary>The authenticated caller's id. Only valid on actions guarded by `[Authorize]`.</summary>
    protected Guid CurrentUserId => currentUser.UserId
        ?? throw new UnauthorizedAccessException("No authenticated user on this request.");
}
