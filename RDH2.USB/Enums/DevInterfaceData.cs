using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// DevInterfaceData contains the Flags for the
    /// SP_DEVICE_INTERFACE_DATA struct.
    /// </summary>
    public enum DevInterfaceData
    {
        SPINT_ACTIVE = 0x00000001,
        SPINT_DEFAULT = 0x00000002,
        SPINT_REMOVED = 0x00000004    
    }
}
