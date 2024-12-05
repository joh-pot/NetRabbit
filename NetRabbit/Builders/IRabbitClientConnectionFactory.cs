using NetRabbit.Models;

namespace NetRabbit.Builders;

internal interface IRabbitClientConnectionFactory
{
    CustomConnectionFactory CreateConnectionFactory(ServiceTypeConnectionSettings settings);
    CustomConnectionFactory? GetConnectionFactory(ServiceTypeConnectionSettings settings);
    void CleanupConnectionFactory(ServiceTypeConnectionSettings settings);
}