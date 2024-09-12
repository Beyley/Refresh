using Newtonsoft.Json.Linq;

namespace Refresh.GameServer.Types.Telemetry.Json;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class JsonTelemetryEvent
{
    public JsonTelemetryHeader Header { get; set; }
    public JObject Data { get; set; }
    public JObject? CustomData { get; set; }
}