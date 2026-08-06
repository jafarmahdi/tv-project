using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

[AllowAnonymous]
public class MoviesController(ICatalogService catalog, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpGet("search")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<MovieSummaryDto>>> Search(
        [FromQuery] string query, [FromQuery] int page = 1, CancellationToken ct = default) =>
        Ok(await catalog.SearchMoviesAsync(query, page, ct));

    [HttpGet("trending")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<MovieSummaryDto>>> Trending(
        [FromQuery] int page = 1, CancellationToken ct = default) =>
        Ok(await catalog.GetTrendingMoviesAsync(page, ct));

    /// <summary>Movies released in <paramref name="year"/> (defaults to the current year), sorted by popularity.</summary>
    [HttpGet("discover")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<MovieSummaryDto>>> Discover(
        [FromQuery] int? year = null, [FromQuery] int page = 1, CancellationToken ct = default) =>
        Ok(await catalog.DiscoverMoviesByYearAsync(year ?? DateTime.UtcNow.Year, page, ct));

    [HttpGet("{tmdbId:int}")]
    public async Task<ActionResult<MovieDetailDto>> GetDetail(int tmdbId, CancellationToken ct) =>
        Ok(await catalog.GetMovieDetailAsync(tmdbId, ct));

    [HttpGet("{tmdbId:int}/similar")]
    public async Task<ActionResult<Application.Common.Models.PagedResult<MovieSummaryDto>>> Similar(int tmdbId, CancellationToken ct) =>
        Ok(await catalog.GetSimilarMoviesAsync(tmdbId, ct));

    [HttpGet("{tmdbId:int}/watch-providers")]
    public async Task<ActionResult<IReadOnlyList<WatchProviderDto>>> WatchProviders(
        int tmdbId, [FromQuery] string region = "US", CancellationToken ct = default) =>
        Ok(await catalog.GetMovieWatchProvidersAsync(tmdbId, region, ct));
}
