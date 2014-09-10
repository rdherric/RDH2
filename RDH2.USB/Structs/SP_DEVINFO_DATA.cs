using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDH2.USB.Structs
{
    /// <summary>
    /// SP_DEVINFO_DATA is used in the enumeration of 
    /// devices on the computer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SP_DEVINFO_DATA
    {
        public UInt32 cbSize;
        public Guid ClassGuid;
        public UInt32 DevInst;
        public IntPtr Reserved;
    }
}
