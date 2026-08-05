using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Ratings;
using WatchLog.Domain.Enums;

namespace WatchLog.Api.Controllers;

public class RatingsController(IRatingService ratingService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Rate(RateRequest request, CancellationToken ct)
    {
        await ratingService.RateAsync(CurrentUserId, request, ct);
        return NoContent();
    }

    [AllowAnonymous]
    [HttpGet("{targetType}/{targetId:guid}")]
    public async Task<ActionResult<RatingSummaryDto>> GetSummary(TargetType targetType, Guid targetId, CancellationToken ct) =>
        Ok(await ratingService.GetSummaryAsync(currentUser.UserId, targetType, targetId, ct));
}
