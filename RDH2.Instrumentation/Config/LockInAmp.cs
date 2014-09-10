using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// LockInAmp holds the Configuration Settings
    /// for the Lock-In Amplifier.
    /// </summary>
    [ConfigurationSection(LockInAmp._sectName)]
    public class LockInAmp : HardwareConfigBase
    {
        #region Member Variables
        private const String _sectName = "lockInAmp";
        private const String _typeKey = "type";
        private const String _sensitivityKey = "sensitivity";
        private const String _sensUnitsKey = "units";
        private const String _intTimeKey = "intTime";
        private const String _intTimeUnitsKey = "intTimeUnits";
        private const String _fullScaleKey = "fullScale";
        private const String _inputFreqencyKey = "inputFrequency";
        private const String _dataAcqIntervalKey = "dataAcqInterval";
        private const String _dataAcqPointsKey = "dataAcqPoints";
        private const String _dataAcqRateKey = "dataAcqRate";
        #endregion


        #region Public Properties
        /// <summary>
        /// Type determines the implementation of the Lock-In Amp
        /// to be used for a New Spectrum.
        /// </summary>
        [ConfigurationProperty(LockInAmp._typeKey, DefaultValue = Enums.LockInType.Hardware, IsRequired = false)]
        public Enums.LockInType Type 
        {
            get { return (Enums.LockInType)Enum.Parse(typeof(Enums.LockInType), this[LockInAmp._typeKey].ToString()); }
            set { this[LockInAmp._typeKey] = value; }
        }


        /// <summary>
        /// Sensitivity determines the UInt value of the Lock-In
        /// Amplifier sensitivity.
        /// </summary>
        [ConfigurationProperty(LockInAmp._sensitivityKey, DefaultValue = 1U, IsRequired = false)]
        public UInt32 Sensitivity
        {
            get { return Convert.ToUInt32(this[LockInAmp._sensitivityKey]); }
            set { this[LockInAmp._sensitivityKey] = value; }
        }


        /// <summary>
        /// Unit determines the Enum value of the Lock-In
        /// Amplifier sensitivity.
        /// </summary>
        [ConfigurationProperty(LockInAmp._sensUnitsKey, DefaultValue = Enums.SensitivityUnit.Nanovolts, IsRequired = false)]
        public Enums.SensitivityUnit Unit
        {
            get { return (Enums.SensitivityUnit)Enum.Parse(typeof(Enums.SensitivityUnit), this[LockInAmp._sensUnitsKey].ToString()); }
            set { this[LockInAmp._sensUnitsKey] = value; }
        }


        /// <summary>
        /// IntegrationTime determines the UInt value of the Lock-In
        /// Amplifier integration time constant.
        /// </summary>
        [ConfigurationProperty(LockInAmp._intTimeKey, DefaultValue = 1U, IsRequired = false)]
        public UInt32 IntegrationTime
        {
            get { return Convert.ToUInt32(this[LockInAmp._intTimeKey]); }
            set { this[LockInAmp._intTimeKey] = value; }
        }


        /// <summary>
        /// IntegrationTimeUnit determines the Enum value of the Lock-In
        /// Amplifier integration time constant units.
        /// </summary>
        [ConfigurationProperty(LockInAmp._intTimeUnitsKey, DefaultValue = Enums.IntegrationUnit.Seconds, IsRequired = false)]
        public Enums.IntegrationUnit IntegrationTimeUnit
        {
            get { return (Enums.IntegrationUnit)Enum.Parse(typeof(Enums.IntegrationUnit), this[LockInAmp._intTimeUnitsKey].ToString()); }
            set { this[LockInAmp._intTimeUnitsKey] = value; }
        }


        /// <summary>
        /// FullScale determines the output full scale voltage
        /// for the Lock-In Amplifier.
        /// </summary>
        [ConfigurationProperty(LockInAmp._fullScaleKey, DefaultValue = 10.0, IsRequired = false)]
        public Double FullScale
        {
            get { return Convert.ToDouble(this[LockInAmp._fullScaleKey]); }
            set { this[LockInAmp._fullScaleKey] = value; }
        }


        /// <summary>
        /// InputFrequency determines the input reference frequency
        /// for the Lock-In Amplifier.
        /// </summary>
        [ConfigurationProperty(LockInAmp._inputFreqencyKey, DefaultValue = 13.0, IsRequired = false)]
        public Double InputFrequency
        {
            get { return Convert.ToDouble(this[LockInAmp._inputFreqencyKey]); }
            set { this[LockInAmp._inputFreqencyKey] = value; }
        }



        /// <summary>
        /// DataAcquisitionCycleInterval determines the number of 
        /// ms between acquisition cycles on the LockIn.Amplifier.
        /// </summary>
        [ConfigurationProperty(LockInAmp._dataAcqIntervalKey, DefaultValue = 250U, IsRequired = false)]
        public UInt32 DataAcquisitonCycleInterval
        {
            get { return Convert.ToUInt32(this[LockInAmp._dataAcqIntervalKey]); }
            set { this[LockInAmp._dataAcqIntervalKey] = value; }
        }


        /// <summary>
        /// DataAcquisitionPointsPerCycle determines the number of 
        /// points to acquire per cycle by the LockIn.Amplifier.
        /// </summary>
        [ConfigurationProperty(LockInAmp._dataAcqPointsKey, DefaultValue = 250U, IsRequired = false)]
        public UInt32 DataAcquisitonPointsPerCycle
        {
            get { return Convert.ToUInt32(this[LockInAmp._dataAcqPointsKey]); }
            set { this[LockInAmp._dataAcqPointsKey] = value; }
        }


        /// <summary>
        /// DataAcquisitionCycleRate determines the rate in Hz
        /// that data should be acquired from the board.
        /// </summary>
        [ConfigurationProperty(LockInAmp._dataAcqRateKey, DefaultValue = 1000U, IsRequired = false)]
        public UInt32 DataAcquisitionCycleRate
        {
            get { return Convert.ToUInt32(this[LockInAmp._dataAcqRateKey]); }
            set { this[LockInAmp._dataAcqRateKey] = value; }
        }
        #endregion
    }
}
