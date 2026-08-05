using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Tracking;

namespace WatchLog.Api.Controllers;

[Authorize]
[Route("api/v1/tracking/movies")]
public class MovieTrackingController(IMovieTrackingService tracking, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost]
    public async Task<IActionResult> Mark(MarkMovieRequest request, CancellationToken ct)
    {
        await tracking.MarkMovieAsync(CurrentUserId, request, ct);
        return NoContent();
    }

    [HttpPost("favorite")]
    public async Task<IActionResult> ToggleFavorite(ToggleMovieFavoriteRequest request, CancellationToken ct)
    {
        await tracking.ToggleFavoriteAsync(CurrentUserId, request, ct);
        return NoContent();
    }
}
