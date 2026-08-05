using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Achievements;
using WatchLog.Application.Catalog;
using WatchLog.Application.Common;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Tracking;

public class EpisodeTrackingService(ICatalogService catalog, IUnitOfWork unitOfWork, IAchievementService achievements) : IEpisodeTrackingService
{
    public async Task MarkEpisodeAsync(Guid userId, MarkEpisodeRequest request, CancellationToken ct = default)
    {
        var episodeId = await catalog.EnsureEpisodeCachedAsync(request.SeriesTmdbId, request.SeasonNumber, request.EpisodeNumber, ct);
        var repo = unitOfWork.Repository<EpisodeProgress>();
        var progress = await repo.Query().FirstOrDefaultAsync(p => p.UserId == userId && p.EpisodeId == episodeId, ct);

        var isNew = progress is null;
        progress ??= new EpisodeProgress { UserId = userId, EpisodeId = episodeId };
        progress.Status = request.Status;
        progress.WatchedAt = request.Status == EpisodeWatchStatus.Watched ? DateTimeOffset.UtcNow : progress.WatchedAt;

        if (isNew) await repo.AddAsync(progress, ct);
        else repo.Update(progress);

        if (request.Status == EpisodeWatchStatus.Watched)
        {
            await unitOfWork.Repository<ActivityFeedEntry>().AddAsync(new ActivityFeedEntry
            {
                UserId = userId,
                Type = ActivityType.WatchedEpisode,
                TargetType = TargetType.Episode,
                TargetId = episodeId,
                MetadataJson = $"{{\"seriesTmdbId\":{request.SeriesTmdbId},\"season\":{request.SeasonNumber},\"episode\":{request.EpisodeNumber}}}"
            }, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        if (request.Status == EpisodeWatchStatus.Watched)
        {
            await achievements.EvaluateAndAwardAsync(userId, ct);
        }
    }

    public async Task ToggleFavoriteAsync(Guid userId, ToggleEpisodeFavoriteRequest request, CancellationToken ct = default)
    {
        var episodeId = await catalog.EnsureEpisodeCachedAsync(request.SeriesTmdbId, request.SeasonNumber, request.EpisodeNumber, ct);
        var repo = unitOfWork.Repository<EpisodeProgress>();
        var progress = await repo.Query().FirstOrDefaultAsync(p => p.UserId == userId && p.EpisodeId == episodeId, ct);

        var isNew = progress is null;
        progress ??= new EpisodeProgress { UserId = userId, EpisodeId = episodeId };
        progress.IsFavorite = request.IsFavorite;

        if (isNew) await repo.AddAsync(progress, ct);
        else repo.Update(progress);
        await unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<SeasonProgressDto> GetSeasonProgressAsync(Guid userId, int seriesTmdbId, int seasonNumber, CancellationToken ct = default)
    {
        var seasonDetail = await catalog.GetSeasonAsync(seriesTmdbId, seasonNumber, ct);
        var seriesId = await catalog.EnsureSeriesCachedAsync(seriesTmdbId, ct);

        var season = await unitOfWork.Repository<Season>().Query()
            .Include(s => s.Episodes)
            .FirstOrDefaultAsync(s => s.SeriesId == seriesId && s.SeasonNumber == seasonNumber, ct)
            ?? throw new NotFoundException(nameof(Season), $"{seriesTmdbId}/{seasonNumber}");

        var episodeIds = season.Episodes.ToDictionary(e => e.EpisodeNumber, e => e.Id);
        var progressByEpisodeId = await unitOfWork.Repository<EpisodeProgress>().Query()
            .Where(p => p.UserId == userId && episodeIds.Values.Contains(p.EpisodeId))
            .ToDictionaryAsync(p => p.EpisodeId, ct);

        var episodes = seasonDetail.Episodes.Select(e =>
        {
            EpisodeProgress? progress = episodeIds.TryGetValue(e.EpisodeNumber, out var epId) && progressByEpisodeId.TryGetValue(epId, out var p) ? p : null;
            return new EpisodeProgressDto(e.EpisodeNumber, e.Title, progress?.Status ?? EpisodeWatchStatus.Unwatched,
                progress?.IsFavorite ?? false, progress?.WatchedAt);
        }).ToList();

        return new SeasonProgressDto(seasonNumber, episodes.Count,
            episodes.Count(e => e.Status == EpisodeWatchStatus.Watched), episodes);
    }

    public async Task<NextEpisodeDto?> GetNextEpisodeAsync(Guid userId, int seriesTmdbId, CancellationToken ct = default)
    {
        var seriesDetail = await catalog.GetSeriesDetailAsync(seriesTmdbId, ct);

        foreach (var season in seriesDetail.Seasons.OrderBy(s => s.SeasonNumber))
        {
            var progress = await GetSeasonProgressAsync(userId, seriesTmdbId, season.SeasonNumber, ct);
            var next = progress.Episodes
                .Where(e => e.Status == EpisodeWatchStatus.Unwatched)
                .OrderBy(e => e.EpisodeNumber)
                .FirstOrDefault();

            if (next is not null)
            {
                var seasonDetail = await catalog.GetSeasonAsync(seriesTmdbId, season.SeasonNumber, ct);
                var tmdbEpisode = seasonDetail.Episodes.First(e => e.EpisodeNumber == next.EpisodeNumber);
                return new NextEpisodeDto(season.SeasonNumber, next.EpisodeNumber, next.Title, tmdbEpisode.StillPath, tmdbEpisode.AirDate);
            }
        }

        return null;
    }
}

public class MovieTrackingService(ICatalogService catalog, IUnitOfWork unitOfWork, IAchievementService achievements) : IMovieTrackingService
{
    public async Task MarkMovieAsync(Guid userId, MarkMovieRequest request, CancellationToken ct = default)
    {
        var movieId = await catalog.EnsureMovieCachedAsync(request.MovieTmdbId, ct);
        var repo = unitOfWork.Repository<MovieWatch>();
        var watch = await repo.Query().FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movieId, ct);

        var isNew = watch is null;
        watch ??= new MovieWatch { UserId = userId, MovieId = movieId };
        watch.IsWatched = request.IsWatched;
        watch.WatchedAt = request.IsWatched ? DateTimeOffset.UtcNow : watch.WatchedAt;

        if (isNew) await repo.AddAsync(watch, ct);
        else repo.Update(watch);

        if (request.IsWatched)
        {
            await unitOfWork.Repository<ActivityFeedEntry>().AddAsync(new ActivityFeedEntry
            {
                UserId = userId,
                Type = ActivityType.WatchedMovie,
                TargetType = TargetType.Movie,
                TargetId = movieId
            }, ct);
        }

        await unitOfWork.SaveChangesAsync(ct);

        if (request.IsWatched)
        {
            await achievements.EvaluateAndAwardAsync(userId, ct);
        }
    }

    public async Task ToggleFavoriteAsync(Guid userId, ToggleMovieFavoriteRequest request, CancellationToken ct = default)
    {
        var movieId = await catalog.EnsureMovieCachedAsync(request.MovieTmdbId, ct);
        var repo = unitOfWork.Repository<MovieWatch>();
        var watch = await repo.Query().FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movieId, ct);

        var isNew = watch is null;
        watch ??= new MovieWatch { UserId = userId, MovieId = movieId };
        watch.IsFavorite = request.IsFavorite;

        if (isNew) await repo.AddAsync(watch, ct);
        else repo.Update(watch);
        await unitOfWork.SaveChangesAsync(ct);
    }
}
