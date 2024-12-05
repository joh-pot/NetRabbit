using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Settings;
using NetRabbit.Subscribers;

namespace NetRabbit.Services.Subscriber.Fake;

internal class FakeMessageHandler : IMessageHandlerAsync
{
    public IEnumerable<SubscriberSettings> GetSubscriberSettings()
    {
        throw new NotImplementedException();
    }

    public Task<IBasicConnectionSettings> GetBasicConnectionSettingsAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ProcessAsync(SubscriberBrokeredMessage message, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true);
    }
}