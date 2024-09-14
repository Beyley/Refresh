using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace Refresh.GameServer.Types.Telemetry.Json.Events;

[JsonConverter(typeof(StringEnumConverter))]
public enum TelemetryHardwareProfileCameraType
{
    [EnumMember(Value = "")]
    None,
    [EnumMember(Value = "default")]
    Default,
    [EnumMember(Value = "Eyetoy1")]
    EyeToy1,
    [EnumMember(Value = "Eyetoy2")]
    EyeToy2,
    [EnumMember(Value = "USB")]
    Usb,
}
