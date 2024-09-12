namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(TelemetryHardwareProfileCameraTypeConverter))]
public enum TelemetryHardwareProfileCameraType
{
    None,
    Default,
    EyeToy1,
    EyeToy2,
    Usb,
}

file class TelemetryHardwareProfileCameraTypeConverter : JsonConverter {
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.TokenType != JsonToken.String)
        {
            throw new JsonSerializationException($"Cannot convert {reader.TokenType} value to {objectType.Name}");
        }

        try
        {
            string enumText = reader.Value?.ToString()!;

            return enumText switch
            {
                "" => TelemetryHardwareProfileCameraType.None,
                "default" => TelemetryHardwareProfileCameraType.Default,
                "Eyetoy1" => TelemetryHardwareProfileCameraType.EyeToy1,
                "Eyetoy2" => TelemetryHardwareProfileCameraType.EyeToy2,
                "USB" => TelemetryHardwareProfileCameraType.Usb,
                _ => throw new JsonSerializationException($"Unknown camera type {enumText}"),
            };
        }
        catch(Exception ex)
        {
            throw new JsonSerializationException("Failed to parse TelemetryIssuerId");
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum;
    }
}