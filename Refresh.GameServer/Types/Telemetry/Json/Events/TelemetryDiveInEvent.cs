using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryDiveInEvent
{
    HOST_START_SOCIAL,
    HOST_START_PAUSE,
    HOST_TIMEOUT_CANCEL,
    HOST_TIMEOUT_CONTINUE,
    HOST_MANUAL_CANCEL,
    HOST_ABORTED,
    HOST_SUCESS,
    JOIN_START_SOCIAL,
    JOIN_START_PAUSE,
    JOIN_TIMEOUT_CANCEL,
    JOIN_MANUAL_CANCEL,
    JOIN_ABORTED,
    JOIN_RECEIVED_HOST,
}