namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryPinAchieveEvent
{
    public string PinSetVersion { get; set; }
    public bool IsFirstTime { get; set; }
    public string PinType { get; set; }
    public int PinId { get; set; }
}