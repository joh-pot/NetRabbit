using System;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NetRabbit;

public static class ModelExtensions
{
    public static Task OnShutdown(this IChannel channel, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(channel);

        cancellationToken.ThrowIfCancellationRequested();

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        channel.ChannelShutdownAsync += (_, _) =>
        {
            tcs.TrySetResult(); return Task.CompletedTask;
        };

        return tcs.Task;
    }

    public static Task<ulong> OnAckOrNack(this IChannel model, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(model);

        cancellationToken.ThrowIfCancellationRequested();

        var tcs = new TaskCompletionSource<ulong>(TaskCreationOptions.RunContinuationsAsynchronously);

        Task AckCallback(object? sender, BasicAckEventArgs args)
        {
            tcs.SetResult(args.DeliveryTag);
            return Task.CompletedTask;
        }

        Task NackCallback(object? sender, BasicNackEventArgs args)
        {
            tcs.SetException(new Exception("Rabbitmq could not accept message"));
            return Task.CompletedTask;
        }

        model.BasicAcksAsync += AckCallback;

        model.BasicNacksAsync += NackCallback;

        return tcs.Task.ContinueWith
        (
            (task, state) =>
            {
                var stateModel = (IChannel?)state;
                stateModel!.BasicAcksAsync -= AckCallback;
                stateModel.BasicNacksAsync -= NackCallback;
                return task.Result;
            },
            model,
            TaskScheduler.Default
        );
    }
}