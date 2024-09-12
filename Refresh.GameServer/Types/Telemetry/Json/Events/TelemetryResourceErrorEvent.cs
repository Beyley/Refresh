namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class TelemetryResourceErrorEvent
{
    public string Hash { get; set; }
    public int Guid { get; set; }
    public string ErrorType { get; set; }
}