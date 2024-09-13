using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryPinType
{
    FIRST,
    PLAY_STORY,
    PLAY_COMMUNITY,
    CREATE,
    SHARE,
    MM,
    MOVE,
    MUPPET,
    CROSSPLAY,
    DCCOMICS,
    LBP3_STORY,
    LBP3_COMMUNITY,
    LBP3_CREATE,
    LBP3_SHARE,
    LBP3_MM,
    LBP3_PS4,
    LBP3_DEPRICATED,
    LBP3_JOURNEY_HOME,
    LBP3_CHALLENGE,
}