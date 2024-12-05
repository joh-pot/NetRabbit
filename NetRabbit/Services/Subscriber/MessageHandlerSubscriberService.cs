using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Builders;
using NetRabbit.Models;
using NetRabbit.Services.Subscriber.ConsumerServices;
using NetRabbit.Settings;
using NetRabbit.Subscribers;
using RabbitMQ.Client;

namespace NetRabbit.Services.Subscriber;

internal class MessageHandlerSubscriberService : ISubscriberService<IMessageHandlerAsync>
{
    private readonly IRabbitChannelFactory _rabbitChannelFactory;
    private readonly IConsumerService<IMessageHandlerAsync> _consumerService;

    public MessageHandlerSubscriberService(IRabbitChannelFactory rabbitChannelFactory, IConsumerService<IMessageHandlerAsync> consumerService)
    {
        _rabbitChannelFactory = rabbitChannelFactory;
        _consumerService = consumerService;
    }
    public Task<IRabbitConsumer> StartAsync
    (
        SubscriberSettings subscriberSettings,
        IConnectionSettings connectionSettings,
        IMessageHandlerAsync handler,
        CancellationToken cancellationToken = default
    )
    {
        var channelBuilderSettings = new ChannelBuilderSettings(subscriberSettings, connectionSettings);

        return InitChannel(subscriberSettings, channelBuilderSettings, handler, cancellationToken);
    }

    private async Task<IRabbitConsumer> InitChannel
    (
        SubscriberSettings subscriberSettings,
        ChannelBuilderSettings channelBuilderSettings,
        IMessageHandlerAsync handler,
        CancellationToken cancellationToken
    )
    {
        var channel = await GetChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        var consumer = _consumerService.Setup(channel, handler, subscriberSettings, cancellationToken);

        await SetupChannel(channel, subscriberSettings, consumer, cancellationToken).ConfigureAwait(false);

        _ = HandleShutdown(channel, subscriberSettings, channelBuilderSettings, handler, cancellationToken);

        return new RabbitConsumer(consumer);
    }

    private Task<IChannel> GetChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        return _rabbitChannelFactory.CreateChannel(channelBuilderSettings, cancellationToken);
    }

    private static async Task SetupChannel(IChannel channel, SubscriberSettings subscriberSettings, IAsyncBasicConsumer consumer, CancellationToken cancellationToken)
    {
        await channel.BasicQosAsync(0, subscriberSettings.PrefetchCount, false, cancellationToken).ConfigureAwait(false);

        await channel.BasicConsumeAsync
        (
            subscriberSettings.QueueName!,
            autoAck: false,
            subscriberSettings.MessageHandlerTag!,
            noLocal: true,
            exclusive: false,
            subscriberSettings.BindingArguments,
            consumer,
            cancellationToken
        ).ConfigureAwait(false);
    }

    private async Task HandleShutdown
    (
        IChannel channel,
        SubscriberSettings subscriberSettings,
        ChannelBuilderSettings channelBuilderSettings,
        IMessageHandlerAsync handler,
        CancellationToken cancellationToken
    )
    {
        await channel.OnShutdown(cancellationToken).ConfigureAwait(false);
        await CleanupAndRestoreChannel(subscriberSettings, channelBuilderSettings, handler, cancellationToken).ConfigureAwait(false);
    }

    private async Task CleanupAndRestoreChannel
    (
        SubscriberSettings subscriberSettings,
        ChannelBuilderSettings channelBuilderSettings,
        IMessageHandlerAsync handler,
        CancellationToken cancellationToken
    )
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return;
        }

        await _rabbitChannelFactory.RestoreChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        await InitChannel
        (
            subscriberSettings,
            channelBuilderSettings,
            handler, cancellationToken
        ).ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _rabbitChannelFactory.DisposeAsync();
    }
}