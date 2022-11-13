using System.ComponentModel;

namespace TMS.Core.Domain.Enums
{
    public enum DeviceStatusEnum : int
    {
        [DescriptionAttribute("The device or module is disconnected.")]
        Disconnected = 0,
        [DescriptionAttribute("The device or module is connected.")]
        Connected = 1,
        [DescriptionAttribute("The device or module is attempting to reconnect.")]
        Disconnected_Retrying = 2,
        [DescriptionAttribute("The device connection was closed.")]
        Disabled = 3,
        [DescriptionAttribute("The device connection was closed.")]
        Broken = 4,
    }
}
