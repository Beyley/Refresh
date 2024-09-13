using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkResourceErrorType
{
    NoErrorState,
    TimeOut,
    PlayerTooBusy,
}