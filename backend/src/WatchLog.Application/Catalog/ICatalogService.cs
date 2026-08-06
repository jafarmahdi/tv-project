using WatchLog.Application.Common.Models;

namespace WatchLog.Application.Catalog;

/// <summary>
/// Search/discover/detail access to movies &amp; series. Backed by TMDB with a Postgres+Redis
/// cache-aside layer, and responsible for materializing a local `Guid` identity for any
/// movie/series/episode the moment another feature (lists, tracking, ratings) needs to reference it.
/// </summary>
public interface ICatalogService
{
    Task<PagedResult<MovieSummaryDto>> SearchMoviesAsync(string query, int page, CancellationToken ct = default);
    Task<PagedResult<SeriesSummaryDto>> SearchSeriesAsync(string query, int page, CancellationToken ct = default);
    Task<PagedResult<MovieSummaryDto>> GetTrendingMoviesAsync(int page, CancellationToken ct = default);
    Task<PagedResult<SeriesSummaryDto>> GetTrendingSeriesAsync(int page, CancellationToken ct = default);
    Task<PagedResult<MovieSummaryDto>> DiscoverMoviesByYearAsync(int year, int page, CancellationToken ct = default);
    Task<PagedResult<SeriesSummaryDto>> DiscoverSeriesByYearAsync(int year, int page, CancellationToken ct = default);
    Task<MovieDetailDto> GetMovieDetailAsync(int tmdbId, CancellationToken ct = default);
    Task<SeriesDetailDto> GetSeriesDetailAsync(int tmdbId, CancellationToken ct = default);
    Task<SeasonDetailDto> GetSeasonAsync(int seriesTmdbId, int seasonNumber, CancellationToken ct = default);
    Task<PagedResult<MovieSummaryDto>> GetSimilarMoviesAsync(int tmdbId, CancellationToken ct = default);
    Task<PagedResult<SeriesSummaryDto>> GetSimilarSeriesAsync(int tmdbId, CancellationToken ct = default);
    Task<IReadOnlyList<WatchProviderDto>> GetMovieWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default);
    Task<IReadOnlyList<WatchProviderDto>> GetSeriesWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default);

    /// <summary>Ensures a local `Movie` row exists for this TMDB id (fetching/upserting if needed) and returns its local id.</summary>
    Task<Guid> EnsureMovieCachedAsync(int tmdbId, CancellationToken ct = default);

    /// <summary>Ensures a local `Series` row exists for this TMDB id and returns its local id.</summary>
    Task<Guid> EnsureSeriesCachedAsync(int tmdbId, CancellationToken ct = default);

    /// <summary>Ensures local `Series`/`Season`/`Episode` rows exist for this TMDB triple and returns the episode's local id.</summary>
    Task<Guid> EnsureEpisodeCachedAsync(int seriesTmdbId, int seasonNumber, int episodeNumber, CancellationToken ct = default);
}
