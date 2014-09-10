using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

using RDH2.USB.Structs;

namespace RDH2.USB.PInvoke
{
    /// <summary>
    /// WinUSB wraps all of the P/Invoke calls to the 
    /// winusb.DLL library.
    /// </summary>
    internal class WinUSB
    {
        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_Initialize(SafeFileHandle DeviceHandle, out IntPtr InterfaceHandle);

        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_QueryInterfaceSettings(IntPtr InterfaceHandle, Byte AlternateSettingNumber, ref USB_INTERFACE_DESCRIPTOR UsbAltInterfaceDescriptor);

        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_QueryPipe(IntPtr InterfaceHandle, Byte AlternateInterfaceNumber, Byte PipeIndex, ref WINUSB_PIPE_INFORMATION PipeInformation);

        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_ReadPipe(IntPtr InterfaceHandle, Byte PipeID, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_WritePipe(IntPtr InterfaceHandle, Byte PipeID, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, IntPtr Overlapped);

        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_ControlTransfer(IntPtr InterfaceHandle, WINUSB_SETUP_PACKET SetupPacket, Byte[] Buffer, UInt32 BufferLength, ref UInt32 LengthTransferred, 
            IntPtr Overlapped);

        [DllImport("winusb.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern Boolean WinUsb_Free(IntPtr InterfaceHandle);
    }
}
