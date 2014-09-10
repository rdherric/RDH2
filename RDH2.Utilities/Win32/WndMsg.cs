using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Utilities.Win32
{
    /// <summary>
    /// WndMsg contains the defines for the Win32 API 
    /// Window Messages
    /// </summary>
    internal enum WndMsg
    {
        WM_DESTROY = 0x0002,
        WM_CLOSE = 0x0010,
        WM_NOTIFY = 0x004E,
        WM_INITDIALOG = 0x0110,
        WM_COMMAND = 0x0111
    }
}
