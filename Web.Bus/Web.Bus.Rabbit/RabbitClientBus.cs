using Polly;
using System.Text;
using Web.Bus.Common;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Web.Bus.Common.Exceptions;
using System.Collections.Concurrent;

namespace Web.Bus.Rabbit;

public sealed class RabbitClientBus : IMessageBus, IDisposable
{
    private readonly IModel channel;
    private readonly IConnection connection;
    private readonly ConcurrentDictionary<string, IModel> channels;

    private readonly object _lock = new();
    private bool _disposed = false;

    public RabbitClientBus(ConnectionFactory factory)
    {
        factory.DispatchConsumersAsync = true;

        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channels = new();
    }

    public void Publish<T>(T message, string exchangeName, string route)
    {
        RabbitDisposeExeption.ThrowIfDisposed(_disposed);

        ArgumentException.ThrowIfNullOrEmpty(exchangeName);

        ArgumentNullException.ThrowIfNull(message);
        ArgumentNullException.ThrowIfNull(route);

        var body = Encoding.UTF8.GetBytes("test message");

        lock (_lock)
        {
            RabbitDisposeExeption.ThrowIfDisposed(_disposed);

            channel?.ExchangeDeclarePassive(exchangeName);

            channel?.BasicPublish(exchangeName, route, mandatory: false, null, body);
        }
    }

    public void Subscribe<T>(Func<T, Task> handler, string queueName)
    {
        RabbitDisposeExeption.ThrowIfDisposed(_disposed);

        ArgumentException.ThrowIfNullOrEmpty(queueName);

        lock (_lock)
        {
            RabbitDisposeExeption.ThrowIfDisposed(_disposed);

            if (!channels.TryGetValue(queueName, out var channel))
            {
                channel = connection.CreateModel();

                channels.TryAdd(queueName, channel);
            }

            channel?.QueueDeclarePassive(queueName);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.Received += async (ch, args) =>
            {
                var body = args.Body.ToArray();

                await handler(default(T));

                channel?.BasicAck(args.DeliveryTag, false);

                await Task.Yield();
            };

            channel?.BasicConsume(queueName, false, consumer);
        }
    }

    public void Bind(string exchangeName, string queueName, string route)
    {
        RabbitDisposeExeption.ThrowIfDisposed(_disposed);

        ArgumentException.ThrowIfNullOrEmpty(queueName);
        ArgumentException.ThrowIfNullOrEmpty(exchangeName);

        ArgumentNullException.ThrowIfNull(route);

        lock (_lock)
        {
            RabbitDisposeExeption.ThrowIfDisposed(_disposed);

            channel?.ExchangeDeclare(exchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
            channel?.QueueDeclare(queueName, durable: true, exclusive: false, autoDelete: false);

            channel?.QueueBind(queueName, exchangeName, route);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            foreach (var channel in channels)
            {
                channel.Value?.Close();
                channel.Value?.Dispose();
            }

            channels?.Clear();

            channel?.Close();
            channel?.Dispose();

            connection?.Close();
            connection?.Dispose();

            _disposed = true;
        }
    }
}
