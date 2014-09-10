using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// UsbPipeDirection determines if the Pipe is 
    /// used for reading or writing.
    /// </summary>
    public enum UsbPipeDirection
    {
        None = 0xFF,
        In = 0x80,
        Out = 0x00
    }
}
