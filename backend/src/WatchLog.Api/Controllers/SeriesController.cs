using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

[AllowAnonymous]
[Route("api/v1/series")]
public class SeriesController(ICatalogService catalog, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpGet("search")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<SeriesSummaryDto>>> Search(
        [FromQuery] string query, [FromQuery] int page = 1, CancellationToken ct = default) =>
        Ok(await catalog.SearchSeriesAsync(query, page, ct));

    [HttpGet("trending")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<SeriesSummaryDto>>> Trending(
        [FromQuery] int page = 1, CancellationToken ct = default) =>
        Ok(await catalog.GetTrendingSeriesAsync(page, ct));

    [HttpGet("{tmdbId:int}")]
    public async Task<ActionResult<SeriesDetailDto>> GetDetail(int tmdbId, CancellationToken ct) =>
        Ok(await catalog.GetSeriesDetailAsync(tmdbId, ct));

    [HttpGet("{tmdbId:int}/seasons/{seasonNumber:int}")]
    public async Task<ActionResult<SeasonDetailDto>> GetSeason(int tmdbId, int seasonNumber, CancellationToken ct) =>
        Ok(await catalog.GetSeasonAsync(tmdbId, seasonNumber, ct));

    [HttpGet("{tmdbId:int}/similar")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<SeriesSummaryDto>>> Similar(int tmdbId, CancellationToken ct) =>
        Ok(await catalog.GetSimilarSeriesAsync(tmdbId, ct));

    [HttpGet("{tmdbId:int}/watch-providers")]
    public async Task<ActionResult<IReadOnlyList<WatchProviderDto>>> WatchProviders(
        int tmdbId, [FromQuery] string region = "US", CancellationToken ct = default) =>
        Ok(await catalog.GetSeriesWatchProvidersAsync(tmdbId, region, ct));
}
