namespace WatchLog.Application.Stats;

public interface IStatsService
{
    Task<UserStatsDto> GetUserStatsAsync(Guid userId, CancellationToken ct = default);
}
