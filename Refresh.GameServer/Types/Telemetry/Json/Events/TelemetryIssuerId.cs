namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(TelemetryIssuerIdConverter))]
public enum TelemetryIssuerId
{
    RetailNetwork,
    DeveloperNetwork,
    QualityAssurance,
    Unknown,
}

file class TelemetryIssuerIdConverter : JsonConverter {
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
                "np" => TelemetryIssuerId.RetailNetwork,
                "sp-int" => TelemetryIssuerId.DeveloperNetwork,
                "cert" => TelemetryIssuerId.QualityAssurance,
                "-" => TelemetryIssuerId.Unknown,
                _ => throw new JsonSerializationException($"Unknown issuer ID {enumText}"),
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