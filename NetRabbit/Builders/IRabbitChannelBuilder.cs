using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Builders;

internal interface IRabbitChannelBuilder : IAsyncDisposable
{
    Task<IChannel> Build(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken);
    Task<IConnection?> GetChannelConnection(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken);
    Task CleanupChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken);
    Task CleanupSelfAndDownstream(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken);
}