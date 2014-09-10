using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Instrumentation.Enums
{
    /// <summary>
    /// CurrentUnit is used to determine the units 
    /// on the Potentiostat Current Range.
    /// </summary>
    public enum CurrentUnit
    {
        Invalid = -1,
        Nanoamps = 0,
        Microamps = 1,
        Milliamps = 2,
        Amps = 3
    }


    /// <summary>
    /// CurrentExponent is a class to translate the CurrentUnit
    /// to a Double Exponent for scaling.
    /// </summary>
    public class CurrentExponent
    {
        #region Const Definitions
        private const Double _nanoAmps = 1E-9;
        private const Double _microAmps = 1E-6;
        private const Double _milliAmps = 1E-3;
        private const Double _amps = 1.0;
        #endregion


        /// <summary>
        /// UnitToDivisor returns the actual value of the 
        /// Enum CurrentUnit.  
        /// </summary>
        /// <param name="unit">The Unit to translate</param>
        /// <returns>Double exponent that represents the Enum</returns>
        public static Double UnitToDivisor(CurrentUnit unit)
        {
            //Declare a variable to return
            Double rtn = -1;

            //Translate the PowerUnit
            switch (unit)
            {
                case CurrentUnit.Nanoamps:
                    rtn = CurrentExponent._nanoAmps;
                    break;

                case CurrentUnit.Microamps:
                    rtn = CurrentExponent._microAmps;
                    break;

                case CurrentUnit.Milliamps:
                    rtn = CurrentExponent._milliAmps;
                    break;

                case CurrentUnit.Amps:
                    rtn = CurrentExponent._amps;
                    break;
            }

            //Return the result
            return rtn;
        }
    }
}
