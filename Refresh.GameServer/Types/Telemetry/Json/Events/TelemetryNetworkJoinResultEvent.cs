namespace Refresh.GameServer.Types.Telemetry.Json.Events;

public class TelemetryNetworkJoinResultEvent
{
    [JsonProperty("reporter")] public string Reporter;
    [JsonProperty("client_id")] public string ClientId;
    [JsonProperty("host_id")] public string HostId;
    [JsonProperty("response")] public string Response;
    [JsonProperty("reason")] public string Reason;
}