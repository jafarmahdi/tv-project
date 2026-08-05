using Microsoft.EntityFrameworkCore;
using WatchLog.Application.Common.Interfaces;
using WatchLog.Application.Notifications;
using WatchLog.Domain.Entities;
using WatchLog.Domain.Enums;

namespace WatchLog.Application.Achievements;

public class AchievementService(IUnitOfWork unitOfWork, INotificationService notifications) : IAchievementService
{
    /// <summary>Achievement.Code -> predicate over (episodesWatched, moviesWatched).</summary>
    private static readonly IReadOnlyList<(string Code, Func<int, int, bool> IsMet)> Criteria =
    [
        ("first-watch", (ep, mv) => ep + mv >= 1),
        ("episodes-50", (ep, _) => ep >= 50),
        ("episodes-100", (ep, _) => ep >= 100),
        ("episodes-500", (ep, _) => ep >= 500),
        ("movies-25", (_, mv) => mv >= 25),
        ("movies-100", (_, mv) => mv >= 100)
    ];

    public async Task EvaluateAndAwardAsync(Guid userId, CancellationToken ct = default)
    {
        var episodesWatched = await unitOfWork.Repository<EpisodeProgress>().Query()
            .CountAsync(p => p.UserId == userId && p.Status == EpisodeWatchStatus.Watched, ct);
        var moviesWatched = await unitOfWork.Repository<MovieWatch>().Query()
            .CountAsync(w => w.UserId == userId && w.IsWatched, ct);

        var alreadyEarnedCodes = await unitOfWork.Repository<UserAchievement>().Query()
            .Where(a => a.UserId == userId)
            .Select(a => a.Achievement.Code)
            .ToListAsync(ct);

        var newlyEarned = Criteria
            .Where(c => !alreadyEarnedCodes.Contains(c.Code) && c.IsMet(episodesWatched, moviesWatched))
            .ToList();

        if (newlyEarned.Count == 0) return;

        var achievements = await unitOfWork.Repository<Achievement>().Query()
            .Where(a => newlyEarned.Select(n => n.Code).Contains(a.Code))
            .ToListAsync(ct);

        foreach (var achievement in achievements)
        {
            await unitOfWork.Repository<UserAchievement>().AddAsync(
                new UserAchievement { UserId = userId, AchievementId = achievement.Id }, ct);
        }
        await unitOfWork.SaveChangesAsync(ct);

        foreach (var achievement in achievements)
        {
            await notifications.CreateAndPushAsync(userId, NotificationType.Achievement,
                "Achievement unlocked!", achievement.Name, ct: ct);
        }
    }
}
