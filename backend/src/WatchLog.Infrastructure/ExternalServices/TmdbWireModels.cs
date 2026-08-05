using System.Text.Json.Serialization;

namespace WatchLog.Infrastructure.ExternalServices;

/// <summary>
/// Wire-format DTOs matching TMDB's actual JSON responses (https://developer.themoviedb.org/reference).
/// Kept private to Infrastructure and mapped to `WatchLog.Application.Common.Interfaces` DTOs by
/// <see cref="TmdbClient"/> so the rest of the app never depends on TMDB's field naming.
/// </summary>
internal record TmdbWirePagedResult<T>(
    [property: JsonPropertyName("page")] int Page,
    [property: JsonPropertyName("results")] List<T> Results,
    [property: JsonPropertyName("total_pages")] int TotalPages,
    [property: JsonPropertyName("total_results")] int TotalResults);

internal record TmdbWireGenre([property: JsonPropertyName("id")] int Id, [property: JsonPropertyName("name")] string Name);

internal record TmdbWireMovieSummary(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("original_title")] string? OriginalTitle,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("poster_path")] string? PosterPath,
    [property: JsonPropertyName("backdrop_path")] string? BackdropPath,
    [property: JsonPropertyName("release_date")] string? ReleaseDate,
    [property: JsonPropertyName("vote_average")] double VoteAverage,
    [property: JsonPropertyName("popularity")] double Popularity,
    [property: JsonPropertyName("genre_ids")] List<int>? GenreIds);

internal record TmdbWireSeriesSummary(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("original_name")] string? OriginalName,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("poster_path")] string? PosterPath,
    [property: JsonPropertyName("backdrop_path")] string? BackdropPath,
    [property: JsonPropertyName("first_air_date")] string? FirstAirDate,
    [property: JsonPropertyName("vote_average")] double VoteAverage,
    [property: JsonPropertyName("popularity")] double Popularity,
    [property: JsonPropertyName("genre_ids")] List<int>? GenreIds);

internal record TmdbWireCastMember(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("character")] string? Character,
    [property: JsonPropertyName("profile_path")] string? ProfilePath);

internal record TmdbWireCrewMember(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("job")] string? Job,
    [property: JsonPropertyName("profile_path")] string? ProfilePath);

internal record TmdbWireCredits(
    [property: JsonPropertyName("cast")] List<TmdbWireCastMember>? Cast,
    [property: JsonPropertyName("crew")] List<TmdbWireCrewMember>? Crew);

internal record TmdbWireVideo(
    [property: JsonPropertyName("key")] string Key,
    [property: JsonPropertyName("site")] string Site,
    [property: JsonPropertyName("type")] string Type);

internal record TmdbWireVideos([property: JsonPropertyName("results")] List<TmdbWireVideo>? Results);

internal record TmdbWireMovieDetail(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("title")] string Title,
    [property: JsonPropertyName("original_title")] string? OriginalTitle,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("poster_path")] string? PosterPath,
    [property: JsonPropertyName("backdrop_path")] string? BackdropPath,
    [property: JsonPropertyName("release_date")] string? ReleaseDate,
    [property: JsonPropertyName("runtime")] int? Runtime,
    [property: JsonPropertyName("vote_average")] double VoteAverage,
    [property: JsonPropertyName("popularity")] double Popularity,
    [property: JsonPropertyName("genres")] List<TmdbWireGenre>? Genres,
    [property: JsonPropertyName("credits")] TmdbWireCredits? Credits,
    [property: JsonPropertyName("videos")] TmdbWireVideos? Videos);

internal record TmdbWireSeasonSummary(
    [property: JsonPropertyName("season_number")] int SeasonNumber,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("poster_path")] string? PosterPath,
    [property: JsonPropertyName("air_date")] string? AirDate,
    [property: JsonPropertyName("episode_count")] int EpisodeCount);

internal record TmdbWireSeriesDetail(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("original_name")] string? OriginalName,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("poster_path")] string? PosterPath,
    [property: JsonPropertyName("backdrop_path")] string? BackdropPath,
    [property: JsonPropertyName("first_air_date")] string? FirstAirDate,
    [property: JsonPropertyName("last_air_date")] string? LastAirDate,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("vote_average")] double VoteAverage,
    [property: JsonPropertyName("popularity")] double Popularity,
    [property: JsonPropertyName("genres")] List<TmdbWireGenre>? Genres,
    [property: JsonPropertyName("seasons")] List<TmdbWireSeasonSummary>? Seasons,
    [property: JsonPropertyName("credits")] TmdbWireCredits? Credits,
    [property: JsonPropertyName("videos")] TmdbWireVideos? Videos);

internal record TmdbWireEpisodeSummary(
    [property: JsonPropertyName("episode_number")] int EpisodeNumber,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("still_path")] string? StillPath,
    [property: JsonPropertyName("air_date")] string? AirDate,
    [property: JsonPropertyName("runtime")] int? Runtime);

internal record TmdbWireSeasonDetail(
    [property: JsonPropertyName("season_number")] int SeasonNumber,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("overview")] string? Overview,
    [property: JsonPropertyName("poster_path")] string? PosterPath,
    [property: JsonPropertyName("air_date")] string? AirDate,
    [property: JsonPropertyName("episodes")] List<TmdbWireEpisodeSummary>? Episodes);

internal record TmdbWireProviderEntry(
    [property: JsonPropertyName("provider_name")] string ProviderName,
    [property: JsonPropertyName("logo_path")] string? LogoPath);

internal record TmdbWireProviderRegion(
    [property: JsonPropertyName("flatrate")] List<TmdbWireProviderEntry>? Flatrate,
    [property: JsonPropertyName("rent")] List<TmdbWireProviderEntry>? Rent,
    [property: JsonPropertyName("buy")] List<TmdbWireProviderEntry>? Buy);

internal record TmdbWireProvidersResponse(
    [property: JsonPropertyName("results")] Dictionary<string, TmdbWireProviderRegion>? Results);
