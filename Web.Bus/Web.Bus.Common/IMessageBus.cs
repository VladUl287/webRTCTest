namespace Web.Bus.Common;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string exchange, CancellationToken cancellationToken = default);

    Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, string queue, CancellationToken token = default);
}
