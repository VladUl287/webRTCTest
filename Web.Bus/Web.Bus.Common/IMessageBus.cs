namespace Web.Bus.Common;

public interface IMessageBus
{
    void Publish<T>(T message, string exchangeName, string route);

    void Subscribe<T>(Func<T, Task> handler, string queueName);

    void Bind(string exchangeName, string queueName, string route);
}
