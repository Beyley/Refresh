using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryNetworkJoinResultReporter
{
    [EnumMember(Value = "HOST")]
    Host,
    [EnumMember(Value = "CLIENT")]
    Client,
    [EnumMember(Value = "UNKNOWN")]
    Unknown,
}