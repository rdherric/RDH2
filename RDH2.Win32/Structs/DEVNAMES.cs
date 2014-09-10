using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.Win32.Structs
{
    /// <summary>
    /// DEVNAMES is returned by the Print Dialog to 
    /// hand out the device name chose by the user.
    /// </summary>
    public struct DEVNAMES
    {
        public UInt16 wDriverOffset;
        public UInt16 wDeviceOffset;
        public UInt16 wOutputOffset;
        public UInt16 wDefault;
    }
}
