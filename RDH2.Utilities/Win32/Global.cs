using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Utilities.Win32
{
    /// <summary>
    /// PrintHookProc is used as the callback method for the
    /// Win32 API to call into the class.
    /// </summary>
    /// <param name="hdlg">The HWnd for the Dialog</param>
    /// <param name="msg">The Win32 API Message</param>
    /// <param name="wparam">The WParam from the System</param>
    /// <param name="lparam">The LParam from the System</param>
    /// <returns>Zero if the Dialog handled the Message, non-Zero otherwise</returns>
    internal delegate IntPtr HookProcDelegate(IntPtr hdlg, Int32 msg, IntPtr wparam, IntPtr lparam);
}
