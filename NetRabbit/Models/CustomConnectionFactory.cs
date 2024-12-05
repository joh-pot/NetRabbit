using System;
using RabbitMQ.Client;

namespace NetRabbit.Models;

internal sealed class CustomConnectionFactory
{
    private readonly ServiceTypeConnectionSettings _settings;
    public IConnectionFactory ConnectionFactory { get; }

    public CustomConnectionFactory(ServiceTypeConnectionSettings settings, IConnectionFactory connectionFactory)
    {
        _settings = settings;
        ConnectionFactory = connectionFactory;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_settings);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not CustomConnectionFactory cf)
            return false;
        return cf._settings.Equals(_settings);
    }
}