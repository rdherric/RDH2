using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// FileAccess is the equivalent of the Win32 
    /// file access flags.
    /// </summary>
    public enum FileAccess : uint
    {
        GenericRead = 0x80000000,
        GenericWrite = 0x40000000,
        GenericExecute = 0x20000000,
        GenericAll = 0x10000000
    }
}
