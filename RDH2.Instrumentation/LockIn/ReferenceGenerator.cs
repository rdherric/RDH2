using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Instrumentation.LockIn
{
    /// <summary>
    /// ReferenceGenerator takes an input Frequency and 
    /// creates a Sine wave based on the offset of time
    /// since the generator was started.
    /// </summary>
    internal class ReferenceGenerator
    {
        #region Member Variables
        private Boolean _isInitialized = false;
        private DateTime _start = DateTime.MinValue;
        #endregion


        #region Initialize Method
        /// <summary>
        /// Initialize sets the DateTime so that the sine
        /// wave begins at that point in time.
        /// </summary>
        public void Initialize()
        {
            //Set the DateTime
            this._start = DateTime.Now;

            //Set the flag 
            this._isInitialized = true;
        }
        #endregion


        #region Wave Generation Methods
        /// <summary>
        /// CalculateWave calculates the time since the object
        /// was initialized and returns an Array of the Sine
        /// wave based on the input frequency and phase.
        /// </summary>
        public Double[] CalculateWave(UInt32 numPoints, Double frequency, Double phase)
        {
            //Declare a variable to return 
            Double[] rtn = new Double[numPoints];

            //Check to make sure that the object is initialized
            this.CheckInitialized();

            //Calculate the values if there are any to make
            if (numPoints > 0)
            {
                //Calculate the time since initialization
                Double timeFromStart = (DateTime.Now - this._start).TotalSeconds;

                //Iterate through the values and calculate them
                for (Int32 i = 0; i < numPoints; i++)
                {
                    //Calculate the time from the frequency
                    Double currTime = timeFromStart + ((1 / frequency) * i);

                    //Calculate the current value
                    rtn[i] = Math.Sin((currTime * frequency * Math.PI * 2) + phase);
                }
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Helper methods
        /// <summary>
        /// CheckInitialized checks to make sure that
        /// the flag has been set prior to performing 
        /// any actual work.
        /// </summary>
        private void CheckInitialized()
        {
            //If the Amplifier hasn't been initialized, throw an Exception
            if (this._isInitialized == false)
                throw new InvalidOperationException("Reference Generator has not been Initialized.");
        }
        #endregion
    }
}
