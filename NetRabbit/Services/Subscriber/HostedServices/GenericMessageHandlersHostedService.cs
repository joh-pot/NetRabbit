using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NetRabbit.Settings;
using NetRabbit.Subscribers;

namespace NetRabbit.Services.Subscriber.HostedServices;

internal sealed class GenericMessageHandlersHostedService<T> : IHostedService
{
    private readonly IEnumerable<IMessageHandlerAsync<T>> _handlers;
    private readonly ISubscriberService<IMessageHandlerAsync<T>> _subscriberService;
    private CancellationTokenSource? _cts;

    public GenericMessageHandlersHostedService
    (
        IEnumerable<IMessageHandlerAsync<T>> handlersOfT,
        ISubscriberService<IMessageHandlerAsync<T>> subscriberService
    )
    {
        _handlers = handlersOfT;
        _subscriberService = subscriberService;
    }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        foreach (var handler in _handlers)
        {
            try
            {
                var subscriberConfigs = handler.GetSubscriberSettings();

                var connectionSettings = new ConnectionSettings(await handler.GetBasicConnectionSettingsAsync(cancellationToken).ConfigureAwait(false));

                foreach (var config in subscriberConfigs)
                {
                    var _ = _subscriberService?.StartAsync
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
        }
        _cts?.Dispose();

        await _subscriberService.DisposeAsync().ConfigureAwait(false)  ;
    }
}