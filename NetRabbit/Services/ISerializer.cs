using System;

namespace NetRabbit.Services;

public interface ISerializer
{
    byte[] SerializeToUtf8Bytes(object obj);
    T? Deserialize<T>(ReadOnlySpan<byte> span);
    public string ContentType()
    {
        return "application/json";
    }
}