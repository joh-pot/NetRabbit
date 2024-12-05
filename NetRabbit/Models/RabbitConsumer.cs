using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace NetRabbit.Models;

public interface IRabbitConsumer
{
    Task MessageDelivered();
}

internal class RabbitConsumer : IRabbitConsumer
{
    private readonly AsyncDefaultBasicConsumer _consumer;

    public RabbitConsumer(IAsyncBasicConsumer consumer)
    {
        _consumer = (AsyncDefaultBasicConsumer)consumer;
    }

    public Task MessageDelivered()
    {
        return _consumer.HandleBasicDeliverAsync("", 0, false, "", "", new BasicProperties(), new ReadOnlyMemory<byte>());
    }
}