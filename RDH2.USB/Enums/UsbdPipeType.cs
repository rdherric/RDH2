using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// UsbdPipeType enumerates the type of USB
    /// Pipe that an interface supports.
    /// </summary>
    public enum UsbdPipeType
    {
        None = -1,
        UsbdPipeTypeControl = 0,
        UsbdPipeTypeIsochronous = 1,
        UsbdPipeTypeBulk = 2,
        UsbdPipeTypeInterrupt = 3
    }
}
