namespace WatchLog.Application.Tracking;

public interface IEpisodeTrackingService
{
    Task MarkEpisodeAsync(Guid userId, MarkEpisodeRequest request, CancellationToken ct = default);
    Task ToggleFavoriteAsync(Guid userId, ToggleEpisodeFavoriteRequest request, CancellationToken ct = default);
    Task<SeasonProgressDto> GetSeasonProgressAsync(Guid userId, int seriesTmdbId, int seasonNumber, CancellationToken ct = default);

    /// <summary>"Auto next episode": the next unwatched episode after the user's furthest watched one.</summary>
    Task<NextEpisodeDto?> GetNextEpisodeAsync(Guid userId, int seriesTmdbId, CancellationToken ct = default);
}

public interface IMovieTrackingService
{
    Task MarkMovieAsync(Guid userId, MarkMovieRequest request, CancellationToken ct = default);
    Task ToggleFavoriteAsync(Guid userId, ToggleMovieFavoriteRequest request, CancellationToken ct = default);
}
