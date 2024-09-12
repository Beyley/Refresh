using Refresh.GameServer.Types.Matching;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryNetworkProfileEvent
{
    public NatType NatType { get; set; }
    public string MacAddress { get; set; }
    public string NetworkQuality { get; set; }
}