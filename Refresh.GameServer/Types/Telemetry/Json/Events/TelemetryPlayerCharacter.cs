using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryPlayerCharacter
{
    SackBoy,
    ToggleBig,
    ToggleSmall,
    Swoop,
    OddSock,
}