using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Services;
using NetRabbit.Settings;

namespace NetRabbit.Subscribers;

public interface IMessageHandlerAsync
{
    IEnumerable<SubscriberSettings> GetSubscriberSettings();
    Task<IBasicConnectionSettings> GetBasicConnectionSettingsAsync(CancellationToken cancellationToken = default);
    Task<bool> ProcessAsync(SubscriberBrokeredMessage message, CancellationToken cancellationToken = default);
}

public interface IMessageHandlerAsync<in TPayload>
{
    IEnumerable<SubscriberSettings> GetSubscriberSettings();
    Task<IBasicConnectionSettings> GetBasicConnectionSettingsAsync(CancellationToken cancellationToken);
    Task<bool> ProcessAsync(TPayload? payload, BasicMessageProperties? messageProperties, CancellationToken cancellationToken);
    ISerializer GetSerializer()
    {
        return SystemTextJsonSerializer.Instance;
    }
    bool PopulateBasicMessageProperties()
    {
        return true;
    }
}