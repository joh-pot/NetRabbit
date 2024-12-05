using System;
using RabbitMQ.Client;

namespace NetRabbit.Models;

internal sealed class ChannelBuilderConnectionSettings
{
    public ChannelBuilderSettings ChannelBuilderSettings { get; }
    public IConnection Connection { get; }
    public InternalChannelBuilder InternalChannelBuilder { get; }

    public ChannelBuilderConnectionSettings(ChannelBuilderSettings channelBuilderSettings, IConnection connection)
    {
        ChannelBuilderSettings = channelBuilderSettings;
        Connection = connection;
        InternalChannelBuilder = new InternalChannelBuilder(connection, channelBuilderSettings.ConfirmSelect);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(ChannelBuilderSettings, Connection);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ChannelBuilderConnectionSettings cbcs)
            return false;

        return cbcs.ChannelBuilderSettings.Equals(ChannelBuilderSettings) &&
            cbcs.Connection.Equals(Connection);
    }
}