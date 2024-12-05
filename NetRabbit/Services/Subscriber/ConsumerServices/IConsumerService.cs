using System.Threading;
using NetRabbit.Settings;
using RabbitMQ.Client;

namespace NetRabbit.Services.Subscriber.ConsumerServices;

internal interface IConsumerService<in THandler>
{
    public IAsyncBasicConsumer Setup
    (
        IChannel channel,
        THandler handler,
        SubscriberSettings subscriberSettings,
        CancellationToken cancellationToken
    );
}