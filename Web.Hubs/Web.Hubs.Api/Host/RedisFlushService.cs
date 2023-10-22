using Web.Hubs.Core.Contracts.Services;

namespace Web.Hubs.Api.Host;

public sealed class RedisFlushService : IHostedService
{
    private readonly IStorage<long> connectionService;

    public RedisFlushService(IStorage<long> connectionService)
    {
        this.connectionService = connectionService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return connectionService.Flush();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return connectionService.Flush();
    }
}
