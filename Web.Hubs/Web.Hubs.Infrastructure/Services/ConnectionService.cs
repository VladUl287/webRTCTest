using StackExchange.Redis;
using Web.Hubs.Core.Services;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ConnectionService : IConnectionService
{
    private readonly IDatabaseAsync database;
    private readonly ConnectionMultiplexer redis;

    public ConnectionService(ConnectionMultiplexer redis)
    {
        database = redis.GetDatabase();
        this.redis = redis;
    }

    public async Task<string[]> Get(long userId)
    {
        var redisKey = new RedisKey(userId.ToString());

        var values = await database.HashGetAllAsync(redisKey);

        return values
            .Select(e => e.Value.ToString())
            .ToArray();
    }

    public async Task<string[]> Get(long[] userIds)
    {
        var entries = new List<HashEntry>();

        foreach (var userId in userIds)
        {
            var redisKey = new RedisKey(userId.ToString());

            var values = await database.HashGetAllAsync(redisKey);

            entries.AddRange(values);
        }

        return entries
            .Select(e => e.Value.ToString())
            .ToArray();
    }

    public Task Add(long userId, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var redisKey = new RedisKey(userId.ToString());

        var redisValue = new RedisValue(value);

        return database.HashSetAsync(redisKey, redisValue, redisValue);
    }

    public Task Delete(long userId, string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return Task.CompletedTask;
        }

        var redisKey = new RedisKey(userId.ToString());

        var redisValue = new RedisValue(value);

        return database.HashDeleteAsync(redisKey, redisValue);
    }

    public async Task Flush()
    {
        var endpoint = await database.IdentifyEndpointAsync();

        await redis.GetServer(endpoint).FlushDatabaseAsync();
    }
}
