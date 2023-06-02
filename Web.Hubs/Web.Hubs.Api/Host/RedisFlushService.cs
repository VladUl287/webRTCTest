using Web.Hubs.Core.Services;

namespace Web.Hubs.Api.Host;

public sealed class RedisFlushService : IHostedService
{
    private readonly IConnectionService connectionService;

    public RedisFlushService(IConnectionService connectionService)
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
