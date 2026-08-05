using System.Text.Json;
using StackExchange.Redis;
using WatchLog.Application.Common.Interfaces;

namespace WatchLog.Infrastructure.Caching;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private IDatabase Db => redis.GetDatabase();

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await Db.StringGetAsync(key);
        return value.IsNullOrEmpty ? default : JsonSerializer.Deserialize<T>((string)value!, JsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct = default) =>
        await Db.StringSetAsync(key, JsonSerializer.Serialize(value, JsonOptions), ttl);

    public async Task RemoveAsync(string key, CancellationToken ct = default) => await Db.KeyDeleteAsync(key);

    public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory, CancellationToken ct = default)
    {
        var cached = await GetAsync<T>(key, ct);
        if (cached is not null) return cached;

        var value = await factory();
        await SetAsync(key, value, ttl, ct);
        return value;
    }
}
