using System.Text.Json;
using System.Text.Json.Serialization;

namespace Token.RabbitMQEvent;

public class JsonHandlerSerializer : IHandlerSerializer
{
    private readonly JsonSerializerOptions? _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false,
        Converters =
        {
            new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
        }
    };

    public byte[] Serialize<TEvent>(TEvent eventEvent) where TEvent : class
    {
        return JsonSerializer.SerializeToUtf8Bytes(eventEvent, _jsonSerializerOptions);
    }

    public TEvent? Deserialize<TEvent>(ReadOnlyMemory<byte> data) where TEvent : class
    {
        return JsonSerializer.Deserialize<TEvent>(data.Span, _jsonSerializerOptions);
    }

    public object? Deserialize(byte[] data, Type type)
    {
        return JsonSerializer.Deserialize(data, type, _jsonSerializerOptions);
    }

    public byte[] Serialize(object @event)
    {
        return JsonSerializer.SerializeToUtf8Bytes(@event);
    }
}