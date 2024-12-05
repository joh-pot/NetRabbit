using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Builders;

internal interface IRabbitChannelFactory : IAsyncDisposable
{
    public Task<IChannel> CreateChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken);
    public Task RestoreChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken);
}