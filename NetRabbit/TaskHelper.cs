using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetRabbit;

internal static class TaskHelper
{
    public static Task<T> Run<T>(Func<object?, T> func, object state, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew
        (
            func,
            state,
            cancellationToken,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default
        );
    }

    public static Task Run(Func<object?, Task> func, object state, CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew
        (
            func,
            state,
            cancellationToken,
            TaskCreationOptions.DenyChildAttach,
            TaskScheduler.Default
        );
    }
}