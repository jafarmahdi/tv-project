using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Stats;

public class StatsService(IUnitOfWork unitOfWork) : IStatsService
{
    public async Task<UserStatsDto> GetUserStatsAsync(Guid userId, CancellationToken ct = default)
    {
        var watchedEpisodes = await unitOfWork.Repository<EpisodeProgress>().Query()
            .Where(p => p.UserId == userId && p.Status == EpisodeWatchStatus.Watched)
            .Include(p => p.Episode).ThenInclude(e => e.Season).ThenInclude(s => s.Series).ThenInclude(s => s.Genres).ThenInclude(g => g.Genre)
            .ToListAsync(ct);

        var watchedMovies = await unitOfWork.Repository<MovieWatch>().Query()
            .Where(w => w.UserId == userId && w.IsWatched)
            .Include(w => w.Movie).ThenInclude(m => m.Genres).ThenInclude(g => g.Genre)
            .ToListAsync(ct);

        var episodeMinutes = watchedEpisodes.Sum(p => p.Episode.RuntimeMinutes ?? 40);
        var movieMinutes = watchedMovies.Sum(w => w.Movie.RuntimeMinutes ?? 110);

        var monthly = watchedEpisodes.Select(p => p.WatchedAt).Concat(watchedMovies.Select(w => w.WatchedAt))
            .Where(d => d is not null).Select(d => d!.Value)
            .GroupBy(d => (d.Year, d.Month))
            .Select(g => new { g.Key.Year, g.Key.Month, Count = g.Count() })
            .OrderBy(g => g.Year).ThenBy(g => g.Month)
            .ToList();

        var monthlyEpisodeCounts = watchedEpisodes.Where(p => p.WatchedAt is not null)
            .GroupBy(p => (p.WatchedAt!.Value.Year, p.WatchedAt.Value.Month))
            .ToDictionary(g => g.Key, g => g.Count());
        var monthlyMovieCounts = watchedMovies.Where(w => w.WatchedAt is not null)
            .GroupBy(w => (w.WatchedAt!.Value.Year, w.WatchedAt.Value.Month))
            .ToDictionary(g => g.Key, g => g.Count());

        var monthlyActivity = monthly
            .Select(m => new MonthlyActivityDto(m.Year, m.Month,
                monthlyEpisodeCounts.GetValueOrDefault((m.Year, m.Month)),
                monthlyMovieCounts.GetValueOrDefault((m.Year, m.Month))))
            .ToList();

        var genreCounts = watchedEpisodes.SelectMany(p => p.Episode.Season.Series.Genres.Select(g => g.Genre.Name))
            .Concat(watchedMovies.SelectMany(w => w.Movie.Genres.Select(g => g.Genre.Name)))
            .GroupBy(name => name)
            .Select(g => new GenreStatDto(g.Key, g.Count()))
            .OrderByDescending(g => g.Count)
            .Take(10)
            .ToList();

        var heatmap = watchedEpisodes.Select(p => p.WatchedAt).Concat(watchedMovies.Select(w => w.WatchedAt))
            .Where(d => d is not null).Select(d => DateOnly.FromDateTime(d!.Value.UtcDateTime))
            .GroupBy(d => d)
            .Select(g => new HeatmapDayDto(g.Key, g.Count()))
            .OrderBy(g => g.Date)
            .ToList();

        var achievements = await unitOfWork.Repository<UserAchievement>().Query()
            .Where(a => a.UserId == userId)
            .Include(a => a.Achievement)
            .Select(a => new BadgeDto(a.Achievement.Code, a.Achievement.Name, a.Achievement.Description, a.Achievement.IconUrl, a.EarnedAt))
            .ToListAsync(ct);

        return new UserStatsDto(
            watchedEpisodes.Count,
            watchedMovies.Count,
            episodeMinutes + movieMinutes,
            monthlyActivity,
            genreCounts,
            heatmap,
            achievements);
    }
}
