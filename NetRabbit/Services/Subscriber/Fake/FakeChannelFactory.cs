using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Builders;
using NetRabbit.Models;
using RabbitMQ.Client;

namespace NetRabbit.Services.Subscriber.Fake;

internal class FakeChannelFactory : IRabbitChannelFactory
{
    private static readonly FakeModel FakeModel = new();
    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public Task<IChannel> CreateChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        return Task.FromResult<IChannel>(FakeModel);
    }

    public Task RestoreChannel(ChannelBuilderSettings channelBuilderSettings, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public static FakeModel GetModel()
    {
        return FakeModel;
    }
}