using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// DaqInterface retrieves the common properties of the
    /// DAQ Interface to run the Photocurrent instruments.
    /// </summary>
    [ConfigurationSection(DaqInterface._sectName)]
    public class DaqInterface : HardwareConfigBase
    {
        #region Member Variables
        private const String _sectName = "daqInterface";

        //Common to all analysis
        private const String _daqTypeKey = "daqType";
        private const String _acquisitionTypeKey = "acquisitionType";
        private const String _boardNameKey = "boardName";
        private const String _voltageInPortKey = "voltageInPort";
        private const String _samplingRateKey = "samplingRate";
        private const String _acquisitionDurationKey = "acquisitionDuration";

        //2-D Spectra
        private const String _stepperOutPortKey = "stepperOutPort";
        private const String _scanUpPortKey = "scanUpPort";
        private const String _scanDownPortKey = "scanDownPort";
        private const String _scanCounterPortKey = "scanCounterPort";

        //3-D Spectra
        private const String _xOutPortKey = "xOutPort";
        private const String _yOutPortKey = "yOutPort";

        //Software Lock-In Amplifier
        private const String _inputFreqCtrInKey = "inputFreqCtrIn";
        #endregion


        #region Public Properties
        /// <summary>
        /// Type determines the actual kind of DAQ card that
        /// is configured on the computer.
        /// </summary>
        [ConfigurationProperty(DaqInterface._daqTypeKey, DefaultValue = Enums.DaqType.Invalid, IsRequired = false)]
        public Enums.DaqType Type
        {
            get { return (Enums.DaqType)Enum.Parse(typeof(Enums.DaqType), this[DaqInterface._daqTypeKey].ToString()); }
            set { this[DaqInterface._daqTypeKey] = value; }
        }


        /// <summary>
        /// AcquisitionTypes determines the actual kind of data 
        /// acquisition that is configured on this computer.
        /// </summary>
        public Enums.AcquisitionType[] AcquisitionTypes
        {
            get
            {
                //Declare a List to hold the output
                List<Enums.AcquisitionType> acqTypes = new List<Enums.AcquisitionType>();

                //Get the String of Enums from the Config
                String typeString = this.AcqTypeString;

                //Split the String 
                String[] types = typeString.Split(',');

                //Iterate through the Strings and try to make
                //Enums out of them
                foreach (String type in types)
                {
                    try
                    {
                        acqTypes.Add((Enums.AcquisitionType)Enum.Parse(typeof(Enums.AcquisitionType), type));
                    }
                    catch { }
                }

                //Return the result
                return acqTypes.ToArray();
            }

            set 
            { 
                //Declare a List to hold the input
                List<String> acqTypes = new List<String>();

                //Add the strings to the List
                foreach (Enums.AcquisitionType type in value)
                    acqTypes.Add(type.ToString());

                //Join the strings with a comma
                String typeString = String.Join(",", acqTypes.ToArray());

                //Finally, set the value in the Config
                this.AcqTypeString = typeString; 
            }
        }


        /// <summary>
        /// BoardName determines the name of the DAQ board
        /// that has been configured.
        /// </summary>
        [ConfigurationProperty(DaqInterface._boardNameKey, DefaultValue = "", IsRequired = false)]
        public String BoardName
        {
            get { return this[DaqInterface._boardNameKey].ToString(); }
            set { this[DaqInterface._boardNameKey] = value; }
        }


        /// <summary>
        /// VoltageInPort determines the name of the DAQ channel
        /// used to read voltages.
        /// </summary>
        [ConfigurationProperty(DaqInterface._voltageInPortKey, DefaultValue = "", IsRequired = false)]
        public String VoltageInPort
        {
            get { return this[DaqInterface._voltageInPortKey].ToString(); }
            set { this[DaqInterface._voltageInPortKey] = value; }
        }


        /// <summary>
        /// StepperOutPort determines the name of the DAQ channel
        /// used to drive the monochromator stepper motor.
        /// </summary>
        [ConfigurationProperty(DaqInterface._stepperOutPortKey, DefaultValue = "", IsRequired = false)]
        public String StepperOutPort
        {
            get { return this[DaqInterface._stepperOutPortKey].ToString(); }
            set { this[DaqInterface._stepperOutPortKey] = value; }
        }


        /// <summary>
        /// ScanUpPort determines the name of the DAQ channel
        /// used to set the stepper controller to scan positive.
        /// </summary>
        [ConfigurationProperty(DaqInterface._scanUpPortKey, DefaultValue = "", IsRequired = false)]
        public String ScanUpPort
        {
            get { return this[DaqInterface._scanUpPortKey].ToString(); }
            set { this[DaqInterface._scanUpPortKey] = value; }
        }


        /// <summary>
        /// ScanDownPort determines the name of the DAQ channel
        /// used to set the stepper controller to scan negative.
        /// </summary>
        [ConfigurationProperty(DaqInterface._scanDownPortKey, DefaultValue = "", IsRequired = false)]
        public String ScanDownPort
        {
            get { return this[DaqInterface._scanDownPortKey].ToString(); }
            set { this[DaqInterface._scanDownPortKey] = value; }
        }


        /// <summary>
        /// ScanCounterPort determines the name of the DAQ channel
        /// used as a Counter to scan the stepper controller.
        /// </summary>
        [ConfigurationProperty(DaqInterface._scanCounterPortKey, DefaultValue = "", IsRequired = false)]
        public String ScanCounterPort
        {
            get { return this[DaqInterface._scanCounterPortKey].ToString(); }
            set { this[DaqInterface._scanCounterPortKey] = value; }
        }


        /// <summary>
        /// XOutPort determines the name of the DAQ channel
        /// used to operate the X-Axis mirror.
        /// </summary>
        [ConfigurationProperty(DaqInterface._xOutPortKey, DefaultValue = "", IsRequired = false)]
        public String XOutPort
        {
            get { return this[DaqInterface._xOutPortKey].ToString(); }
            set { this[DaqInterface._xOutPortKey] = value; }
        }


        /// <summary>
        /// YOutPort determines the name of the DAQ channel
        /// used to operate the Y-Axis mirror.
        /// </summary>
        [ConfigurationProperty(DaqInterface._yOutPortKey, DefaultValue = "", IsRequired = false)]
        public String YOutPort
        {
            get { return this[DaqInterface._yOutPortKey].ToString(); }
            set { this[DaqInterface._yOutPortKey] = value; }
        }

        
        /// <summary>
        /// SamplingRate determines how many data points
        /// to acquire per second
        /// </summary>
        [ConfigurationProperty(DaqInterface._samplingRateKey, DefaultValue = 37U, IsRequired = false)]
        public UInt32 SamplingRate
        {
            get { return Convert.ToUInt32(this[DaqInterface._samplingRateKey]); }
            set { this[DaqInterface._samplingRateKey] = value; }
        }


        /// <summary>
        /// AcquisitionDuration determines how how long it takes
        /// for the DAQ to read a point of data.
        /// </summary>
        [ConfigurationProperty(DaqInterface._acquisitionDurationKey, DefaultValue = 100U, IsRequired = false)]
        public UInt32 AcquisitionDuration
        {
            get { return Convert.ToUInt32(this[DaqInterface._acquisitionDurationKey]); }
            set { this[DaqInterface._acquisitionDurationKey] = value; }
        }


        /// <summary>
        /// InputCounterInPort determines the name of the DAQ channel
        /// used to count the input frequency for the software-based
        /// lock-in amplifier.
        /// </summary>
        [ConfigurationProperty(DaqInterface._inputFreqCtrInKey, DefaultValue = "", IsRequired = false)]
        public String InputCounterInPort
        {
            get { return this[DaqInterface._inputFreqCtrInKey].ToString(); }
            set { this[DaqInterface._inputFreqCtrInKey] = value; }
        }
        #endregion


        #region Private Properties
        /// <summary>
        /// AcqTypeString returns the String value of the 
        /// Enum Array from the Config file.
        /// </summary>
        [ConfigurationProperty(DaqInterface._acquisitionTypeKey, DefaultValue = "Invalid", IsRequired = false)]
        public String AcqTypeString
        {
            get { return this[DaqInterface._acquisitionTypeKey].ToString(); }
            set { this[DaqInterface._acquisitionTypeKey] = value; }
        }
        #endregion
    }
}
