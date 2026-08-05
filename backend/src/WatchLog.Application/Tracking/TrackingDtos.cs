using WatchLog.Domain.Enums;

namespace WatchLog.Application.Tracking;

public record MarkEpisodeRequest(int SeriesTmdbId, int SeasonNumber, int EpisodeNumber, EpisodeWatchStatus Status);
public record ToggleEpisodeFavoriteRequest(int SeriesTmdbId, int SeasonNumber, int EpisodeNumber, bool IsFavorite);

public record EpisodeProgressDto(int EpisodeNumber, string Title, EpisodeWatchStatus Status, bool IsFavorite, DateTimeOffset? WatchedAt);

public record SeasonProgressDto(int SeasonNumber, int TotalEpisodes, int WatchedEpisodes, IReadOnlyList<EpisodeProgressDto> Episodes);

public record MarkMovieRequest(int MovieTmdbId, bool IsWatched);
public record ToggleMovieFavoriteRequest(int MovieTmdbId, bool IsFavorite);

/// <summary>Result of "auto next episode": the next unwatched episode in the series, if any.</summary>
public record NextEpisodeDto(int SeasonNumber, int EpisodeNumber, string Title, string? StillPath, DateOnly? AirDate);
