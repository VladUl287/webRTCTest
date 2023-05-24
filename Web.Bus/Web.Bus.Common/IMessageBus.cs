namespace Web.Bus.Common;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, string exchangeName, string route);

    Task SubscribeAsync<T>(Func<T, Task> handler, string queueName);

    Task BindAsync(string exchangeName, string queueName, string route);
}
