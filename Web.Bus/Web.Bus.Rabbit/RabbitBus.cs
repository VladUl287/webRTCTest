using Polly;
using EasyNetQ;
using Web.Bus.Common;
using EasyNetQ.Topology;
using Polly.Contrib.WaitAndRetry;

namespace Web.Bus.Rabbit;

public sealed class RabbitBus : IMessageBus
{
    private readonly IAdvancedBus bus;
    private readonly IAsyncPolicy retryPolicy;

    private readonly object subscriptionLock = new();

    public RabbitBus(IBus bus)
    {
        this.bus = bus.Advanced;

        retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(5), 5),
                onRetry: (response, calculatedWaitDuration) =>
                {
                    //logging
                }
            );
    }

    public async Task PublishAsync<T>(T message, string exchangeName, string route)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNullOrEmpty(exchangeName);

        await bus.ExchangeDeclarePassiveAsync(exchangeName, CancellationToken.None);

        var body = new Message<T>(message);
        var exchange = new Exchange(exchangeName);

        await retryPolicy.ExecuteAsync(() => bus.PublishAsync(exchange, route, mandatory: false, body));
    }

    public async Task SubscribeAsync<T>(Func<T, Task> handler, string queueName)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(queueName);

        await bus.QueueDeclarePassiveAsync(queueName, CancellationToken.None);

        var queue = new Queue(queueName);

        Func<IMessage<T>, MessageReceivedInfo, Task> subscribe = (message, _) => handler(message.Body);

        bus.Consume(queue, subscribe);
    }

    public async Task BindAsync(string exchangeName, string queueName, string route)
    {
        ArgumentNullException.ThrowIfNull(route);
        ArgumentNullException.ThrowIfNullOrEmpty(exchangeName);
        ArgumentNullException.ThrowIfNullOrEmpty(queueName);

        var exchange = await bus.ExchangeDeclareAsync(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
        var queue = await bus.QueueDeclareAsync(queueName, durable: true, exclusive: false, autoDelete: false);

        await bus.BindAsync(exchange, queue, route);
    }
}
