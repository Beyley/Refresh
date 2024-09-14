namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryHardwareProfileEventCustomData
{
    public TelemetryHardwareProfileConsole Console { get; set; }
}