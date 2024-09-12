namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryMenuScreenEvent
{
    public string MenuScreen { get; set; }
    public string ReferrerMenu { get; set; }
}