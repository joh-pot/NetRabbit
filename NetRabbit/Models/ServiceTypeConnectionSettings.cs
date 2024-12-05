using System;
using NetRabbit.Settings;

namespace NetRabbit.Models;

internal sealed class ServiceTypeConnectionSettings
{
    public ServiceType Service { get; }
    public IConnectionSettings ConnectionSettings { get; }

    public ServiceTypeConnectionSettings(ServiceType service, IConnectionSettings connectionSettings)
    {
        Service = service;
        ConnectionSettings = connectionSettings;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Service, ConnectionSettings);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not ServiceTypeConnectionSettings stcs)
            return false;

        return Service == stcs.Service &&
            stcs.ConnectionSettings.Equals(ConnectionSettings);
    }
}