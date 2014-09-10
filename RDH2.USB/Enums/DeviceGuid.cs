using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// DevEnum is an enumerator class for the 
    /// types of Devices that can be retrieved
    /// by the DeviceEnumerator class.
    /// </summary>
    public static class DeviceGuid
    {
        public static Guid GUID_DEVINTERFACE_USB_DEVICE = new Guid(0xA5DCBF10, 0x6530, 0x11D2, 0x90, 0x1F, 0x00, 0xC0, 0x4F, 0xB9, 0x51, 0xED);
    }
}
