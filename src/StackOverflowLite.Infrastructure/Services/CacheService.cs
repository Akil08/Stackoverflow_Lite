using System.Text.Json;
using StackOverflowLite.Application.Common.Interfaces;
using StackExchange.Redis;

namespace StackOverflowLite.Infrastructure.Services;

public class CacheService : ICacheService
{
    private readonly IConnectionMultiplexer _connectionMultiplexer;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public CacheService(IConnectionMultiplexer connectionMultiplexer)
    {
        _connectionMultiplexer = connectionMultiplexer;
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var value = await db.StringGetAsync(key);

        if (!value.HasValue)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(value!, _jsonOptions);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiry, CancellationToken cancellationToken = default)
    {
        var db = _connectionMultiplexer.GetDatabase();
        var serialized = JsonSerializer.Serialize(value, _jsonOptions);
        await db.StringSetAsync(key, serialized, expiry);
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
    {
        foreach (var endpoint in _connectionMultiplexer.GetEndPoints())
        {
            var server = _connectionMultiplexer.GetServer(endpoint);
            if (!server.IsConnected)
            {
                continue;
            }

            var keys = server.Keys(pattern: $"{prefix}*").ToArray();
            if (keys.Length == 0)
            {
                continue;
            }

            var db = _connectionMultiplexer.GetDatabase();
            await db.KeyDeleteAsync(keys);
        }
    }
}