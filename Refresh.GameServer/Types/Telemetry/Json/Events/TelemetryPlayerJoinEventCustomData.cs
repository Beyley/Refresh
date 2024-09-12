namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPlayerJoinEventCustomData
{
    public string LevelId { get; set; }
    public int GameTime { get; set; }
    public int Number { get; set; }
    [JsonProperty("HowMatched")] public string HowMatched { get; set; }
}