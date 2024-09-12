namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerLeaveEventCustomData
{
    public int GameTime { get; set; }
    public int Number { get; set; }
}