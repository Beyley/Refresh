namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerJoinEvent
{
    public string GameId { get; set; }
    public string NpOnlineId { get; set; }
    public string PlayerType { get; set; }
}