using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Builders;
using NetRabbit.Models;
using NetRabbit.Settings;
using RabbitMQ.Client;

namespace NetRabbit.Services.Publisher;

internal sealed class PublisherService : IPublisherService
{
    private readonly IRabbitChannelFactory _rabbitChannelFactory;
    private readonly IBasicConnectionSettings _basicConnectionSettings;
    private static readonly string AppName = AppDomain.CurrentDomain.FriendlyName;
    private const string ContentEncoding = "UTF-8";
    public PublisherService
    (
        IBasicConnectionSettings basicConnectionSettings,
        IRabbitChannelFactory rabbitChannelFactory
    )
    {
        _rabbitChannelFactory = rabbitChannelFactory;
        _basicConnectionSettings = basicConnectionSettings;
    }

    public Task<PublishResponseMessage> PublishAsync<T>
    (
        IBasicConnectionSettings connectionSettings,
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        return InternalPublish(connectionSettings, message, exchange, routingKey, cancellationToken);
    }

    public Task<PublishResponseMessage> PublishAsync<T>
    (
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        return InternalPublish(_basicConnectionSettings, message, exchange, routingKey, cancellationToken);
    }

    /// <summary>
    /// Sets publisher confirms on. Asynchronously awaits till node acks/nacks message.
    /// If message is not acked from node, exception will be wrapped
    /// </summary>
    /// <param name="connectionSettings"></param>
    /// <param name="message"></param>
    /// <param name="exchange"></param>
    /// <param name="routingKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PublishResponseMessage> PublishConfirmAsync<T>
    (
        IBasicConnectionSettings connectionSettings,
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        return InternalPublishConfirm(connectionSettings, message, exchange, routingKey, cancellationToken);
    }

    /// <summary>
    /// Sets publisher confirms on. Asynchronously awaits till node acks/nacks message.
    /// If message is not acked from node, exception will be wrapped
    /// </summary>
    /// <param name="message"></param>
    /// <param name="exchange"></param>
    /// <param name="routingKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<PublishResponseMessage> PublishConfirmAsync<T>
    (
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        return InternalPublishConfirm(_basicConnectionSettings, message, exchange, routingKey, cancellationToken);
    }

    private async Task<PublishResponseMessage> InternalPublish<T>
    (
        IBasicConnectionSettings connectionSettings,
        PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {

        try
        {
            var channel = await GetChannel(connectionSettings, exchange, confirmSelect: false, cancellationToken).ConfigureAwait(false);

            //var semaphore = Semaphores.GetOrAdd(settings, _ => new(1, 1));
            //if (channelTask.IsCompletedSuccessfully)
            //{
            //    return new ValueTask<PublishResponseMessage>
            //    (
            //        InternalPublish(channelTask.Result, message, exchange, routingKey)
            //    );
            //}

            return await InternalPublish(channel, message, exchange, routingKey, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            return PublishResponseMessage.FromException(exception);
        }
    }

    private static async Task<PublishResponseMessage> InternalPublish<T>
    (
        IChannel channel,
        //SemaphoreSlim semaphore,
        PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {

        try
        {
            //await semaphore.WaitAsync().ConfigureAwait(false);
            await channel.BasicPublishAsync
            (
                exchange,
                routingKey ?? string.Empty,
                false, GetBasicProperties(message),
                message.GetBody(),
                cancellationToken
            ).ConfigureAwait(false);
            return PublishResponseMessage.Success();
        }
        finally
        {
            //semaphore.Release();
        }

        //lock (channel)
        //{
        //    channel.BasicPublish
        //    (
        //        exchange,
        //        routingKey ?? string.Empty,
        //        GetBasicProperties(channel, message),
        //        message.GetBody()
        //    );

        //    return PublishResponseMessage.Success();
        //}
    }

    //private static async Task<PublishResponseMessage> InternalPublish<T>
    //(
    //    ValueTask<IChannel> channelTask,
    //    PublisherBrokeredMessage<T> message,
    //    string exchange,
    //    string? routingKey = default
    //) where T : notnull
    //{
    //    try
    //    {
    //        var channel = await channelTask.ConfigureAwait(false);
    //        return await InternalPublish(channel, message, exchange, routingKey);
    //    }
    //    catch (Exception exception)
    //    {
    //        return PublishResponseMessage.FromException(exception);
    //    }
    //}

    private async Task<PublishResponseMessage> InternalPublishConfirm<T>
    (
        IBasicConnectionSettings connectionSettings,
        PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull
    {
        var channelBuilderSettings = GetChannelBuilderSettings
        (
            connectionSettings,
            exchange,
            true
        );

        var channel = await _rabbitChannelFactory.CreateChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        //var semaphore = Semaphores.GetOrAdd(channelBuilderSettings, _ => new(1, 1));

        try
        {
            //await semaphore.WaitAsync().ConfigureAwait(false);

            var ackOrNackTask = channel.OnAckOrNack(cancellationToken);

            await channel.BasicPublishAsync
            (
                exchange,
                routingKey ?? string.Empty,
                false,
                GetBasicProperties(message),
                message.GetBody(),
                cancellationToken
            ).ConfigureAwait(false);

            await ackOrNackTask.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);

            return PublishResponseMessage.Success();
        }
        catch (AggregateException aggregateException)
        {
            return PublishResponseMessage.FromException(aggregateException.Flatten());
        }
        catch (Exception exception)
        {
            return PublishResponseMessage.FromException(exception);
        }
        finally
        {
            //semaphore.Release();
        }
    }

    private static ChannelBuilderSettings GetChannelBuilderSettings
    (
        IBasicConnectionSettings connectionSettings,
        string exchange,
        bool confirmSelect
    )
    {
        var stcs = new ServiceTypeConnectionSettings
        (
            ServiceType.Publisher,
            new ConnectionSettings(connectionSettings)
        );

        return new ChannelBuilderSettings(stcs, exchange, confirmSelect);
    }

    private async Task<IChannel> GetChannel(IBasicConnectionSettings connectionSettings, string exchange, bool confirmSelect, CancellationToken cancellationToken)
    {
        var channelBuilderSettings = GetChannelBuilderSettings(connectionSettings, exchange, confirmSelect);

        return await _rabbitChannelFactory.CreateChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
    }

    private static BasicProperties GetBasicProperties<T>(in PublisherBrokeredMessage<T> message) where T : notnull
    {
        return new BasicProperties
        {
            ContentType = message.ContentType,
            Headers = message.Headers!,
            ContentEncoding = ContentEncoding,
            MessageId = Guid.NewGuid().ToString(),
            AppId = AppName,
            DeliveryMode = DeliveryModes.Persistent
        };
    }

    //public void Dispose()
    //{
    //    //_semaphore.Dispose();
    //    _objectPool.Dispose();
    //}
}