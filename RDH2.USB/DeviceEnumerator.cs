using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

using RDH2.USB.Enums;
using RDH2.USB.PInvoke;
using RDH2.USB.Structs;

namespace RDH2.USB
{
    /// <summary>
    /// DeviceEnumerator is used to get the devices
    /// that are installed and present on the Computer.
    /// </summary>
    public class DeviceEnumerator
    {
        #region Member variables
        private Guid _devGuid = Guid.Empty;
        private List<Device> _devices = null;

        private const String _vidPidToken = "#VID_([A-Fa-f0-9]{1,4})&PID_([A-Fa-f0-9]{1,4})#";
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor for the DeviceEnumerator class.
        /// </summary>
        /// <param name="devGuid">The Type of device to enumerate</param>
        public DeviceEnumerator(Guid devGuid)
        {
            //Save the input in the member variables
            this._devGuid = devGuid;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// Devices exposes all of the devices that were
        /// enumerated on the computer.
        /// </summary>
        public List<Device> Devices
        {
            get 
            { 
                //If the Devices haven't been enumerated yet,
                //enumerate them
                if (this._devices == null)
                    this.FillDevices();

                //Return the List
                return this._devices; 
            }
        }
        #endregion


        #region Public class to expose the Devices that were found
        /// <summary>
        /// The Device class is exposed by the DeviceEnumerator
        /// class as a list of devices that were found on the
        /// computer.
        /// </summary>
        public class Device
        {
            public Guid ClassGuid = Guid.Empty;
            public String DevicePath = String.Empty;
            public Int32 VendorID = 0;
            public Int32 ProductID = 0;
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// FillDevices enumerates all of the devices that were
        /// requested in the constructor.
        /// </summary>
        private void FillDevices()
        {
            //Create the List in which the Devices will be stored
            this._devices = new List<Device>();

            //Declare a variable to hold the list of Devices
            IntPtr hDevInfo = IntPtr.Zero;

            try 
            {
                //Get the SP_DEVINFO_DATA handle
                hDevInfo = SetupAPI.SetupDiGetClassDevs(ref this._devGuid, null, IntPtr.Zero, 
                    Convert.ToUInt32(GetClassDevs.DIGCF_PRESENT | GetClassDevs.DIGCF_DEVICEINTERFACE));

                //If an error occurred, throw an exception
                if (hDevInfo.ToInt32() == -1)
                    throw new System.ArgumentException("Could not enumerate Devices on the local computer.");

                //Iterate through the classes that were returned
                UInt32 classIndex = 0;
                Boolean enumClasses = true;

                while (enumClasses == true)
                {
                    //Get the class information
                    SP_DEVINFO_DATA devInfoData = new SP_DEVINFO_DATA();
                    devInfoData.cbSize = Convert.ToUInt32(Marshal.SizeOf(devInfoData));
                    enumClasses = SetupAPI.SetupDiEnumDeviceInfo(hDevInfo, classIndex, ref devInfoData);
                                        
                    //Enumerate through the devices until the 
                    //function returns false
                    UInt32 ifaceIndex = 0;
                    Boolean enumIfaces = true;

                    while (enumClasses == true && enumIfaces == true)
                    {
                        //Declare an Interface Data struct
                        SP_DEVICE_INTERFACE_DATA iface = new SP_DEVICE_INTERFACE_DATA();
                        iface.cbSize = Convert.ToUInt32(Marshal.SizeOf(iface));
                        enumIfaces = SetupAPI.SetupDiEnumDeviceInterfaces(hDevInfo, ref devInfoData, ref this._devGuid, ifaceIndex, ref iface); 

                        //If the Interface was returned, get the detail
                        if (enumIfaces == true)
                        {
                            //Declare an Interface Detail Data struct
                            SP_DEVICE_INTERFACE_DETAIL_DATA detail = new SP_DEVICE_INTERFACE_DETAIL_DATA();

                            //Set the size based on the OS.  This is being done
                            //as instructed by pinvoke.net
                            if (IntPtr.Size == 8)
                                detail.cbSize = 8;
                            else
                                detail.cbSize = Convert.ToUInt32(4 + Marshal.SystemDefaultCharSize);

                            //Get the detail
                            UInt32 reqSize = 0;
                            SetupAPI.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref iface, ref detail, 256, out reqSize, ref devInfoData);

                            //If the detail was retrieved, create a new Device struct
                            if (detail.DevicePath != String.Empty)
                                this._devices.Add(this.CreateDeviceStruct(iface, detail));
                        }

                        //Increment the Interface index
                        ifaceIndex++;
                    }

                    //Increment the Class index
                    classIndex++;
                }
            }
            finally
            {
                //Clean up the List if it was created
                if (hDevInfo != IntPtr.Zero)
                    SetupAPI.SetupDiDestroyDeviceInfoList(hDevInfo);
            }
        }


        /// <summary>
        /// CreateDeviceStruct parses the device path and creates
        /// a new Device object.
        /// </summary>
        /// <param name="devPath">The Path to the Device</param>
        private Device CreateDeviceStruct(SP_DEVICE_INTERFACE_DATA iface, SP_DEVICE_INTERFACE_DETAIL_DATA detail)
        {
            //Declare a variable to return
            Device rtn = new Device();

            //Fill the members
            rtn.ClassGuid = iface.InterfaceClassGuid;
            rtn.DevicePath = detail.DevicePath;

            //Parse out the USB information if it exists
            Regex vidPid = new Regex(DeviceEnumerator._vidPidToken);
            Match m = vidPid.Match(detail.DevicePath.ToUpper());

            //If both VID and PID were captured, set them
            if (m.Groups.Count == 3)
            {
                rtn.VendorID = Convert.ToInt32(m.Groups[1].Value, 16);
                rtn.ProductID = Convert.ToInt32(m.Groups[2].Value, 16);
            }

            //Return the result
            return rtn;
        }
        #endregion
    }
}
