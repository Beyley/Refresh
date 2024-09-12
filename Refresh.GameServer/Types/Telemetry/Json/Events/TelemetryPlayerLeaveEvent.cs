namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerLeaveEvent
{
    public string GameId { get; set; }
    public string NpOnlineId { get; set; }
    public string LevelId { get; set; }
    public int DurationSecs { get; set; }
    public string Gameover { get; set; }
    public string Mode { get; set; }
    public int PlayerCount { get; set; }
}