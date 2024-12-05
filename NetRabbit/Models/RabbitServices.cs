using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using NetRabbit.Services;
using NetRabbit.Services.Publisher;
using NetRabbit.Services.Subscriber;
using NetRabbit.Services.Subscriber.ConsumerServices;
using NetRabbit.Services.Subscriber.HostedServices;
using NetRabbit.Subscribers;

namespace NetRabbit.Models;

public sealed class RabbitServices
{
    public IServiceCollection ServiceCollection { get; }

    public RabbitServices(IServiceCollection serviceCollection)
    {
        ServiceCollection = serviceCollection;
    }

    [Browsable(false)]
    [CompilerGenerated]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerHidden]
    public RabbitServices AddMessageHandlerInternal<T>() where T : IMessageHandlerAsync
    {
        ServiceCollection.AddHostedService<MessageHandlersHostedService>()
                         .Configure<HostOptions>(opts =>
                         {
                             opts.ShutdownTimeout = TimeSpan.FromSeconds(15);
                             opts.ServicesStartConcurrently = true;
                             opts.ServicesStopConcurrently = true;
                         });
        ServiceCollection.AddSingleton<ISubscriberService<IMessageHandlerAsync>, MessageHandlerSubscriberService>();
        ServiceCollection.AddSingleton<IConsumerService<IMessageHandlerAsync>, MessageHandlerConsumerService>();
        ServiceCollection.AddSingleton(typeof(IMessageHandlerAsync), typeof(T));
        return this;
    }

    /// <summary>
    /// This method should not be called by user code. Use the generated extension methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    [Browsable(false)]
    [CompilerGenerated]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerHidden]
    public RabbitServices AddMessageHandlerInternal<T, TPayload>() where T : IMessageHandlerAsync<TPayload>
    {
        ServiceCollection.AddHostedService<GenericMessageHandlersHostedService<TPayload>>()
                         .Configure<HostOptions>(opts =>
                         {
                             opts.ShutdownTimeout = TimeSpan.FromSeconds(15);
                             opts.ServicesStartConcurrently = true;
                             opts.ServicesStopConcurrently = true;
                         });
        ServiceCollection.AddSingleton<ISubscriberService<IMessageHandlerAsync<TPayload>>, MessageHandlerSubscriberService<TPayload>>();
        ServiceCollection.AddSingleton<IConsumerService<IMessageHandlerAsync<TPayload>>, MessageHandlerConsumerService<TPayload>>();
        ServiceCollection.AddSingleton(typeof(IMessageHandlerAsync<TPayload>), typeof(T));
        return this;
    }

    public RabbitServices AddPublisherService()
    {
        ServiceCollection.TryAddSingleton<IPublisherService, PublisherService>();
        ServiceCollection.TryAddSingleton<IChannelPreWarmer, ChannelPreWarmer>();
        return this;
    }

    /// <summary>
    /// This method should not be called by user code. Use the generated extension methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Browsable(false)]
    [CompilerGenerated]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerHidden]
    public RabbitServices AddSynchronizedMessageHandlerInternal<T>() where T : SynchronizedMessageHandler
    {
        ServiceCollection.AddHostedService<SynchronizedMessageHandlersHostedService>()
                         .Configure<HostOptions>(opts =>
                         {
                             opts.ShutdownTimeout = TimeSpan.FromSeconds(15);
                             opts.ServicesStartConcurrently = true;
                             opts.ServicesStopConcurrently = true;
                         });
        ServiceCollection.AddSingleton<ISubscriberService<SynchronizedMessageHandler>, SynchronizedMessageHandlerSubscriberService>();
        ServiceCollection.AddSingleton<IConsumerService<SynchronizedMessageHandler>, SynchronizedMessageHandlerConsumerService>();
        ServiceCollection.AddSingleton(typeof(SynchronizedMessageHandler), typeof(T));
        return this;
    }

    /// <summary>
    /// This method should not be called by user code. Use the generated extension methods.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TPayload"></typeparam>
    [Browsable(false)]
    [CompilerGenerated]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DebuggerHidden]
    public RabbitServices AddSynchronizedMessageHandlerInternal<T, TPayload>() where T : SynchronizedMessageHandler<TPayload>
    {
        ServiceCollection.AddHostedService<SynchronizedMessageHandlersHostedService<TPayload>>()
                         .Configure<HostOptions>(opts =>
                         {
                             opts.ShutdownTimeout = TimeSpan.FromSeconds(15);
                             opts.ServicesStartConcurrently = true;
                             opts.ServicesStopConcurrently = true;
                         });
        ServiceCollection.AddSingleton<ISubscriberService<SynchronizedMessageHandler<TPayload>>, SynchronizedMessageHandlerSubscriberService<TPayload>>();
        ServiceCollection.AddSingleton<IConsumerService<SynchronizedMessageHandler<TPayload>>, SynchronizedMessageHandlerConsumerService<TPayload>>();
        ServiceCollection.AddSingleton(typeof(SynchronizedMessageHandler<TPayload>), typeof(T));
        return this;
    }

}