using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Builders;
using NetRabbit.Models;
using NetRabbit.Settings;

namespace NetRabbit.Services;

public interface IConnectionProbe
{
    public Task<bool> IsPublisherConnectionOpen(IBasicConnectionSettings connectionSettings, CancellationToken cancellationToken);
    public Task<bool> IsPublisherConnectionOpen(CancellationToken cancellationToken);
    public Task<bool> IsSubscriberConnectionOpen(IBasicConnectionSettings connectionSettings, CancellationToken cancellationToken);
    public Task<bool> IsSubscriberConnectionOpen(CancellationToken cancellationToken);

}

internal sealed class ConnectionProbe : IConnectionProbe
{
    private readonly IRabbitConnectionFactory _connectionFactory;
    private readonly IBasicConnectionSettings _basicConnectionSettings;

    public ConnectionProbe
    (
        IRabbitConnectionFactory connectionFactory,
        IBasicConnectionSettings basicConnectionSettings
    )
    {
        _connectionFactory = connectionFactory;
        _basicConnectionSettings = basicConnectionSettings;
    }
    public async Task<bool> IsPublisherConnectionOpen
    (
        IBasicConnectionSettings basicConnectionSettings,
        CancellationToken cancellationToken
    )
    {
        var connection = await _connectionFactory.CreateConnection
        (
            new ServiceTypeConnectionSettings
            (
                ServiceType.Publisher,
                new ConnectionSettings(basicConnectionSettings)
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return connection.IsOpen;
    }

    public async Task<bool> IsPublisherConnectionOpen(CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnection
        (
            new ServiceTypeConnectionSettings
            (
                ServiceType.Publisher,
                new ConnectionSettings(_basicConnectionSettings)
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return connection.IsOpen;
    }

    public async Task<bool> IsSubscriberConnectionOpen
    (
        IBasicConnectionSettings basicConnectionSettings,
        CancellationToken cancellationToken
    )
    {
        var connection = await _connectionFactory.CreateConnection
        (
            new ServiceTypeConnectionSettings
            (
                ServiceType.Subscriber,
                new ConnectionSettings(basicConnectionSettings)
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return connection.IsOpen;
    }

    public async Task<bool> IsSubscriberConnectionOpen(CancellationToken cancellationToken)
    {
        var connection = await _connectionFactory.CreateConnection
        (
            new ServiceTypeConnectionSettings
            (
                ServiceType.Subscriber,
                new ConnectionSettings(_basicConnectionSettings)
            ),
            cancellationToken
        ).ConfigureAwait(false);

        return connection.IsOpen;
    }
}