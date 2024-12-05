using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Builders;

internal interface IRabbitConnectionFactory : IAsyncDisposable
{
    Task<IConnection> CreateConnection(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken);
    Task<IConnection?> GetConnection(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken);
    Task CleanupSelfAndDownstream(ServiceTypeConnectionSettings settings, CancellationToken cancellationToken);
}