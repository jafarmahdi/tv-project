using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Stats;

namespace WatchLog.Api.Controllers;

[Authorize]
public class StatsController(IStatsService statsService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpGet("me")]
    public async Task<ActionResult<UserStatsDto>> GetMyStats(CancellationToken ct) =>
        Ok(await statsService.GetUserStatsAsync(CurrentUserId, ct));
}
