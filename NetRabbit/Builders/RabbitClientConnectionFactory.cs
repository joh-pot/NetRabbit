using System.Collections.Concurrent;
using System.Collections.Generic;
using NetRabbit.Models;

namespace NetRabbit.Builders;

internal sealed class RabbitClientConnectionFactory : IRabbitClientConnectionFactory
{
    private readonly ConcurrentDictionary<ServiceTypeConnectionSettings, CustomConnectionFactory> _pool = new();

    public CustomConnectionFactory CreateConnectionFactory(ServiceTypeConnectionSettings settings)
    {
        if (_pool.TryGetValue(settings, out var customFactory))
            return customFactory;

        var internalBuilder = new InternalConnectionFactoryBuilder(settings);

        customFactory = internalBuilder.Build();

        _pool.TryAdd(settings, customFactory);

        return customFactory;
    }

    public CustomConnectionFactory? GetConnectionFactory(ServiceTypeConnectionSettings settings)
    {
        return _pool.GetValueOrDefault(settings);
    }

    public void CleanupConnectionFactory(ServiceTypeConnectionSettings settings)
    {
        _pool.TryRemove(settings, out _);
    }
}