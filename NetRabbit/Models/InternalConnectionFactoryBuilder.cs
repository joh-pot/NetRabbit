using System;
using RabbitMQ.Client;

namespace NetRabbit.Models;

internal sealed class InternalConnectionFactoryBuilder
{
    private readonly ServiceTypeConnectionSettings _settings;

    public InternalConnectionFactoryBuilder(ServiceTypeConnectionSettings settings)
    {
        _settings = settings;
    }

    public CustomConnectionFactory Build()
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(_settings.ConnectionSettings.ConnectionString ?? throw new ArgumentNullException()),
            UserName = _settings.ConnectionSettings.Username!,
            Password = _settings.ConnectionSettings.Password!,
            AutomaticRecoveryEnabled = false,
            TopologyRecoveryEnabled = false,
            RequestedHeartbeat = TimeSpan.FromSeconds(_settings.ConnectionSettings.RequestedHeartBeat),
            VirtualHost = _settings.ConnectionSettings.VirtualHost!,
            ClientProvidedName = "NetRabbit",
        };
        return new CustomConnectionFactory(_settings, factory);
    }
    public override int GetHashCode()
    {
        return HashCode.Combine(_settings);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InternalConnectionFactoryBuilder cf)
            return false;
        return cf._settings.Equals(_settings);
    }
}