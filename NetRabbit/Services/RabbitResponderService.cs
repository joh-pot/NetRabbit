using System;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using NetRabbit.Models;

namespace NetRabbit.Services;

internal class RabbitResponderService
{
    private readonly Channel<ConsumerInfo> _pipe = Channel.CreateUnbounded<ConsumerInfo>
    (
        new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        }
    );

    public Task StartListening(CancellationToken cancellationToken)
    {
        return Task.Run(() => ReadPipelineRespondAsync(cancellationToken), cancellationToken);
    }

    public ValueTask EnqueueToRespond(in ConsumerInfo info, CancellationToken cancellationToken)
    {
        return _pipe.Writer.WriteAsync(info, cancellationToken);
    }

    private async Task ReadPipelineRespondAsync(CancellationToken cancellationToken)
    {
        await foreach (var info in _pipe.Reader.ReadAllAsync(cancellationToken).ConfigureAwait(false))
        {
            try
            {
                //we can (n)ack without having to ensure
                //thread safety as this is a single reader
                await RabbitResponder.RespondLockFree
                (
                    info.Success,
                    info.Channel,
                    info.DeliveryTag,
                    info.SubscriberSettings.RequeueOnNack,
                    cancellationToken
                ).ConfigureAwait(false);
            }
            catch (Exception)
            {
                //swallow, don't have unobserved exceptions on background thread
                //this will only throw if underlying connection has closed and heartbeat has not detected it
                //rabbit client will restore channels when connection is recovered.
            }
        }
    }
}