namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryUserProfileEvent
{
    public string NpOnlineId { get; set; }
    public int NpAccountId { get; set; }
    public int Age { get; set; }
    public string Region { get; set; }
    public string Language { get; set; }
    public string[] LanguagesUsed { get; set; }
    public bool RestrictChat { get; set; }
}