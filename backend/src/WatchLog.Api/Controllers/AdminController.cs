using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WatchLog.Application.Admin;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Api.Controllers;

/// <summary>Admin-only operational endpoints. Requires the "Admin" role — see
/// <c>WatchLog.Infrastructure.Identity.IdentitySeeder</c> for how an account gets that role.</summary>
[Authorize(Roles = "Admin")]
public class AdminController(IAdminImportService importService, ICurrentUserService currentUser) : ApiControllerBase(currentUser)
{
    [HttpPost("import/movie/{tmdbId:int}")]
    public async Task<ActionResult<ImportedCatalogItemDto>> ImportMovieByTmdbId(int tmdbId, CancellationToken ct = default) =>
        Ok(await importService.ImportMovieByTmdbIdAsync(tmdbId, ct));

    [HttpPost("import/series/{tmdbId:int}")]
    public async Task<ActionResult<ImportedCatalogItemDto>> ImportSeriesByTmdbId(int tmdbId, CancellationToken ct = default) =>
        Ok(await importService.ImportSeriesByTmdbIdAsync(tmdbId, ct));

    [HttpPost("import/episode")]
    public async Task<ActionResult<ImportedCatalogItemDto>> ImportEpisode(ImportEpisodeRequest request, CancellationToken ct = default) =>
        Ok(await importService.ImportEpisodeAsync(request.SeriesTmdbId, request.SeasonNumber, request.EpisodeNumber, ct));

    /// <summary>Pulls up to <paramref name="pages"/> pages (20 items/page) of <paramref name="year"/>'s
    /// most popular movies from TMDB into the local catalog. Slow by design (one detail fetch per
    /// movie) — keep <paramref name="pages"/> modest per call.</summary>
    [HttpPost("import/movies")]
    public async Task<ActionResult<ImportResultDto>> ImportMovies(
        [FromQuery] int? year = null, [FromQuery] int pages = 5, CancellationToken ct = default) =>
        Ok(await importService.ImportMoviesByYearAsync(year ?? DateTime.UtcNow.Year, Math.Clamp(pages, 1, 20), ct));

    /// <summary>Same as <see cref="ImportMovies"/> but for series (matched by first-air-date year).</summary>
    [HttpPost("import/series")]
    public async Task<ActionResult<ImportResultDto>> ImportSeries(
        [FromQuery] int? year = null, [FromQuery] int pages = 5, CancellationToken ct = default) =>
        Ok(await importService.ImportSeriesByYearAsync(year ?? DateTime.UtcNow.Year, Math.Clamp(pages, 1, 20), ct));
}
