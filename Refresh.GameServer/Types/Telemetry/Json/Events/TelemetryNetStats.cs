namespace Refresh.GameServer.Types.Telemetry.Json.Events;

public class TelemetryNetStats
{
    [JsonProperty("IsLocal")] public bool IsLocal { get; set; }
    [JsonProperty("Player")] public string Player { get; set; }
    [JsonProperty("AvailBW")] public int AvailableBandwidth { get; set; }
    [JsonProperty("AvailRNPBW")] public int AvailableRnpBandwidth { get; set; }
    [JsonProperty("AvailGameBW")] public double AvailableGameBandwidth { get; set; }
    [JsonProperty("RecentTotBWUsed")] public int RecentTotalBandwidthUsed { get; set; }
    [JsonProperty("TimeBtwnSends")] public double TimeBetweenSends { get; set; }
}