namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryStoreStartEvent
{
    public string StoreSessionId { get; set; }
    public string EntryPoint { get; set; }
}