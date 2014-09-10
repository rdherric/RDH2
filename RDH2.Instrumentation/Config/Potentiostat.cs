using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// Potentiostat retrieves the parameters set on the 
    /// Potentiostat from the app.config file.
    /// </summary>
    [ConfigurationSection(Potentiostat._sectName)]
    public class Potentiostat : HardwareConfigBase
    {
        #region Member Variables
        private const String _sectName = "potentiostat";
        private const String _currentRangeKey = "currentRange";
        private const String _currentUnitsKey = "units";
        private const String _fullScaleKey = "fullScale";
        #endregion


        #region Public Properties
        /// <summary>
        /// CurrentRange determines the last Double value of the 
        /// Potentiostat current range configured.
        /// </summary>
        [ConfigurationProperty(Potentiostat._currentRangeKey, DefaultValue = 1.0, IsRequired = false)]
        public Double CurrentRange
        {
            get { return Convert.ToDouble(this[Potentiostat._currentRangeKey]); }
            set { this[Potentiostat._currentRangeKey] = value; }
        }


        /// <summary>
        /// Unit determines the Enum value of the Potentiostat
        /// current range.
        /// </summary>
        [ConfigurationProperty(Potentiostat._currentUnitsKey, DefaultValue = Enums.CurrentUnit.Nanoamps, IsRequired = false)]
        public Enums.CurrentUnit Unit
        {
            get { return (Enums.CurrentUnit)Enum.Parse(typeof(Enums.CurrentUnit), this[Potentiostat._currentUnitsKey].ToString()); }
            set { this[Potentiostat._currentUnitsKey] = value; }
        }


        /// <summary>
        /// FullScale determines the Double value of the Full Scale
        /// voltage that the Potentiostat outputs.
        /// </summary>
        [ConfigurationProperty(Potentiostat._fullScaleKey, DefaultValue = 10.0, IsRequired = false)]
        public Double FullScale
        {
            get { return Convert.ToDouble(this[Potentiostat._fullScaleKey]); }
            set { this[Potentiostat._fullScaleKey] = value; }
        }
        #endregion
    }
}
