using System;
using NetRabbit.Settings;

namespace NetRabbit.Models;

internal sealed class ChannelBuilderSettings
{
    public ServiceTypeConnectionSettings ServiceTypeConnectionSettings { get; }
    public string Owner { get; }
    public bool ConfirmSelect { get; }

    public ChannelBuilderSettings
    (
        ServiceTypeConnectionSettings serviceTypeConnectionSettings,
        string owner,
        bool confirmSelect
    )
    {
        ServiceTypeConnectionSettings = serviceTypeConnectionSettings;
        Owner = owner;
        ConfirmSelect = confirmSelect;
    }

    public ChannelBuilderSettings
    (
        SubscriberSettings subscriberSettings,
        IConnectionSettings connectionSettings
    ) : this
    (
        new ServiceTypeConnectionSettings(ServiceType.Subscriber, connectionSettings),
        subscriberSettings.QueueName + "_" + Guid.NewGuid().ToString(),
        false
    ) { }

    public override int GetHashCode()
    {
        return HashCode.Combine(ServiceTypeConnectionSettings, Owner, ConfirmSelect);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ChannelBuilderSettings cbs)
        {
            return false;
        }

        return ServiceTypeConnectionSettings.Equals(cbs.ServiceTypeConnectionSettings) &&
            Owner.Equals(cbs.Owner, StringComparison.OrdinalIgnoreCase) &&
            ConfirmSelect.Equals(cbs.ConfirmSelect);
    }
}