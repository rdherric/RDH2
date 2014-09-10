using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.USB.Enums
{
    /// <summary>
    /// FileShare is the equivalent of the Win32 
    /// file sharing flags.
    /// </summary>
    public enum FileShare : uint
    {
        None = 0x00000000,
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004
    }
}
