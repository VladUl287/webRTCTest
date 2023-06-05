using Web.Hubs.Core.Services;

namespace Web.Hubs.Infrastructure.Services;

public sealed class ConnectionServiceDict : IConnectionService
{
    private static readonly Dictionary<long, HashSet<string>> connections = new();

    public async Task<string[]> Get(long userId)
    {
        if (connections.TryGetValue(userId, out HashSet<string> value))
        {
            return value.ToArray();
        }

        return Array.Empty<string>();
    }

    public async Task<string[]> Get(long[] userIds)
    {
        var result = new List<string>();

        foreach (var item in userIds)
        {
            if (connections.TryGetValue(item, out HashSet<string> value))
            {
                result.AddRange(value);
            }
        }

        return result.ToArray();
    }

    public Task Add(long userId, string value)
    {
        if (connections.TryGetValue(userId, out HashSet<string> values))
        {
            values.Add(value);
        }
        else
        {
            connections.Add(userId, new() { value });
        }

        return Task.CompletedTask;
    }

    public Task Delete(long userId, string value)
    {
        if (connections.TryGetValue(userId, out HashSet<string> values))
        {
            values.Remove(value);
        }

        return Task.CompletedTask;
    }

    public Task Flush()
    {
        return Task.CompletedTask;
    }
}
