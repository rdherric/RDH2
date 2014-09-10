using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.Win32.SafeHandles;

using RDH2.USB.Enums;
using RDH2.USB.PInvoke;
using RDH2.USB.Structs;

namespace RDH2.USB
{
    /// <summary>
    /// The UsbInterface class is used to communicate
    /// with a USB device installed on the computer.
    /// </summary>
    public class UsbInterface : IDisposable
    {
        #region Member Variables
        private SafeFileHandle _deviceHandle = null;
        private IntPtr _usbHandle = IntPtr.Zero;
        private List<UsbPipe> _pipes = new List<UsbPipe>();
        #endregion


        #region Constructors / Destructor
        /// <summary>
        /// Default constructor for the UsbInterface object.
        /// </summary>
        /// <param name="vendorID">The Vendor ID of the USB Device</param>
        /// <param name="productID">The Product ID of the USB Device</param>
        public UsbInterface(Int32 vendorID, Int32 productID)
        {
            //Get the Device Path -- this will throw an Exception
            //if the device is not found
            String devPath = this.FindUSBDevicePath(vendorID, productID);

            //Open the handle to the Device
            this._usbHandle = this.CreateWinUSBHandle(devPath);

            //Finally, get the Pipe IDs for the device
            this.GetPipeIDs();
        }


        /// <summary>
        /// The UsbInterface destructor cleans up the resources.
        /// </summary>
        ~UsbInterface()
        {
            //Call the Dispose method
            this.Dispose();
        }
        #endregion


        #region Device Access Methods
        /// <summary>
        /// Write puts the specified byte Array on the device.
        /// </summary>
        /// <param name="input">The Array of Bytes to write to the Device</param>
        public void Write(UsbPipe pipe, Byte[] input)
        {
            //Check the pipe
            this.CheckPipe(pipe, UsbPipeDirection.Out, UsbdPipeType.UsbdPipeTypeBulk);

            //Declare variables to pass to the Device
            UInt32 bytesToWrite = Convert.ToUInt32(input.GetLength(0));
            UInt32 bytesWritten = 0;

            //Write the Array to the device -- save the error code
            //if one comes up
            Int32 errCode = Int32.MaxValue;
            if (WinUSB.WinUsb_WritePipe(this._usbHandle, pipe.ID, input, bytesToWrite, ref bytesWritten, IntPtr.Zero) == false)
                errCode = Marshal.GetLastWin32Error();

            //If the number of Bytes written don't match, get the 
            //error code again
            if (bytesToWrite != bytesWritten)
                errCode = Marshal.GetLastWin32Error();
            
            //Throw an exception if an error occurred
            if (errCode != Int32.MaxValue)
            {
                throw new System.IO.IOException("Write to device failed.  Error Code: " + errCode.ToString(),
                    new System.ComponentModel.Win32Exception(errCode));
            }
        }


