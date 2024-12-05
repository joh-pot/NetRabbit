using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Builders;

internal sealed class RabbitConnectionFactory : IRabbitConnectionFactory
{
    private readonly IRabbitConnectionBuilder _connectionBuilder;

    public RabbitConnectionFactory(IRabbitConnectionBuilder connectionBuilder)
    {
        _connectionBuilder = connectionBuilder;
    }

    public async Task<IConnection> CreateConnection(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken)
    {
        var connection = await _connectionBuilder.Build(serviceTypeConnectionSettings, cancellationToken).ConfigureAwait(false);

        if (connection.IsOpen)
        {
            return connection;
        }

        await CleanupSelfAndDownstream(serviceTypeConnectionSettings, cancellationToken).ConfigureAwait(false);
        throw new Exception("Connection established but is not open");
    }

    public async Task<IConnection?> GetConnection(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken)
    {
        return await _connectionBuilder.GetConnection(settings, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanupConnection(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken)
    {
        await _connectionBuilder.CleanupConnection(settings, cancellationToken).ConfigureAwait(false);
    }

    public async Task CleanupSelfAndDownstream(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken)
    {
        await CleanupConnection(settings, cancellationToken).ConfigureAwait(false);

        await _connectionBuilder.CleanupSelfAndDownstream(settings, cancellationToken).ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _connectionBuilder.DisposeAsync();
    }
}