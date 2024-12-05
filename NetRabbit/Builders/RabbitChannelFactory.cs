using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace NetRabbit.Builders;

internal sealed class RabbitChannelFactory : IRabbitChannelFactory
{
    private readonly IRabbitChannelBuilder _channelBuilder;
    private readonly ConcurrentDictionary<ChannelBuilderSettings, SemaphoreSlim> _semaphores = new();

    public RabbitChannelFactory(IRabbitChannelBuilder channelBuilder)
    {
        _channelBuilder = channelBuilder;
    }

    public async Task<IChannel> CreateChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        var channel = await _channelBuilder.Build(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        return await CleanupAndBuildChannel(channel, channelBuilderSettings, cancellationToken).ConfigureAwait(false);
    }

    public async Task RestoreChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        var connection = await _channelBuilder.GetChannelConnection(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        if (connection?.IsOpen == false)
        {
            await CleanupSelfAndDownstream(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            await CleanupChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
        }
    }

    public async Task CleanupChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        await _channelBuilder.CleanupChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanupSelfAndDownstream(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        await _channelBuilder.CleanupSelfAndDownstream(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
    }

    private Task<IChannel> CleanupAndBuildChannel
    (
        IChannel channel,
        ChannelBuilderSettings channelBuilderSettings,
        CancellationToken cancellationToken
    )
    {
        return channelBuilderSettings.ServiceTypeConnectionSettings.Service switch
        {
            ServiceType.Publisher => CleanupAndBuildChannelPublisher(channel, channelBuilderSettings, cancellationToken),
            ServiceType.Subscriber => CleanupAndBuildChannelSubscriber(channel, channelBuilderSettings, cancellationToken),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private async Task<IChannel> CleanupAndBuildChannelSubscriber
    (
        IChannel channel,
        ChannelBuilderSettings channelBuilderSettings,
        CancellationToken cancellationToken
    )
    {
        //for subscribers we want it to retry connection attempts for as long as it takes
        while (true)
        {
            var semaphore = _semaphores.GetOrAdd(channelBuilderSettings, new SemaphoreSlim(1, 1));

            try
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                if (channel.IsOpen)
                {
                    return channel;
                }

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);

                await RestoreChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

                channel = await _channelBuilder.Build(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
            }
            catch (BrokerUnreachableException)
            {
                await _channelBuilder.CleanupSelfAndDownstream(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

                await Task.Delay(TimeSpan.FromSeconds(5), cancellationToken).ConfigureAwait(false);

                channel = await _channelBuilder.Build(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    private async Task<IChannel> CleanupAndBuildChannelPublisher
    (
        IChannel channel,
        ChannelBuilderSettings channelBuilderSettings,
        CancellationToken cancellationToken
    )
    {
        //publishers we want a quick fail as it's usually part of the request path
        for (var counter = 0; counter < 3; counter++)
        {
            var semaphore = _semaphores.GetOrAdd(channelBuilderSettings, new SemaphoreSlim(1, 1));

            try
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                if (channel.IsOpen)
                {
                    return channel;
                }

                await RestoreChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

                channel = await _channelBuilder.Build(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
            }
            catch (BrokerUnreachableException)
            {
                await _channelBuilder.CleanupSelfAndDownstream(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

                channel = await _channelBuilder.Build(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                semaphore?.Release();
            }
        }

        throw new Exception("Could not establish a connection to the broker after multiple attempts");
    }

    public ValueTask DisposeAsync()
    {
        return _channelBuilder.DisposeAsync();
    }
}