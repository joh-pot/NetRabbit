using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NetRabbit.Settings;
using NetRabbit.Subscribers;

namespace NetRabbit.Services.Subscriber.HostedServices
{
    internal class SynchronizedMessageHandlersHostedService : IHostedService
    {
        private readonly IEnumerable<SynchronizedMessageHandler> _syncHandlers;
        private readonly ISubscriberService<SynchronizedMessageHandler>? _subscriberService;
        private CancellationTokenSource? _cts;
        public SynchronizedMessageHandlersHostedService
        (
            IEnumerable<SynchronizedMessageHandler> syncHandlers,
            ISubscriberService<SynchronizedMessageHandler>? subscriberService
        )
        {
            _syncHandlers = syncHandlers;
            _subscriberService = subscriberService;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            foreach (var handler in _syncHandlers)
            {
                try
                {
                    var subscriberConfigs = handler.GetSubscriberSettings();

                    var connectionSettings = new ConnectionSettings(await handler.GetBasicConnectionSettingsAsync(cancellationToken).ConfigureAwait(false));

                    foreach (var config in subscriberConfigs)
                    {
                        _ = _subscriberService?.StartAsync
                        (
                            config,
                            connectionSettings,
                            handler,
                            _cts.Token
                        );
                    }
                }
                catch (Exception)
                {
                    //suppress so that other handlers can actually startup and run
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_cts is not null)
            {
                await _cts.CancelAsync().ConfigureAwait(false);
                _cts.Dispose();
            }
            if(_subscriberService is not null)
            {
                await _subscriberService.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}
