using System;
using Polly;
using EasyNetQ;
using Web.Bus.Common;
using Polly.Contrib.WaitAndRetry;

namespace Web.Bus.Rabbit;

public class RabbitBus : IMessageBus
{
    private readonly IBus bus;
    private readonly IAsyncPolicy retryPolicy;

    public RabbitBus(IBus bus)
    {
        this.bus = bus;

        retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(10), 5)
            // onRetry: (response, calculatedWaitDuration) =>
            // {
            //     this.logger.Debug(_category, "RabbitMQ Error EventBus. Check RabbitMQ Server State");
            // }
            );
    }

    public Task PublishAsync<T>(T message, string exchange, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, string queue, CancellationToken token = default)
    {
        throw new NotImplementedException();
    }
}
