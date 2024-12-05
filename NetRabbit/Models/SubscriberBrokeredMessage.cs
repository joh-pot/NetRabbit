using System;
using System.Text;
using System.Text.Json;
using NetRabbit.Services;

namespace NetRabbit.Models;

public readonly record struct SubscriberBrokeredMessage(in ReadOnlyMemory<byte> Body, BasicMessageProperties BasicProperties)
{
    /// <summary>
    /// Uses System.Text.Json.JsonSerializer.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>T?</returns>
    public T? JsonDeserialize<T>(JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(Body.Span, options);
    }

    public string BodyAsString()
    {
        return Encoding.UTF8.GetString(Body.Span);
    }

    /// <summary>
    /// Implement ISerializer and use your app's default json serializer
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="serializer"></param>
    /// <returns>T</returns>
    public T? Deserialize<T>(ISerializer serializer)
    {
        return serializer.Deserialize<T>(Body.Span);
    }
}