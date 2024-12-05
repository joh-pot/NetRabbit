using System;
using System.Text;
using System.Text.Json;
using NetRabbit.Services;
using HeadersDictionary = System.Collections.Generic.Dictionary<string, object>;

namespace NetRabbit.Models;

public readonly record struct PublisherBrokeredMessage<T> where T : notnull
{
    private readonly byte[] _body;
    public string ContentType { get; }

    public HeadersDictionary? Headers { get; }

    public PublisherBrokeredMessage(string payload, HeadersDictionary? headers = null)
    {
        _body = Encoding.UTF8.GetBytes(payload);
        ContentType = "application/text";
        Headers = headers;
    }

    public PublisherBrokeredMessage(byte[] jsonBytes, HeadersDictionary? headers = null)
    {
        _body = jsonBytes;
        ContentType = "application/json";
        Headers = headers;
    }

    /// <summary>
    /// Uses System.Text.Json.JsonSerializer for serialization with default options
    /// <list type="bullet">
    ///<item>DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull</item>
    ///<item>PropertyNameCaseInsensitive = true</item>
    ///<item>PropertyNamingPolicy = JsonNamingPolicy.CamelCase</item>
    ///<item>Converter added = JsonStringEnumConverter</item>
    /// </list>
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="headers"></param>
    public PublisherBrokeredMessage(T payload, HeadersDictionary? headers = null)
    {
        _body = JsonSerializer.SerializeToUtf8Bytes(payload, JsonSerializerOptionsInternal.Instance);
        ContentType = "application/json";
        Headers = headers;
    }

    /// <summary>
    /// Uses System.Text.Json.JsonSerializer for serialization with default options
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="options"></param>
    /// <param name="headers"></param>
    public PublisherBrokeredMessage(T payload, JsonSerializerOptions options, HeadersDictionary? headers = null)
    {
        _body = JsonSerializer.SerializeToUtf8Bytes(payload, options);
        ContentType = "application/json";
        Headers = headers;
    }

    /// <summary>
    /// Implement ISerializer and use your app's default json serializer
    /// </summary>
    /// <param name="payload"></param>
    /// <param name="serializer"></param>
    /// <param name="headers"></param>
    public PublisherBrokeredMessage(T payload, ISerializer serializer, HeadersDictionary? headers = null)
    {
        ArgumentNullException.ThrowIfNull(serializer);
        _body = serializer.SerializeToUtf8Bytes(payload);
        ContentType = serializer.ContentType();
        Headers = headers;
    }

    public byte[] GetBody()
    {
        return _body;
    }
}