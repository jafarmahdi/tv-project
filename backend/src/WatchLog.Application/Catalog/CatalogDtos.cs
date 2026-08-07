namespace WatchLog.Application.Catalog;

public record MovieSummaryDto(int TmdbId, string Title, string? PosterPath, string? BackdropPath,
    DateOnly? ReleaseDate, double VoteAverage, IReadOnlyList<string> Genres);

public record SeriesSummaryDto(int TmdbId, string Title, string? PosterPath, string? BackdropPath,
    DateOnly? FirstAirDate, double VoteAverage, IReadOnlyList<string> Genres);

public record CastMemberDto(int TmdbId, string Name, string? Character, string? ProfilePath);
public record CrewMemberDto(int TmdbId, string Name, string? Job, string? ProfilePath);

public record MovieDetailDto(Guid Id, int TmdbId, string Title, string? OriginalTitle, string? Overview, string? PosterPath,
    string? BackdropPath, DateOnly? ReleaseDate, int? RuntimeMinutes, double VoteAverage,
    IReadOnlyList<string> Genres, IReadOnlyList<CastMemberDto> Cast, IReadOnlyList<CrewMemberDto> Crew,
    string? TrailerYoutubeKey);

public record EpisodeSummaryDto(Guid Id, int EpisodeNumber, string Title, string? Overview, string? StillPath,
    DateOnly? AirDate, int? RuntimeMinutes);

public record SeasonSummaryDto(int SeasonNumber, string Name, string? PosterPath, DateOnly? AirDate, int EpisodeCount);

public record SeriesDetailDto(Guid Id, int TmdbId, string Title, string? OriginalTitle, string? Overview, string? PosterPath,
    string? BackdropPath, DateOnly? FirstAirDate, DateOnly? LastAirDate, string Status, double VoteAverage,
    IReadOnlyList<string> Genres, IReadOnlyList<CastMemberDto> Cast, IReadOnlyList<CrewMemberDto> Crew,
    IReadOnlyList<SeasonSummaryDto> Seasons, string? TrailerYoutubeKey);

public record SeasonDetailDto(int SeasonNumber, string Name, string? Overview, string? PosterPath,
    DateOnly? AirDate, IReadOnlyList<EpisodeSummaryDto> Episodes);

public record WatchProviderDto(string ProviderName, string? LogoPath, string Type);
