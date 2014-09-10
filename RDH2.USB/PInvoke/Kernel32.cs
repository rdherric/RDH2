using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Win32.SafeHandles;

using RDH2.USB.Enums;

namespace RDH2.USB.PInvoke
{
    internal class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern SafeFileHandle CreateFile(String lpFileName, FileAccess dwDesiredAccess, FileShare dwShareMode, IntPtr lpSecurityAttributes,
            CreationDisposition dwCreationDisposition, FileAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll", BestFitMapping = true, CharSet = CharSet.Ansi)]
        public static extern Boolean WriteFile(SafeFileHandle hFile, Byte[] lpBuffer, UInt32 nNumberOfBytesToWrite, out UInt32 lpNumberOfBytesWritten,
            [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern Boolean ReadFile(SafeFileHandle hFile, [Out] Byte[] lpBuffer, UInt32 nNumberOfBytesToRead, out UInt32 lpNumberOfBytesRead,
            [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean DeviceIoControl(SafeFileHandle hDevice, UInt32 dwIoControlCode, Byte[] InBuffer, UInt32 nInBufferSize,
            Byte[] OutBuffer, UInt32 nOutBufferSize, ref UInt32 pBytesReturned, [In] ref NativeOverlapped lpOverlapped);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Boolean CloseHandle(SafeFileHandle hFile);
    }
}
