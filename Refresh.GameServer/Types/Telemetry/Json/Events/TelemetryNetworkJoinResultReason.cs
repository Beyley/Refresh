using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkJoinResultReason
{
    NONE,
    OVERHEATING,
    JOINING_ANOTHER,
    BLOCKING_PARTY_OPERATION,
    SAVING,
    INTRO,
    UNKNOWN,
}