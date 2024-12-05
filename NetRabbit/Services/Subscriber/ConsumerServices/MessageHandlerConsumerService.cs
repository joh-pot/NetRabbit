using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Settings;
using NetRabbit.Subscribers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NetRabbit.Services.Subscriber.ConsumerServices;

internal class MessageHandlerConsumerService : IConsumerService<IMessageHandlerAsync>
{
    public IAsyncBasicConsumer Setup
    (
        IChannel channel,
        IMessageHandlerAsync handler,
        SubscriberSettings subscriberSettings,
        CancellationToken cancellationToken
    )
    {
        var consumer = new AsyncEventingBasicConsumer(channel);

        var responderService = new RabbitResponderService();

        _ = responderService.StartListening(cancellationToken);

        consumer.ReceivedAsync += (sender, eventArgs) =>
            ReceivedAsync
            (
                ((IAsyncBasicConsumer)sender).Channel!,
                new SubscriberBrokeredMessage
                (
                    eventArgs.Body,
                    eventArgs.BasicProperties.ToBasicMessageProperties()
                ),
                eventArgs.DeliveryTag,
                handler,
                subscriberSettings,
                responderService,
                cancellationToken
            );

        return consumer;
    }

    private static Task ReceivedAsync
    (
        IChannel channel,
        in SubscriberBrokeredMessage brokeredMessage,
        ulong deliveryTag,
        IMessageHandlerAsync handler,
        SubscriberSettings subscriberSettings,
        RabbitResponderService responderService,
        CancellationToken cancellationToken
    )
    {
        _ = RunProcessor
        (
            channel,
            brokeredMessage,
            deliveryTag,
            handler,
            subscriberSettings,
            responderService,
            cancellationToken
        );

        return Task.CompletedTask;
    }

    [MethodImpl(MethodImplOptions.AggressiveOptimization)]
    private static async Task RunProcessor
    (
        IChannel channel,
        SubscriberBrokeredMessage brokeredMessage,
        ulong deliveryTag,
        IMessageHandlerAsync handler,
        SubscriberSettings subscriberSettings,
        RabbitResponderService responderService,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await handler.ProcessAsync(brokeredMessage, cancellationToken).ConfigureAwait(false);

            await responderService.EnqueueToRespond
            (
                new ConsumerInfo(result, channel, subscriberSettings, deliveryTag),
                cancellationToken
            ).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await responderService.EnqueueToRespond
            (
                new ConsumerInfo(false, channel, subscriberSettings, deliveryTag),
                cancellationToken
            ).ConfigureAwait(false);
        }
    }
}