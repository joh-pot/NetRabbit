using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Services;
using NetRabbit.Settings;

namespace NetRabbit.Subscribers
{
    public abstract class SynchronizedMessageHandler<T> : IDisposable
    {
        public abstract IEnumerable<SubscriberSettings> GetSubscriberSettings();
        public abstract Task<IBasicConnectionSettings> GetBasicConnectionSettingsAsync(CancellationToken cancellationToken = default);
        public abstract Task<bool> ProcessAsync(T? body, BasicMessageProperties? messageProperties, CancellationToken cancellationToken = default);
        public abstract IEnumerable<int?> GetKeys(T? body);
        private readonly AsyncCache<int, ulong> _currentlyProcessing = new();
        private readonly SemaphoreSlim _semaphore = new(1, 1);

        private async Task<bool> ProcessSerially(T? body, BasicMessageProperties? messageProperties, int key, CancellationToken cancellationToken)
        {
            try
            {
                await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);

                while (!_currentlyProcessing.TryAdd(key))
                {
                    await _currentlyProcessing.ContainsKeyWaitAsync(key, cancellationToken).ConfigureAwait(false);
                }

                return await ProcessAsync(body, messageProperties, cancellationToken).ConfigureAwait(false);

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
            T? body,
            BasicMessageProperties? messageProperties,
            CancellationToken cancellationToken = default
        )
        {
            var keys = GetKeys(body)
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

                    return await ProcessSerially(body, messageProperties, key.Value, cancellationToken).ConfigureAwait(false);
                }

                return await ProcessAsync(body, messageProperties, cancellationToken).ConfigureAwait(false);

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

        public virtual ISerializer GetSerializer()
        {
            return SystemTextJsonSerializer.Instance;
        }

        public virtual bool PopulateBasicMessageProperties()
        {
            return true;
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
