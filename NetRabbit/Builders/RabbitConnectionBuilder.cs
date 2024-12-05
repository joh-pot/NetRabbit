using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Builders;

internal sealed class RabbitConnectionBuilder : IRabbitConnectionBuilder
{
    private readonly IRabbitClientConnectionFactory _rabbitClientConnectionFactory;

    private readonly ConcurrentDictionary<CustomConnectionFactory, Lazy<Task<IConnection>>> _factoryPool;

    public RabbitConnectionBuilder(IRabbitClientConnectionFactory rabbitClientConnectionFactory)
    {
        _factoryPool = new ConcurrentDictionary<CustomConnectionFactory, Lazy<Task<IConnection>>>();
        _rabbitClientConnectionFactory = rabbitClientConnectionFactory;
    }

    public async Task<IConnection> Build(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken)
    {
        var factory = _rabbitClientConnectionFactory.CreateConnectionFactory(settings);

        return await _factoryPool.GetOrAdd
        (
            factory,
            (key, tkn) => new Lazy<Task<IConnection>>(() => key.ConnectionFactory.CreateConnectionAsync(tkn)),
            cancellationToken
        )
        .Value
        .ConfigureAwait(false);
    }

    public async Task CleanupConnection(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken)
    {
        var connectionFactory = _rabbitClientConnectionFactory.GetConnectionFactory(settings);

        if (connectionFactory == null)
            return;

        if (!_factoryPool.TryRemove(connectionFactory, out Lazy<Task<IConnection>>? lazyConnection))
        {
            return;
        }

        if (!lazyConnection.IsValueCreated)
        {
            return;
        }

        await CleanupLazyConnection(lazyConnection, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanupSelfAndDownstream(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken)
    {
        await CleanupConnection(serviceTypeConnectionSettings, cancellationToken).ConfigureAwait(false);

        _rabbitClientConnectionFactory.CleanupConnectionFactory(serviceTypeConnectionSettings);
    }

    public async Task<IConnection?> GetConnection(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var connectionFactory = _rabbitClientConnectionFactory.GetConnectionFactory(settings);

        if (connectionFactory == null)
        {
            return null;
        }

        if (!_factoryPool.TryGetValue(connectionFactory, out Lazy<Task<IConnection>>? lazyConnection))
        {
            return null;
        }

        return lazyConnection.IsValueCreated
            ? await lazyConnection.Value.ConfigureAwait(false)
            : null;
    }

    private static async Task CleanupLazyConnection(Lazy<Task<IConnection>> lazyConnection, CancellationToken cancellationToken)
    {
        if (!lazyConnection.IsValueCreated)
            return;

        var connection = await lazyConnection.Value.ConfigureAwait(false);

        if (connection.IsOpen)
        {
            await connection.CloseAsync(cancellationToken).ConfigureAwait(false);
        }

        await connection.DisposeAsync().ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        foreach (var lazyConnection in _factoryPool.Values)
        {
            await CleanupLazyConnection(lazyConnection, CancellationToken.None).ConfigureAwait(false);
        }
    }
}