using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.LockIn.Util
{
    /// <summary>
    /// Boxcar is used to perform a boxcar average
    /// on a set of data.  
    /// </summary>
    internal class Boxcar
    {
        /// <summary>
        /// Average takes a set of data and performs the Boxcar
        /// average on it.  It takes full-sampled data and averages
        /// it down to 10 x the excitation frequency of the data.
        /// </summary>
        /// <param name="input">The data to be averaged</param>
        /// <param name="samplingRate">The rate at which data is sampled</param>
        /// <param name="excitationFrequency">The frequency at which the data is excited</param>
        /// <returns>Double Array of averaged data</returns>
        public static Double[] Average(Double[] input, Int32 samplingRate, Double excitationFrequency)
        {
            //Get the length of the input data
            Int32 inputLength = input.GetLength(0);

            //Get the sec / point of the input data
            Double inputPeriod = 1d / samplingRate;

            //Get the amount of time sampled in the input
            Double totalPeriod = inputPeriod * inputLength;

            //Get the Nyquist frequency of the excitation
            Int32 nyquistExcitation = Convert.ToInt32(10d * excitationFrequency);

            //Get the sec / point of the output data
            Double outputPeriod = 1d / nyquistExcitation;

            //Get the number of points in the output
            Int32 outputLength = Convert.ToInt32(Math.Ceiling(totalPeriod / outputPeriod));

            //Get the number of full-sample points in an 
            //averaged-sample unit
            Int32 dataLength = inputLength / outputLength;

            //Declare a variable to return
            Double[] rtn = new Double[outputLength];

            //Iterate through the data and average it
            Int32 inputIndex = 0;
            for (Int32 i = 0; i < outputLength; i++)
            {
                //Get the current set of data
                if (dataLength + inputIndex > inputLength)
                    dataLength = inputLength - inputIndex;

                //Average the data to a point
                rtn[i] = Boxcar.AverageArray(input, inputIndex, dataLength);

                //Increment the input index
                inputIndex += dataLength;
            }

            //Return the result
            return rtn;
        }


        #region Helper Methods
        /// <summary>
        /// AverageArray takes an input Array and averages
        /// it to a single point.
        /// </summary>
        /// <param name="input">The Array to average</param>
        /// <param name="startIndex">The Index at which to start averaging</param>
        /// <param name="length">The length of data to average</param>
        /// <returns>Double average of the contents of the Array</returns>
        public static Double AverageArray(Double[] input, Int32 startIndex, Int32 length)
        {
            //Declare a variable to hold the sum
            Double sum = 0.0;

            //Iterate through the data and sum it
            for (Int32 i = startIndex; i < startIndex + length; i++)
                sum += input[i];

            //Return the result
            return sum / length;
        }
        #endregion
    }
}
