using System.Text.Json;
using System.Text.Json.Serialization;

namespace NetRabbit.Models;

internal static class JsonSerializerOptionsInternal
{
    public static JsonSerializerOptions Instance = Build();

    private static JsonSerializerOptions Build()
    {
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        options.Converters.Add(new JsonStringEnumConverter());
        return options;
    }
}