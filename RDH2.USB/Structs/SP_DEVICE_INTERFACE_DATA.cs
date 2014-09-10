using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDH2.USB.Structs
{
    /// <summary>
    /// SP_DEVICE_INTERFACE_DATA is used in the enumeration
    /// of devices on a computer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVICE_INTERFACE_DATA
    {
        public UInt32 cbSize;
        public Guid  InterfaceClassGuid;
        public UInt32 Flags;
        public IntPtr Reserved;
    }
}
