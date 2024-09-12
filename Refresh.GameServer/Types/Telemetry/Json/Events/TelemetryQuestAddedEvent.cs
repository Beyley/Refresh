namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonObject(NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
public class TelemetryQuestAddedEvent
{
    public int QuestId { get; set; }
    public string QuestName { get; set; }
    public string QuestType { get; set; }
    public int[] Slot { get; set; }
    [JsonProperty("subobjectives")] public uint[] SubObjectives { get; set; }
    public int[] Targets { get; set; }
}