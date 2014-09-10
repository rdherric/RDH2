using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// Monochromator retrieves the parameters for the Monochromator
    /// from the app.config file.
    /// </summary>
    [ConfigurationSection(Monochromator._sectName)]
    public class Monochromator : HardwareConfigBase
    {
        #region Member Variables
        private const String _sectName = "monochromator";
        private const String _startWavelengthKey = "startWavelength";
        private const String _endWavelengthKey = "endWavelength";
        private const String _incrementKey = "increment";
        private const String _stepsPerNmKey = "stepsPerNm";
        #endregion


        #region Public Properties
        /// <summary>
        /// StartWavelength determines the last configured 
        /// starting wavelength on the Monochromator.
        /// </summary>
        [ConfigurationProperty(Monochromator._startWavelengthKey, DefaultValue = 750U, IsRequired = false)]
        public UInt32 StartWavelength
        {
            get { return Convert.ToUInt32(this[Monochromator._startWavelengthKey]); }
            set { this[Monochromator._startWavelengthKey] = value; }
        }


        /// <summary>
        /// EndWavelength determines the last configured 
        /// ending wavelength on the Monochromator.
        /// </summary>
        [ConfigurationProperty(Monochromator._endWavelengthKey, DefaultValue = 450U, IsRequired = false)]
        public UInt32 EndWavelength
        {
            get { return Convert.ToUInt32(this[Monochromator._endWavelengthKey]); }
            set { this[Monochromator._endWavelengthKey] = value; }
        }


        /// <summary>
        /// Increment determines the last configured 
        /// wavelength step increment on the Monochromator.
        /// </summary>
        [ConfigurationProperty(Monochromator._incrementKey, DefaultValue = 2U, IsRequired = false)]
        public UInt32 Increment
        {
            get { return Convert.ToUInt32(this[Monochromator._incrementKey]); }
            set { this[Monochromator._incrementKey] = value; }
        }


        /// <summary>
        /// StepsPerNm determines the number of steps to take
        /// to move one nm of wavelength on the Monochromator.
        /// </summary>
        [ConfigurationProperty(Monochromator._stepsPerNmKey, DefaultValue = 20.0, IsRequired = false)]
        public Double StepsPerNm
        {
            get { return Convert.ToDouble(this[Monochromator._stepsPerNmKey]); }
            set { this[Monochromator._stepsPerNmKey] = value; }
        }
        #endregion
    }
}
