using System;
using System.Runtime.InteropServices;

namespace RDH2.Win32.PInvoke
{
    /// <summary>
    /// Kernel32 wraps all of the P/Invoke calls out to the
    /// kernel32.DLL library.
    /// </summary>
    internal class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean GlobalUnlock(IntPtr hMem);

    }
}
