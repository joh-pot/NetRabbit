using System.Collections.Generic;

namespace NetRabbit.Models;

public class BasicMessageProperties
{
    public string? ContentType { get; init; }
    public Dictionary<string, object>? Headers { get; init; }
    public string? ContentEncoding { get; init; }
    public string? MessageId { get; init; }
    public string? AppId { get; init; }
    public int DeliveryMode { get; init; }
}