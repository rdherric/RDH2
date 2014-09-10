using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using RDH2.Win32.Structs;

namespace RDH2.Win32.PInvoke
{
    /// <summary>
    /// ComDlg32 wraps all of the P/Invoke calls to the
    /// comdlg32.DLL library.
    /// </summary>
    internal class ComDlg32
    {
        [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean PrintDlg(ref PRINTDLG pdlg);

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean GetOpenFileName(ref OPENFILENAME ofn);

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean GetSaveFileName(ref OPENFILENAME ofn);

        [DllImport("comdlg32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 CommDlgExtendedError();
    }
}
