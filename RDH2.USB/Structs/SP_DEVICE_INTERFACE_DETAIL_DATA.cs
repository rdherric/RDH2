using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDH2.USB.Structs
{
    /// <summary>
    /// SP_DEVICE_INTERFACE_DETAIL_DATA is used in the 
    /// enumeration of the devices on a computer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct SP_DEVICE_INTERFACE_DETAIL_DATA
    {
        public UInt32 cbSize;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public String DevicePath;
    }
}
