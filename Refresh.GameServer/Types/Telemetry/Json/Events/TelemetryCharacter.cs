using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryCharacter
{
    Giant,
    Sackboy,
    Oddsock,
    Bird,
    Dwarf,
}