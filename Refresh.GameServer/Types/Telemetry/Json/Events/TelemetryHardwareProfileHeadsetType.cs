using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(TelemetryHardwareProfileCameraTypeConverter))]
public enum TelemetryHardwareProfileHeadsetType
{
    None,
    Ps4,
    // TODO: what are *actually* the values this can be on PS3? "Bluetooth" and "A2DP" seems wrong to be the only options.
    Bluetooth,
    A2DP,
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
                "" => TelemetryHardwareProfileHeadsetType.None,
                "ps4" => TelemetryHardwareProfileHeadsetType.Ps4,
                "Bluetooth" => TelemetryHardwareProfileHeadsetType.Bluetooth,
                "A2DP" => TelemetryHardwareProfileHeadsetType.A2DP,
                "USB" => TelemetryHardwareProfileHeadsetType.Usb,
                _ => throw new JsonSerializationException($"Unknown headset type {enumText}"),
            };
        }
        catch(Exception ex)
        {
            throw new JsonSerializationException("Failed to parse headset type");
        }
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType.IsEnum;
    }
}