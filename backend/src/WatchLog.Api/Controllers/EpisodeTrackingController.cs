using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Tracking;

namespace WatchLog.Api.Controllers;

[Authorize]
[Route("api/v1/tracking/episodes")]
public class EpisodeTrackingController(IEpisodeTrackingService tracking, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost]
    public async Task<IActionResult> Mark(MarkEpisodeRequest request, CancellationToken ct)
    {
        await tracking.MarkEpisodeAsync(CurrentUserId, request, ct);
        return NoContent();
    }

    [HttpPost("favorite")]
    public async Task<IActionResult> ToggleFavorite(ToggleEpisodeFavoriteRequest request, CancellationToken ct)
    {
        await tracking.ToggleFavoriteAsync(CurrentUserId, request, ct);
        return NoContent();
    }

    [HttpGet("series/{seriesTmdbId:int}/seasons/{seasonNumber:int}")]
    public async Task<ActionResult<SeasonProgressDto>> GetSeasonProgress(int seriesTmdbId, int seasonNumber, CancellationToken ct) =>
        Ok(await tracking.GetSeasonProgressAsync(CurrentUserId, seriesTmdbId, seasonNumber, ct));

    [HttpGet("series/{seriesTmdbId:int}/next")]
    public async Task<ActionResult<NextEpisodeDto?>> GetNextEpisode(int seriesTmdbId, CancellationToken ct) =>
        Ok(await tracking.GetNextEpisodeAsync(CurrentUserId, seriesTmdbId, ct));
}
