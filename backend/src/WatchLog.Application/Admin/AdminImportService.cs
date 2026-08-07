using WatchLog.Application.Catalog;

namespace WatchLog.Application.Admin;

public class AdminImportService(ICatalogService catalog) : IAdminImportService
{
    public async Task<ImportedCatalogItemDto> ImportMovieByTmdbIdAsync(int tmdbId, CancellationToken ct = default)
    {
        var movie = await catalog.GetMovieDetailAsync(tmdbId, ct);
        return new ImportedCatalogItemDto(movie.Id, "Movie", movie.Title, $"TMDB movie {movie.TmdbId}");
    }

    public async Task<ImportedCatalogItemDto> ImportSeriesByTmdbIdAsync(int tmdbId, CancellationToken ct = default)
    {
        var series = await catalog.GetSeriesDetailAsync(tmdbId, ct);
        return new ImportedCatalogItemDto(series.Id, "Series", series.Title, $"TMDB series {series.TmdbId}");
    }

    public async Task<ImportedCatalogItemDto> ImportEpisodeAsync(int seriesTmdbId, int seasonNumber, int episodeNumber, CancellationToken ct = default)
    {
        var season = await catalog.GetSeasonAsync(seriesTmdbId, seasonNumber, ct);
        var episode = season.Episodes.FirstOrDefault(e => e.EpisodeNumber == episodeNumber)
            ?? throw new InvalidOperationException($"Episode S{seasonNumber}E{episodeNumber} was not found in TMDB.");

        return new ImportedCatalogItemDto(
            episode.Id,
            "Episode",
            episode.Title,
            $"Series {seriesTmdbId} • S{seasonNumber}E{episodeNumber}");
    }

    public Task<ImportResultDto> ImportMoviesByYearAsync(int year, int pages, CancellationToken ct = default) =>
        ImportAsync(year, pages,
            (p, c) => catalog.DiscoverMoviesByYearAsync(year, p, c),
            m => (m.TmdbId, m.Title),
            catalog.EnsureMovieCachedAsync, ct);

    public Task<ImportResultDto> ImportSeriesByYearAsync(int year, int pages, CancellationToken ct = default) =>
        ImportAsync(year, pages,
            (p, c) => catalog.DiscoverSeriesByYearAsync(year, p, c),
            s => (s.TmdbId, s.Title),
            catalog.EnsureSeriesCachedAsync, ct);

    /// <summary>
    /// Walks `pages` pages of a discover feed and, for each item found, ensures the local
    /// (Postgres) row is fully cached — pulling rich detail data, not just the summary — via
    /// `ensureCachedAsync`. Stops early if TMDB returns an empty page. One item's failure
    /// (e.g. a transient TMDB error) is recorded and skipped rather than aborting the whole run.
    /// </summary>
    private static async Task<ImportResultDto> ImportAsync<TSummary>(
        int year, int pages,
        Func<int, CancellationToken, Task<Application.Common.Models.PagedResult<TSummary>>> discoverPage,
        Func<TSummary, (int TmdbId, string Title)> identify,
        Func<int, CancellationToken, Task<Guid>> ensureCachedAsync,
        CancellationToken ct)
    {
        var discovered = 0;
        var imported = 0;
        var errors = new List<string>();

        for (var page = 1; page <= pages; page++)
        {
            var result = await discoverPage(page, ct);
            if (result.Items.Count == 0) break;

            foreach (var item in result.Items)
            {
                var (tmdbId, title) = identify(item);
                discovered++;
                try
                {
                    await ensureCachedAsync(tmdbId, ct);
                    imported++;
                }
                catch (Exception ex)
                {
                    errors.Add($"{tmdbId} ({title}): {ex.Message}");
                }
            }

            if (page >= result.TotalPages) break;
        }

        return new ImportResultDto(year, pages, discovered, imported, errors);
    }
}
