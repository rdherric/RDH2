using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.LockIn.Util
{
    /// <summary>
    /// ReferenceGenerator takes an input Frequency and 
    /// creates a Sine wave based on the offset of time
    /// since the generator was started.
    /// </summary>
    internal class ReferenceGenerator
    {
        #region Member Variables
        private Int32 _samplingRate = 1000;
        private Double _frequency = 13;
        private Boolean _isInitialized = false;
        private Object _startLock = new Object();
        private DateTime _start = DateTime.MinValue;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the ReferenceGenerator object.
        /// </summary>
        /// <param name="samplingRate">The rate at which the wave should be sampled</param>
        /// <param name="frequency">The frequency of the resulting wave</param>
        public ReferenceGenerator(Int32 samplingRate, Double frequency)
        {
            //Save the input in the member variables
            this._samplingRate = samplingRate;
            this._frequency = frequency;
        }
        #endregion


        #region Initialize Method
        /// <summary>
        /// Initialize sets the DateTime so that the sine
        /// wave begins at that point in time.
        /// </summary>
        public void Initialize()
        {
            //Set the DateTime
            lock (this._startLock)
            {
                this._start = DateTime.Now;
            }

            //Set the flag 
            this._isInitialized = true;
        }
        #endregion


        #region Wave Generation Methods
        /// <summary>
        /// CalculateWave calculates the time since the object
        /// was initialized and returns an Array of the Cosine
        /// wave based on the input frequency and phase.
        /// </summary>
        /// <param name="numPoints">The Number of Points to generate</param>
        /// <param name="phase">The phase in Degrees with which to generate the wave</param>
        /// <returns>Array of Wave data</returns>
        public Double[] CalculateWave(Int32 numPoints, Double phase)
        {
            //Declare a variable to return 
            Double[] rtn = new Double[numPoints];

            //Check to make sure that the object is initialized
            this.CheckInitialized();

            //Calculate the values if there are any to make
            if (numPoints > 0)
            {
                //Calculate the time since initialization in a Thread-safe manner
                Double timeFromStart = 0.0;
                lock (this._startLock)
                {
                    timeFromStart = (DateTime.Now - this._start).TotalSeconds;
                }

                //Calculate the phase in radians
                Double radPhase = (Math.PI / 180) * phase;

                //Calculate the amount of time between points -- should
                //simply be the sampling rate
                Double pointPeriod = 1.0 / Convert.ToDouble(this._samplingRate);

                //Iterate through the values and calculate them
                for (Int32 i = 0; i < numPoints; i++)
                {
                    //Calculate the time from the period
                    Double currTime = timeFromStart + (pointPeriod * i);

                    //Calculate the current value
                    rtn[i] = Math.Cos((currTime * this._frequency * 2 * Math.PI) + radPhase);
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
