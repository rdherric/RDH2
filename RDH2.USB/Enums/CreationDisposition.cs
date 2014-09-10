using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// CreationDisposition is the equivalent of the Win32
    /// file creation disposition flags.
    /// </summary>
    public enum CreationDisposition : uint
    {
        New = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TruncateExisting = 5
    }
}
