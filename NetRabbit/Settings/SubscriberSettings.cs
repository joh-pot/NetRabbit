using System.Collections.Generic;

namespace NetRabbit.Settings;

public sealed class SubscriberSettings
{
    public string? MessageHandlerTag { get; }
    public string? QueueName { get; }
    public ushort PrefetchCount { get; }
    public Dictionary<string,object?>? BindingArguments { get; set; }
    public bool RequeueOnNack { get; set; } = true;

    public SubscriberSettings(string messageHandlerTag, string queueName, ushort prefetchCount)
    {
        MessageHandlerTag = messageHandlerTag;
        QueueName = queueName;
        PrefetchCount = prefetchCount;
    }
}