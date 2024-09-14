using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryIssuerId
{
    [EnumMember(Value = "np")]
    RetailNetwork,
    [EnumMember(Value = "sp-int")]
    DeveloperNetwork,
    [EnumMember(Value = "cert")]
    QualityAssuranceNetwork,
    [EnumMember(Value = "-")]
    Unknown,
}
