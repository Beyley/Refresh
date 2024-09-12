namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryLevelHackedEvent
{
    public string LevelId { get; set; } // in the format of "[%u,%u]"
}