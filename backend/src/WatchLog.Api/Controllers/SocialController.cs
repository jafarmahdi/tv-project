using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Social;
using WatchLog.Domain.Enums;

namespace WatchLog.Api.Controllers;

[Authorize]
public class SocialController(ISocialService socialService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost("follow/{targetUserId:guid}")]
    public async Task<IActionResult> Follow(Guid targetUserId, CancellationToken ct)
    {
        await socialService.FollowAsync(CurrentUserId, targetUserId, ct);
        return NoContent();
    }

    [HttpDelete("follow/{targetUserId:guid}")]
    public async Task<IActionResult> Unfollow(Guid targetUserId, CancellationToken ct)
    {
        await socialService.UnfollowAsync(CurrentUserId, targetUserId, ct);
        return NoContent();
    }

    [HttpGet("followers")]
    public async Task<ActionResult<IReadOnlyList<FollowSummaryDto>>> GetFollowers(CancellationToken ct) =>
        Ok(await socialService.GetFollowersAsync(CurrentUserId, ct));

    [HttpGet("following")]
    public async Task<ActionResult<IReadOnlyList<FollowSummaryDto>>> GetFollowing(CancellationToken ct) =>
        Ok(await socialService.GetFollowingAsync(CurrentUserId, ct));

    [HttpGet("feed")]
    public async Task<ActionResult<IReadOnlyList<ActivityFeedItemDto>>> GetFeed(
        [FromQuery] int page = 1, [FromQuery] int pageSize = 25, CancellationToken ct = default) =>
        Ok(await socialService.GetFeedAsync(CurrentUserId, page, pageSize, ct));

    [HttpPost("comments")]
    public async Task<ActionResult<CommentDto>> AddComment(AddCommentRequest request, CancellationToken ct) =>
        Ok(await socialService.AddCommentAsync(CurrentUserId, request, ct));

    [AllowAnonymous]
    [HttpGet("comments/{targetType}/{targetId:guid}")]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetComments(TargetType targetType, Guid targetId, CancellationToken ct) =>
        Ok(await socialService.GetCommentsAsync(targetType, targetId, ct));

    [HttpPost("likes")]
    public async Task<IActionResult> ToggleLike(ToggleLikeRequest request, CancellationToken ct)
    {
        await socialService.ToggleLikeAsync(CurrentUserId, request, ct);
        return NoContent();
    }
}
