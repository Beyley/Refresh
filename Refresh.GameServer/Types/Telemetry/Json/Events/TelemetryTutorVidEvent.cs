namespace Refresh.GameServer.Types.Telemetry.Json.Events;

// This struct uses PascalCase intentionally
public class TelemetryTutorVidEvent
{
    public int Video { get; set; }
    public bool FirstWatch { get; set; }
    public string Title { get; set; }
    public bool PlayedFromTutorialMenu { get; set; }
}