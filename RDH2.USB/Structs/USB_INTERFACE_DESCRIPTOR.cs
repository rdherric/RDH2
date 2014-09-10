using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.USB.Structs
{
    /// <summary>
    /// USB_INTERFACE_DESCRIPTOR is the struct used to 
    /// query the WinUSB library about an interface on 
    /// a USB device.
    /// </summary>
    internal struct USB_INTERFACE_DESCRIPTOR
    {
        public Byte bLength;
        public Byte bDescriptorType;
        public Byte bInterfaceNumber;
        public Byte bAlternateSetting;
        public Byte bNumEndpoints;
        public Byte bInterfaceClass;
        public Byte bInterfaceSubClass;
        public Byte bInterfaceProtocol;
        public Byte iInterface;
    }
}
