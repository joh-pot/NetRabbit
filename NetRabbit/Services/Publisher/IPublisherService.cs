using System.Threading;
using System.Threading.Tasks;
using NetRabbit.Models;
using NetRabbit.Settings;

namespace NetRabbit.Services.Publisher;

public interface IPublisherService
{
    public Task<PublishResponseMessage> PublishAsync<T>
    (
        IBasicConnectionSettings connectionSettings,
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull;

    public Task<PublishResponseMessage> PublishAsync<T>
    (
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull;

    public Task<PublishResponseMessage> PublishConfirmAsync<T>
    (
        IBasicConnectionSettings connectionSettings,
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull;

    public Task<PublishResponseMessage> PublishConfirmAsync<T>
    (
        in PublisherBrokeredMessage<T> message,
        string exchange,
        string? routingKey = default,
        CancellationToken cancellationToken = default
    ) where T : notnull;
}