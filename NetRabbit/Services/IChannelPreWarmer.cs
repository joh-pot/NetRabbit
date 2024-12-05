using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Builders;
using NetRabbit.Models;
using NetRabbit.Settings;

namespace NetRabbit.Services;

public interface IChannelPreWarmer
{
    Task<bool> WarmUpForPublish(IBasicConnectionSettings channelBuilderSettings, string exchangeName, bool confirm, CancellationToken cancellationToken);
}

internal sealed class ChannelPreWarmer : IChannelPreWarmer
{
    private readonly IRabbitChannelBuilder _channelBuilder;

    public ChannelPreWarmer(IRabbitChannelBuilder channelBuilder)
    {
        _channelBuilder = channelBuilder;
    }
    public async Task<bool> WarmUpForPublish(IBasicConnectionSettings connectionSettings, string exchangeName, bool confirm, CancellationToken cancellationToken)
    {
        var channel = await _channelBuilder.Build
        (
            new ChannelBuilderSettings
            (
                new ServiceTypeConnectionSettings
                (
                    ServiceType.Publisher,
                    new ConnectionSettings(connectionSettings)
                ),
                exchangeName,
                confirm
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return channel.IsOpen;
    }
}