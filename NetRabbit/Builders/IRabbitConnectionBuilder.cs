using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Builders;

internal interface IRabbitConnectionBuilder : IAsyncDisposable
{
    Task<IConnection> Build(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken);
    Task CleanupConnection(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken);
    Task CleanupSelfAndDownstream(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken);
    Task<IConnection?> GetConnection(ServiceTypeConnectionSettings serviceTypeConnectionSettings, CancellationToken cancellationToken);
}