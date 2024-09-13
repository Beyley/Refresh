using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkTimeoutError
{
    TIMEOUT_WAIT_FOR_JOIN_RESPONSE,
    DISCONNECT_WAIT_FOR_JOIN_RESPONSE,
    UNKNOWN,
}