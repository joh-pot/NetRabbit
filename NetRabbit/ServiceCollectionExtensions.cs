using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using NetRabbit.Builders;
using NetRabbit.Models;
using NetRabbit.Services;
using NetRabbit.Settings;

namespace NetRabbit;

public static class ServiceCollectionExtensions
{
    public static RabbitServices AddRabbit(this IServiceCollection services)
    {
        services.TryAddSingleton<IRabbitClientConnectionFactory, RabbitClientConnectionFactory>();
        services.TryAddSingleton<IRabbitConnectionBuilder, RabbitConnectionBuilder>();
        services.TryAddSingleton<IRabbitConnectionFactory, RabbitConnectionFactory>();
        services.TryAddSingleton<IRabbitChannelBuilder, RabbitChannelBuilder>();
        services.TryAddSingleton<IRabbitChannelFactory, RabbitChannelFactory>();
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IBasicConnectionSettings, BasicConnectionSettings>());
        services.TryAddSingleton<IChannelProbe, ChannelProbe>();
        services.TryAddSingleton<IConnectionProbe, ConnectionProbe>();

        return new RabbitServices(services);
    }
}