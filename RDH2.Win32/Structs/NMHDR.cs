using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDH2.Win32.Structs
{
    /// <summary>
    /// The NMHDR struct is passed whenever a WM_NOTIFY
    /// message is sent to a Dialog box.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
    internal struct NMHDR 
    {
        public IntPtr hwndFrom;
        public IntPtr idFrom;
        public UInt32 code;
    }
}
