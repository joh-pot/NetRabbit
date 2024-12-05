using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;

namespace NetRabbit.Builders;

internal sealed class RabbitChannelBuilder : IRabbitChannelBuilder
{
    private readonly IRabbitConnectionFactory _connectionFactory;
    private readonly ConcurrentDictionary<ChannelBuilderConnectionSettings, Lazy<Task<IChannel>>> _pool;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    public RabbitChannelBuilder(IRabbitConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
        _pool = new ConcurrentDictionary<ChannelBuilderConnectionSettings, Lazy<Task<IChannel>>>();
    }

    public async Task<IChannel> Build(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        try
        {
            var connection = await _connectionFactory.CreateConnection
            (
                channelBuilderSettings.ServiceTypeConnectionSettings,
                cancellationToken
            ).ConfigureAwait(false);

            var settings = new ChannelBuilderConnectionSettings(channelBuilderSettings, connection);

            var cached = _pool.GetOrAdd
            (
                settings,
                key => new Lazy<Task<IChannel>>(() => key.InternalChannelBuilder.Build())
            ).Value;

            return await cached.ConfigureAwait(false);
        }
        catch (AlreadyClosedException)
        {
            await CleanupSelfAndDownstream(channelBuilderSettings, cancellationToken).ConfigureAwait(false);
            throw;
        }
    }

    public async Task<IConnection?> GetChannelConnection(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        return await _connectionFactory.GetConnection(channelBuilderSettings.ServiceTypeConnectionSettings, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanupChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.GetConnection
        (
            channelBuilderSettings.ServiceTypeConnectionSettings,
            cancellationToken
        ).ConfigureAwait(false);

        if (connection == null)
        {
            return;
        }

        var key = new ChannelBuilderConnectionSettings(channelBuilderSettings, connection);

        if (!_pool.TryRemove(key, out var lazyChannel))
        {
            return;
        }

        await CleanupLazyChannel(lazyChannel, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task CleanupSelfAndDownstream(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        await CleanupChannel(channelBuilderSettings, cancellationToken)
            .ConfigureAwait(false);

        await _connectionFactory
            .CleanupSelfAndDownstream(channelBuilderSettings.ServiceTypeConnectionSettings, cancellationToken)
            .ConfigureAwait(false);
    }

    private static async Task CleanupLazyChannel(Lazy<Task<IChannel>> lazyChannel, CancellationToken cancellationToken)
    {
        if (!lazyChannel.IsValueCreated)
        {
            return;
        }

        var channel = await lazyChannel.Value.ConfigureAwait(false);

        if (channel.IsOpen)
        {
            await channel.CloseAsync(cancellationToken).ConfigureAwait(false);
        }

        await channel.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            await _semaphore.WaitAsync().ConfigureAwait(false);

            foreach (var lazyChannel in _pool.Values)
            {
                await CleanupLazyChannel(lazyChannel, CancellationToken.None).ConfigureAwait(false);
            }

            await _connectionFactory.DisposeAsync().ConfigureAwait(false);
        }
        finally
        {
            _semaphore.Release();
            _semaphore.Dispose();
        }
    }
}