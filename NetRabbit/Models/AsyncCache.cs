using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace NetRabbit.Models;

internal sealed class AsyncCache<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, TaskCompletionSource<TValue?>> _cache = new();

    public bool TryAdd(TKey key)
    {
        return _cache.TryAdd
        (
            key,
            new TaskCompletionSource<TValue?>(TaskCreationOptions.RunContinuationsAsynchronously)
        );
    }

    public Task ContainsKeyWaitAsync(TKey key, CancellationToken cancellationToken)
    {
        if (!_cache.TryGetValue(key, out var tcs)) return Task.CompletedTask;

        if (cancellationToken.IsCancellationRequested)
        {
            tcs.TrySetCanceled(cancellationToken);
        }

        return tcs.Task;
    }

    public bool ContainsKey(TKey key)
    {
        return _cache.ContainsKey(key);
    }

    public bool TryRemove(TKey key)
    {
        var removed = _cache.TryRemove(key, out var tcs);

        if (!removed)
        {
            return false;
        }

        tcs?.SetResult(default);

        return true;
    }
}