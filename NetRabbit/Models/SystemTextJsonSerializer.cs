using System;
using System.Text.Json;
using NetRabbit.Services;

namespace NetRabbit.Models;

internal class SystemTextJsonSerializer : ISerializer
{
    public static readonly ISerializer Instance = new SystemTextJsonSerializer();

    public byte[] SerializeToUtf8Bytes(object obj)
    {
        return JsonSerializer.SerializeToUtf8Bytes(obj, JsonSerializerOptionsInternal.Instance);
    }

    public T? Deserialize<T>(ReadOnlySpan<byte> span)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(span, JsonSerializerOptionsInternal.Instance);
        }
        catch (Exception )
        {
            return default;
        }
    }
}