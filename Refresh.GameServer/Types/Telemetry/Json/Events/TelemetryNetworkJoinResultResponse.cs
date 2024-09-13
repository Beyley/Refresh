using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkJoinResultResponse
{
    ALLOW_JOIN,
    DISALLOW,
    TIMEOUT,
    AUTO_REJECT,
    CHAT,
    WRONG_VERSION,
    WRONG_BUILD_DATE,
    NO_VACANCIES,
    BUSY,
    TUTORIAL,
    POPPET_PAINT,
    BLOCKED_PLAYER_IN_PARTY,
    NO_NETWORK_MULTIPLAY,
    UNKNOWN,
}