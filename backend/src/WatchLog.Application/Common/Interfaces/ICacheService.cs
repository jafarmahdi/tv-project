namespace WatchLog.Application.Common.Interfaces;

/// <summary>Redis-backed cache-aside abstraction, used mainly to avoid re-hitting TMDB for hot data.</summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);

    /// <summary>Cache-aside helper: return the cached value, or compute + cache it on a miss.</summary>
    Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory, CancellationToken ct = default);
}
