namespace WatchLog.Application.Achievements;

/// <summary>
/// Evaluates a user's activity against the fixed set of <c>Achievement</c> criteria (seeded via
/// migration) and awards any newly-earned badges, pushing a notification for each. Called
/// opportunistically after tracking actions (see the episode/movie tracking controllers).
/// </summary>
public interface IAchievementService
{
    Task EvaluateAndAwardAsync(Guid userId, CancellationToken ct = default);
}
