using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Instrumentation.Enums
{
    /// <summary>
    /// IntegrationUnit defines the units on the Time Constant
    /// for the LockIn.Amplifier class.
    /// </summary>
    public enum IntegrationUnit
    {
        Invalid = -1,
        Microseconds = 0,
        Milliseconds = 1,
        Seconds = 2
    }


    /// <summary>
    /// IntegrationDivisor is a class to translate the IntegrationUnit
    /// to a Double Exponent for scaling.
    /// </summary>
    public class IntegrationExponent
    {
        #region Const Definitions
        private const Double _microSecs = 1E-6;
        private const Double _milliSecs = 1E-3;
        private const Double _secs = 1.0;
        #endregion


        /// <summary>
        /// UnitToDivisor returns the actual value of the 
        /// Enum IntegrationUnit.  
        /// </summary>
        /// <param name="unit">The Unit to translate</param>
        /// <returns>Double exponent that represents the Enum</returns>
        public static Double UnitToDivisor(IntegrationUnit unit)
        {
            //Declare a variable to return
            Double rtn = -1;

            //Translate the PowerUnit
            switch (unit)
            {
                case IntegrationUnit.Microseconds:
                    rtn = IntegrationExponent._microSecs;
                    break;

                case IntegrationUnit.Milliseconds:
                    rtn = IntegrationExponent._milliSecs;
                    break;

                case IntegrationUnit.Seconds:
                    rtn = IntegrationExponent._secs;
                    break;
            }

            //Return the result
            return rtn;
        }
    }

}
