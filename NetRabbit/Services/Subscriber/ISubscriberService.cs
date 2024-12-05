using System;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Settings;

namespace NetRabbit.Services.Subscriber;
internal interface ISubscriberService<in THandler> : IAsyncDisposable
{
    Task<IRabbitConsumer> StartAsync
    (
        SubscriberSettings subscriberSettings,
        IConnectionSettings connectionSettings,
        THandler messageHandler,
        CancellationToken cancellationToken = default
    );
}