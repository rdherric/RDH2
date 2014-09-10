using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.USB.Structs
{
    /// <summary>
    /// WINUSB_SETUP_PACKET is used to send device I/O
    /// control requests to a USB device.
    /// </summary>
    internal struct WINUSB_SETUP_PACKET
    {
        public Byte RequestType;
        public Byte Request;
        public UInt16 Value;
        public UInt16 Index;
        public UInt16 Length;
    }
}
