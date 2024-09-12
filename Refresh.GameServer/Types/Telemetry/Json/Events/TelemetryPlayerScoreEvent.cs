namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerScoreEvent
{
    public string GameId { get; set; }
    public int Score { get; set; }
}