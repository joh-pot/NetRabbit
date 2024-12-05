using System;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace NetRabbit.Models;

internal sealed class InternalChannelBuilder
{
    public IConnection Connection { get; }
    public bool ConfirmSelect { get; }

    public InternalChannelBuilder(IConnection connection, bool confirmSelect)
    {
        Connection = connection;
        ConfirmSelect = confirmSelect;
    }

    public async Task<IChannel> Build()
    {
        return await Connection.CreateChannelAsync(new CreateChannelOptions(ConfirmSelect, false)).ConfigureAwait(false);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Connection);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InternalChannelBuilder icb)
        {
            return false;
        }

        return icb.Connection.Equals(Connection);
    }
}