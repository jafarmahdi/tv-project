namespace WatchLog.Application.Common.Interfaces;

/// <summary>
/// Client for the TMDB (The Movie Database) API — movies, series, seasons, episodes, cast/crew,
/// images, trailers, watch providers. Implemented in Infrastructure with an `HttpClient` and a
/// Redis cache-aside layer. Requires `Tmdb:ApiKey` to be configured; throws
/// <see cref="TmdbNotConfiguredException"/> otherwise so callers get a clear error instead of fake data.
/// </summary>
public interface ITmdbClient
{
    Task<TmdbPagedResult<TmdbMovieSummary>> SearchMoviesAsync(string query, int page, CancellationToken ct = default);
    Task<TmdbPagedResult<TmdbSeriesSummary>> SearchSeriesAsync(string query, int page, CancellationToken ct = default);
    Task<TmdbPagedResult<TmdbMovieSummary>> GetTrendingMoviesAsync(int page, CancellationToken ct = default);
    Task<TmdbPagedResult<TmdbSeriesSummary>> GetTrendingSeriesAsync(int page, CancellationToken ct = default);

    /// <summary>Movies whose primary release date falls in <paramref name="year"/>, sorted by popularity.</summary>
    Task<TmdbPagedResult<TmdbMovieSummary>> DiscoverMoviesByYearAsync(int year, int page, CancellationToken ct = default);

    /// <summary>Series whose first-air-date falls in <paramref name="year"/>, sorted by popularity.</summary>
    Task<TmdbPagedResult<TmdbSeriesSummary>> DiscoverSeriesByYearAsync(int year, int page, CancellationToken ct = default);
    Task<TmdbMovieDetail?> GetMovieDetailAsync(int tmdbId, CancellationToken ct = default);
    Task<TmdbSeriesDetail?> GetSeriesDetailAsync(int tmdbId, CancellationToken ct = default);
    Task<TmdbSeasonDetail?> GetSeasonDetailAsync(int seriesTmdbId, int seasonNumber, CancellationToken ct = default);
    Task<TmdbPagedResult<TmdbMovieSummary>> GetSimilarMoviesAsync(int tmdbId, CancellationToken ct = default);
    Task<TmdbPagedResult<TmdbSeriesSummary>> GetSimilarSeriesAsync(int tmdbId, CancellationToken ct = default);
    Task<IReadOnlyList<TmdbWatchProvider>> GetMovieWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default);
    Task<IReadOnlyList<TmdbWatchProvider>> GetSeriesWatchProvidersAsync(int tmdbId, string region, CancellationToken ct = default);
}

public class TmdbNotConfiguredException()
    : Exception("TMDB API key is not configured. Set Tmdb:ApiKey (or the TMDB__APIKEY env var) to enable movie/series data.");

public record TmdbPagedResult<T>(IReadOnlyList<T> Results, int Page, int TotalPages, int TotalResults);

public record TmdbGenre(int Id, string Name);

public record TmdbMovieSummary(int Id, string Title, string? OriginalTitle, string? Overview, string? PosterPath,
    string? BackdropPath, DateOnly? ReleaseDate, double VoteAverage, double Popularity, IReadOnlyList<int> GenreIds);

public record TmdbMovieDetail(int Id, string Title, string? OriginalTitle, string? Overview, string? PosterPath,
    string? BackdropPath, DateOnly? ReleaseDate, int? Runtime, double VoteAverage, double Popularity,
    IReadOnlyList<TmdbGenre> Genres, IReadOnlyList<TmdbCastMember> Cast, IReadOnlyList<TmdbCrewMember> Crew,
    string? TrailerYoutubeKey);

public record TmdbSeriesSummary(int Id, string Name, string? OriginalName, string? Overview, string? PosterPath,
    string? BackdropPath, DateOnly? FirstAirDate, double VoteAverage, double Popularity, IReadOnlyList<int> GenreIds);

public record TmdbSeriesDetail(int Id, string Name, string? OriginalName, string? Overview, string? PosterPath,
    string? BackdropPath, DateOnly? FirstAirDate, DateOnly? LastAirDate, string Status, double VoteAverage,
    double Popularity, IReadOnlyList<TmdbGenre> Genres, IReadOnlyList<TmdbCastMember> Cast,
    IReadOnlyList<TmdbCrewMember> Crew, IReadOnlyList<TmdbSeasonSummary> Seasons, string? TrailerYoutubeKey);

public record TmdbSeasonSummary(int SeasonNumber, string Name, string? Overview, string? PosterPath, DateOnly? AirDate, int EpisodeCount);

public record TmdbSeasonDetail(int SeasonNumber, string Name, string? Overview, string? PosterPath, DateOnly? AirDate,
    IReadOnlyList<TmdbEpisodeSummary> Episodes);

public record TmdbEpisodeSummary(int EpisodeNumber, string Name, string? Overview, string? StillPath, DateOnly? AirDate, int? Runtime);

public record TmdbCastMember(int Id, string Name, string? Character, string? ProfilePath);

public record TmdbCrewMember(int Id, string Name, string? Job, string? ProfilePath);

public record TmdbWatchProvider(string ProviderName, string? LogoPath, string Type);