        /// <summary>
        /// Read gets the specified Byte Array from the Device.
        /// </summary>
        /// <param name="bytesToRead">The amount of memory to read from the Device</param>
        /// <returns>Byte Array of data from the device</returns>
        public Byte[] Read(UsbPipe pipe, Int32 bytesToRead)
        {
            //Check the pipe
            this.CheckPipe(pipe, UsbPipeDirection.In, UsbdPipeType.UsbdPipeTypeBulk);

            //Declare a variable to hold the output
            Byte[] rtn = new Byte[bytesToRead];

            //Declare variables to pass to the Device
            UInt32 bytesRead = 0;

            //Read the Array from the Device -- save the error code
            //if one comes up
            Int32 errCode = Int32.MaxValue;
            if (WinUSB.WinUsb_ReadPipe(this._usbHandle, pipe.ID, rtn, Convert.ToUInt32(bytesToRead), ref bytesRead, IntPtr.Zero) == false)
                errCode = Marshal.GetLastWin32Error();

            //Throw an exception if an error occurred
            if (errCode != Int32.MaxValue)
            {
                throw new System.IO.IOException("Read from device failed.  Error Code: " + errCode.ToString(),
                    new System.ComponentModel.Win32Exception(errCode));
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// IoControl puts the specified byte Array on the Device
        /// and returns the control code-specified output.
        /// </summary>
        /// <param name="input">The input buffer to the Device</param>
        /// <param name="controlCode">The Control Code to send to the Device</param>
        /// <param name="bytesToRead">The number of bytes to read from the device</param>
        /// <returns>Byte Array or data from the device based on the Control Code</returns>
        public Byte[] IoControl(UsbPipe pipe, Byte[] input, UInt32 controlCode, Int32 bytesToRead)
        {
            //Check the pipe
            this.CheckPipe(pipe, UsbPipeDirection.In & UsbPipeDirection.Out, UsbdPipeType.UsbdPipeTypeControl);

            //Declare a variable to hold the output
            Byte[] rtn = new Byte[bytesToRead];

            //Declare variables to pass to the Device
            WINUSB_SETUP_PACKET setupPkt = new WINUSB_SETUP_PACKET();
            UInt32 bytesRead = 0;
            UInt32 bytesToWrite = 0;
            if (input != null)
                bytesToWrite = Convert.ToUInt32(input.GetLength(0));

            //Setup the setup packet
            setupPkt.Request = Convert.ToByte(controlCode);
            setupPkt.Length = Convert.ToUInt16(bytesToRead);
            setupPkt.Value = 0;
            setupPkt.Index = 0;

            //Set the type to 0 for host to device transfer.
            //If bytesToRead is not zero, set the type to 1.
            setupPkt.RequestType = 0;
            if (bytesToRead > 0)
                setupPkt.RequestType = 1;

            //Read the Array from the Device -- save the error code
            //if one comes up
            Int32 errCode = Int32.MaxValue;
            if (WinUSB.WinUsb_ControlTransfer(this._usbHandle, setupPkt, input, bytesToWrite, ref bytesRead, IntPtr.Zero) == false)
                errCode = Marshal.GetLastWin32Error();

            //Throw an exception if an error occurred
            if (errCode != Int32.MaxValue)
            {
                throw new System.IO.IOException("Control I/O to device failed.  Error Code: " + errCode.ToString(),
                    new System.ComponentModel.Win32Exception(errCode));
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// Pipes returns the available USB Pipes for
        /// reading and writing.
        /// </summary>
        public List<UsbPipe> Pipes
        {
            get { return this._pipes; }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// CreateWinUSBHandle does the dirty work of opening the 
        /// Device for Read / Write access and initializing the 
        /// WinUSB library with it.
        /// </summary>
        /// <param name="devPath">The path to the Device</param>
        /// <returns>The Handle to the Device "file"</returns>
        private IntPtr CreateWinUSBHandle(String devPath)
        {
            //Declare a variable to return
            IntPtr rtn = IntPtr.Zero;

            //Open the device with the Path
            this._deviceHandle = Kernel32.CreateFile(devPath, FileAccess.GenericRead | FileAccess.GenericWrite, 
                FileShare.Read | FileShare.Write, IntPtr.Zero, CreationDisposition.OpenExisting, 
                FileAttributes.Overlapped, IntPtr.Zero);

            //If the IntPtr returned is valid, initialize 
            //WinUSB and return that
            if (this._deviceHandle != null && this._deviceHandle.IsInvalid == false)
            {
                if (WinUSB.WinUsb_Initialize(this._deviceHandle, out rtn) == true)
                    return rtn;
            }

            //If we got here, the handle is invalid, so throw an exception
            throw new System.ArgumentException("Could not open path to Device '" + devPath + "'.");
        }


        /// <summary>
        /// GetPipeIDs enumerates the USB Pipes and determines
        /// which are the in and out pipes.
        /// </summary>
        private void GetPipeIDs()
        {
            //Declare a USB_INTERFACE_DESCRIPTOR to get the 
            //pipe information
            USB_INTERFACE_DESCRIPTOR uid = new USB_INTERFACE_DESCRIPTOR();

            //If the interface is retrieved successfully, enumerate the pipes
            if (WinUSB.WinUsb_QueryInterfaceSettings(this._usbHandle, 0, ref uid) == true)
            {
                //Iterate through the pipes and save the IDs
                for (Byte i = 0; i < uid.bNumEndpoints; i++)
                {
                    //Declare a WINUSB_PIPE_INFORMATION struct to get the data
                    WINUSB_PIPE_INFORMATION wpi = new WINUSB_PIPE_INFORMATION();

                    //If the Pipe was queried successfully, save the Pipe
                    //in the member variable
                    if (WinUSB.WinUsb_QueryPipe(this._usbHandle, 0, i, ref wpi) == true)
                    {
                        //Create a new Pipe
                        UsbPipe pipe = new UsbPipe();
                        pipe.ID = wpi.PipeId;
                        pipe.Type = wpi.PipeType;

                        //Set the direction
                        if ((wpi.PipeId & (Byte)UsbPipeDirection.In) > 0)
                            pipe.Direction = UsbPipeDirection.In;
                        else
                            pipe.Direction = UsbPipeDirection.Out;

                        //Add the Pipe to the Member Variable
                        this._pipes.Add(pipe);
                    }
                }
            }
        }


        /// <summary>
        /// FindUSBDevicePath iterates through the USB devices
        /// installed on the computer and checks the Vendor ID
        /// and Product ID for the specified device.
        /// </summary>
        /// <param name="vendorID">The ID of the Vendor who made the device</param>
        /// <param name="productID">The Product ID of the device</param>
        /// <returns>String Device Path for the specified device</returns>
        private String FindUSBDevicePath(Int32 vendorID, Int32 productID)
        {
            //Declare a variable to return
            String rtn = String.Empty;

            //Get the Device Path using the DeviceEnumerator
            DeviceEnumerator devEnum = new DeviceEnumerator(DeviceGuid.GUID_DEVINTERFACE_USB_DEVICE);

            //Iterate through the devices and find the one
            //that is specified
            foreach (DeviceEnumerator.Device dev in devEnum.Devices)
            {
                if (dev.VendorID == vendorID && dev.ProductID == productID)
                {
                    rtn = dev.DevicePath;
                    break;
                }
            }

            //If the device path was not found, throw an Exception
            if (rtn == String.Empty)
                throw new System.ArgumentException("Vendor ID '" + vendorID.ToString() + "' and Product ID '" + productID.ToString() + "' not found.");

            //Return the result
            return rtn;
        }


        /// <summary>
        /// CheckPipe is used to make sure that the pipe that was 
        /// passed as a parameter is valid for the operation.
        /// </summary>
        /// <param name="pipe">The Pipe that is being used</param>
        /// <param name="direction">The desired transfer direction</param>
        /// <param name="type">The Type of the Pipe</param>
        private void CheckPipe(UsbPipe pipe, UsbPipeDirection direction, UsbdPipeType type)
        {
            //If the Pipe is null, throw an Exception
            if (pipe == null)
                throw new System.ArgumentNullException("USB Pipe object cannot be Null.");

            //If the ID isn't valid, throw an Exception
            if (pipe.ID == Byte.MaxValue)
                throw new System.ArgumentException("USB Pipe object has an invalid ID.");

            //If the Type doesn't match, throw an Exception
            if (pipe.Type != type)
                throw new System.ArgumentException("USB Pipe object is not valid for the type  of transfer.");

            //If the direction doesn't match, throw an Exception
            if (pipe.Direction != direction)
                throw new System.ArgumentException("UsbPipe is not valid for the direction of transfer.");
        }
        #endregion


        #region Public class for exposing USB Pipes
        /// <summary>
        /// The UsbPipe class is used to return information about
        /// a Pipe on a USB device to the caller.
        /// </summary>
        public class UsbPipe
        {
            public Byte ID = Byte.MaxValue;
            public UsbdPipeType Type = UsbdPipeType.None;
            public UsbPipeDirection Direction = UsbPipeDirection.None;
        }
        #endregion


        #region IDisposable Members
        /// <summary>
        /// Dispose is used to free the WinUSB handle that 
        /// might be hanging around.
        /// </summary>
        public void Dispose()
        {
            //Free the WinUSB handle if it has been created
            if (this._usbHandle != IntPtr.Zero)
            {
                WinUSB.WinUsb_Free(this._usbHandle);
                this._usbHandle = IntPtr.Zero;
            }

            //Free the Device handle if it has been created
            if (this._deviceHandle != null)
            {
                Kernel32.CloseHandle(this._deviceHandle);
                this._deviceHandle = null;
            }
        }
        #endregion
    }
}
