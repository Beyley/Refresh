using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryPlayerJoinHowMatched
{
    CREATE,
    HOST_MIGRATE,
    FRIEND,
    INVITE,
    FOLLOW,
    HOME_GAME_LAUNCH,
    SEARCH_RESULT,
    DIVE_IN,
    UNKNOWN,
}