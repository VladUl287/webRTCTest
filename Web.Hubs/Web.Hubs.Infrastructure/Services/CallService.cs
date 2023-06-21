using Web.Hubs.Core.Services;

namespace Web.Hubs.Infrastructure.Services;

public sealed class CallService : ICallService
{
    private static readonly Dictionary<Guid, HashSet<long>> calls = new();

    public Task AddUser(Guid callId, long value)
    {
        if (calls.TryGetValue(callId, out var values))
        {
            values.Add(value);
        }
        else
        {
            calls.Add(callId, new() { value });
        }

        return Task.CompletedTask;
    }

    public Task<bool> UserExists(long userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> UserExists(Guid callId, long userId)
    {
        throw new NotImplementedException();
    }

    public Task RemoveUser(Guid callId, long userId)
    {
        throw new NotImplementedException();
    }

    public Task Flush()
    {
        return Task.CompletedTask;
    }
}
