namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryNetworkJoinResultEvent
{
    public TelemetryNetworkJoinResultReporter Reporter;
    public string ClientId;
    public string HostId;
    public TelemetryNetworkJoinResultResponse Response;
    public TelemetryNetworkJoinResultReason Reason;
}