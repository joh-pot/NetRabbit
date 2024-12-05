using NetRabbit.Settings;
using RabbitMQ.Client;

namespace NetRabbit.Models;

internal readonly record struct ConsumerInfo(bool Success, IChannel Channel, SubscriberSettings SubscriberSettings, ulong DeliveryTag);