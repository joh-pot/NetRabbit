using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Builders;
using NetRabbit.Models;
using NetRabbit.Settings;

namespace NetRabbit.Services;

public interface IChannelProbe
{
    Task<bool> IsChannelOpen
    (
        IBasicConnectionSettings connectionSettings,
        string exchange,
        bool confirmSelect,
        CancellationToken cancellationToken
    );

    Task<bool> IsChannelOpen
    (
        string exchange,
        bool confirmSelect,
        CancellationToken cancellationToken
    );
}

internal class ChannelProbe : IChannelProbe
{
    private readonly IRabbitChannelFactory _rabbitChannelFactory;
    private readonly IBasicConnectionSettings _basicConnectionSettings;

    public ChannelProbe
    (
        IRabbitChannelFactory rabbitChannelFactory,
        IBasicConnectionSettings basicConnectionSettings
    )
    {
        _rabbitChannelFactory = rabbitChannelFactory;
        _basicConnectionSettings = basicConnectionSettings;
    }
    public async Task<bool> IsChannelOpen
    (
        IBasicConnectionSettings connectionSettings,
        string exchange,
        bool confirmSelect,
        CancellationToken cancellationToken
    )
    {
        var channelBuilderSettings = GetChannelBuilderSettings
        (
            connectionSettings,
            exchange,
            confirmSelect
        );

        var channel = await _rabbitChannelFactory.CreateChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        return channel.IsOpen;
    }

    public async Task<bool> IsChannelOpen(string exchange, bool confirmSelect, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(_basicConnectionSettings.ConnectionString))
        {
            throw new Exception
            (
                "Make sure connection is initialized on app startup or use different overload"
            );
        }

        var channelBuilderSettings = GetChannelBuilderSettings
        (
            _basicConnectionSettings,
            exchange,
            confirmSelect
        );

        var channel = await _rabbitChannelFactory.CreateChannel(channelBuilderSettings, cancellationToken).ConfigureAwait(false);

        return channel.IsOpen;
    }

    private static ChannelBuilderSettings GetChannelBuilderSettings
    (
        IBasicConnectionSettings connectionSettings,
        string exchange,
        bool confirmSelect
    )
    {
        var stcs = new ServiceTypeConnectionSettings
        (
            ServiceType.Publisher,
            new ConnectionSettings(connectionSettings)
        );

        return new ChannelBuilderSettings(stcs, exchange, confirmSelect);
    }

}