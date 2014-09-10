using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDH2.Win32.Structs
{
    /// <summary>
    /// POINT holds the definition of a Point in Windows.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct POINT
    {
        public Int32 x;
        public Int32 y;
    }
}
