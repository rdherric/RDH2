using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Instrumentation.Enums
{
    /// <summary>
    /// SensitivityUnit is used to determine the units 
    /// on the Lock-In Amplifier Sensitivity.
    /// </summary>
    public enum SensitivityUnit
    {
        Invalid = -1,
        Nanovolts = 0,
        Microvolts = 1,
        Millivolts = 2,
        Volts = 3
    }


    /// <summary>
    /// SensitivityDivisor is a class to translate the SensitivityUnit
    /// to a Double Exponent for scaling.
    /// </summary>
    public class SensitivityExponent
    {
        #region Const Definitions
        private const Double _nanoVolts = 1E-9;
        private const Double _microVolts = 1E-6;
        private const Double _milliVolts = 1E-3;
        private const Double _volts = 1.0;
        #endregion


        /// <summary>
        /// UnitToDivisor returns the actual value of the 
        /// Enum SensitivityUnit.  
        /// </summary>
        /// <param name="unit">The Unit to translate</param>
        /// <returns>Double exponent that represents the Enum</returns>
        public static Double UnitToDivisor(SensitivityUnit unit)
        {
            //Declare a variable to return
            Double rtn = -1;

            //Translate the PowerUnit
            switch (unit)
            {
                case SensitivityUnit.Nanovolts:
                    rtn = SensitivityExponent._nanoVolts;
                    break;

                case SensitivityUnit.Microvolts:
                    rtn = SensitivityExponent._microVolts;
                    break;

                case SensitivityUnit.Millivolts:
                    rtn = SensitivityExponent._milliVolts;
                    break;

                case SensitivityUnit.Volts:
                    rtn = SensitivityExponent._volts;
                    break;
            }

            //Return the result
            return rtn;
        }
    }
}
