using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.LockIn.Util
{
    /// <summary>
    /// LowPass takes an array of data and processes
    /// the high-frequency data out of it.
    /// </summary>
    internal class LowPass
    {
        /// <summary>
        /// Filter takes an Array of data and performs a Low-Pass
        /// digital filter operation on it.
        /// </summary>
        /// <param name="input">The data to be Filtered</param>
        /// <param name="passFrequency">The Frequency that needs to pass</param>
        /// <returns>Array of filtered data</returns>
        public static Double[] Filter(Double[] input, Double passFrequency, Double samplingFrequency)
        {
            //Get the length of the input Array
            Int32 inputLength = input.GetLength(0);

            //Declare a variable to return
            Double[] rtn = new Double[inputLength];

            //Calculate the necessary RC constant for the pass Frequency
            Double RC = 1 / (2 * Math.PI * passFrequency);

            //Calculate the delta time based on the sampling rate
            Double dTime = 1 / samplingFrequency;

            //Calculate the smoothing factor alpha
            Double alpha = dTime / (dTime + RC);

            //Transform the data
            rtn[0] = input[0];
            for (Int32 i = 1; i < inputLength; i++)
                rtn[i] = rtn[i - 1] + (alpha * (input[i] - rtn[i - 1]));

            //Return the result
            return rtn;
        }
    }
}
