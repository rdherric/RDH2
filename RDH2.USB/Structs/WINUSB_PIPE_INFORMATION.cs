using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RDH2.USB.Enums;

namespace RDH2.USB.Structs
{
    /// <summary>
    /// WINUSB_PIPE_INFORMATION is the equivalent of the
    /// Win32 struct.
    /// </summary>
    internal struct WINUSB_PIPE_INFORMATION
    {
        public UsbdPipeType PipeType;
        public Byte PipeId;
        public UInt16 MaximumPacketSize;
        public Byte Interval;
    }
}
