using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace NetRabbit;

internal static class RabbitResponder
{
    public static async Task RespondLockFree(bool success, IChannel channel, ulong tag, bool requeue, CancellationToken cancellationToken)
    {
        if (success)
            await channel.BasicAckAsync(tag, false, cancellationToken).ConfigureAwait(false);
        else
            await channel.BasicNackAsync(tag, false, requeue, cancellationToken).ConfigureAwait(false);
    }
}