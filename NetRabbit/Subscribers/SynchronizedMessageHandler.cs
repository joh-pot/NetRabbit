using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Settings;

namespace NetRabbit.Subscribers;

public abstract class SynchronizedMessageHandler : IDisposable
{
    public abstract IEnumerable<SubscriberSettings> GetSubscriberSettings();
    public abstract Task<IBasicConnectionSettings> GetBasicConnectionSettingsAsync(CancellationToken cancellationToken = default);
    public abstract Task<bool> ProcessAsync(object? body, SubscriberBrokeredMessage payload, CancellationToken cancellationToken = default);
    public abstract object? GetBody(SubscriberBrokeredMessage message);
    /// <summary>
    /// If your desired property is not an int, call GetHashCode() on it
    /// </summary>
    /// <returns></returns>
    public abstract IEnumerable<int?> GetKeys(object? body, SubscriberBrokeredMessage message);

    private readonly AsyncCache<int, ulong> _currentlyProcessing = new();
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    private async Task<bool> ProcessSerially(object? body, SubscriberBrokeredMessage message, int key, CancellationToken cancellationToken)
    {
        try
        {
            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

            while (!_currentlyProcessing.TryAdd(key))
            {
                await _currentlyProcessing.ContainsKeyWaitAsync(key, cancellationToken).ConfigureAwait(false);
            }

            return await ProcessAsync(body, message, cancellationToken).ConfigureAwait(false);

        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            _currentlyProcessing.TryRemove(key);
            _semaphore.Release();
        }
    }

    internal async Task<bool> ProcessAsyncInternal
    (
        SubscriberBrokeredMessage message,
        ulong deliveryTag,
        CancellationToken cancellationToken = default
    )
    {
        var body = GetBody(message);

        var keys = GetKeys(body, message)
                   .Where(key => key is not null)
                   .Distinct()
                   .ToList();

        try
        {
            foreach (var key in keys)
            {
                //current key is not being processed in different thread
                if (_currentlyProcessing.TryAdd(key!.Value))
                {
                    continue;
                }

                return await ProcessSerially(body, message, key.Value, cancellationToken).ConfigureAwait(false);
            }

            return await ProcessAsync(body, message, cancellationToken).ConfigureAwait(false);

        }
        catch (Exception)
        {
            return false;
        }
        finally
        {
            foreach (var key in keys)
            {
                if (key is null)
                {
                    continue;
                }

                _currentlyProcessing.TryRemove(key.Value);
            }
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
    }
}