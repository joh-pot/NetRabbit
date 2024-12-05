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

internal class MessageHandlerConsumerService<T> : IConsumerService<IMessageHandlerAsync<T>>
{
    public IAsyncBasicConsumer Setup
    (
        IChannel channel,
        IMessageHandlerAsync<T> handler,
        SubscriberSettings subscriberSettings,
        CancellationToken cancellationToken
    )
    {
        var consumer = new AsyncEventingBasicConsumer(channel);

        var responderService = new RabbitResponderService();

        _ = responderService.StartListening(cancellationToken);

        var serializer = handler.GetSerializer();

        consumer.ReceivedAsync += (sender, eventArgs) =>
            ReceivedAsync
            (
                ((IAsyncBasicConsumer)sender).Channel!,
                eventArgs,
                handler,
                serializer,
                subscriberSettings,
                responderService,
                cancellationToken
            );

        return consumer;
    }

    private static Task ReceivedAsync
    (
        IChannel channel,
        BasicDeliverEventArgs eventArgs,
        IMessageHandlerAsync<T> handler,
        ISerializer serializer,
        SubscriberSettings subscriberSettings,
        RabbitResponderService responderService,
        CancellationToken cancellationToken
    )
    {
        _ = RunProcessor
        (
            channel,
            eventArgs,
            handler,
            serializer,
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
        BasicDeliverEventArgs eventArgs,
        IMessageHandlerAsync<T> handler,
        ISerializer serializer,
        SubscriberSettings subscriberSettings,
        RabbitResponderService responderService,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var result = await handler.ProcessAsync
            (
                serializer.Deserialize<T>(eventArgs.Body.Span),
                handler.PopulateBasicMessageProperties() ? eventArgs.BasicProperties.ToBasicMessageProperties() : null,
                cancellationToken
            ).ConfigureAwait(false);

            await responderService.EnqueueToRespond
            (
                new ConsumerInfo(result, channel, subscriberSettings, eventArgs.DeliveryTag),
                cancellationToken
            ).ConfigureAwait(false);
        }
        catch (Exception)
        {
            await responderService.EnqueueToRespond
            (
                new ConsumerInfo(false, channel, subscriberSettings, eventArgs.DeliveryTag),
                cancellationToken
            ).ConfigureAwait(false);
        }
    }
}