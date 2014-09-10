using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RDH2.Instrumentation.LockIn
{
    /// <summary>
    /// FrequencyDetector runs a background thread that reads the
    /// frequency from an optical chopper.  This input frequency
    /// is used to generate the sine wave for the Lock-In software.
    /// </summary>
    internal class FrequencyDetector : IDisposable
    {
        #region Member Variables
        private DAQ.DaqBase _board = null;
        private Timer _timer = null;
        private Int32 _readPeriod = 1000;
        private Object _freqLock = new Object();
        private Double _lastFrequency = Double.MinValue;
        private Double _frequency = 0.0;
        private Int32 _lastCount = 0;
        private DateTime _start = DateTime.MinValue;
        private Boolean _isInitialized = false;
        #endregion


        #region Constructor 
        /// <summary>
        /// Default constructor for the FrequencyDetector class.
        /// </summary>
        /// <param name="board">The DAQ board that will be used to get the Frequency</param>
        public FrequencyDetector(DAQ.DaqBase board)
        {
            //Save the member variables
            this._board = board;
        }
        #endregion


        #region Setup Method
        /// <summary>
        /// Initialize must be called prior to using the 
        /// FrequencyDetector.  It sets things up in the 
        /// member variables and clears the Counter.
        /// </summary>
        public void Initialize()
        {
            //Clear the Counter
            this._board.ClearLockInCounter();

            //Reset the time counter
            this._start = DateTime.Now;

            //Reset the last Counter value
            this._lastCount = 0;

            //Create and start the timer if necessary 
            if (this._timer == null)
            {
                this._timer = new Timer(new TimerCallback(this.ReadFrequency));
                this._timer.Change(this._readPeriod, this._readPeriod);
            }

            //Set the Initialized flag
            this._isInitialized = true;
        }
        #endregion


        #region Timer Callback
        /// <summary>
        /// ReadFrequency does the actual work of reading from the
        /// Counter, doing the calculation, and resetting the counter
        /// if necessary.
        /// </summary>
        /// <param name="state">Object passed in by the Timer -- always null</param>
        private void ReadFrequency(Object state)
        {
            //Hold the Timer
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);

            //Get the counter value from the DAQ member variable
            Int32 currentCount = this._board.ReadLockInCounter();

            //Calculate the number of pulses in this last period
            Int32 periodCount = currentCount - this._lastCount;

            //Calculate the frequency of the current period
            Double periodFreq = Convert.ToDouble(periodCount) / (Convert.ToDouble(this._readPeriod) / 1000.0);

            //If no frequency has been detected yet or the difference between 
            //detected and current is greater than 1 Hz, clear everything and
            //set the Frequency manually.  Otherwise, do the calculations.
            if (this._start == DateTime.MinValue || Math.Abs(this._frequency - periodFreq) > 1)
            {
                //Reinit the FrequencyDetector
                this.Initialize();

                //Set the Frequency directly
                this._frequency = periodFreq;
            }
            else
            {
                //Calculate the actual Frequency based on the amount of 
                //time that has elapsed 
                lock (this._freqLock)
                {
                    //Save the last Frequency value in the member variable
                    //so that the output can be buffered and only show valid
                    //frequencies when things have settled down
                    this._lastFrequency = this._frequency;

                    //Save the new frequency
                    this._frequency = Convert.ToDouble(currentCount) / (DateTime.Now - this._start).TotalSeconds;
                }
            }

            //Update the Counter
            this._lastCount = currentCount;

            //Restart the Timer
            this._timer.Change(this._readPeriod, this._readPeriod);
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// ReadPeriod defines the number of milliseconds between
        /// reads on the counter to determine the frequency of the
        /// input signal.
        /// </summary>
        public Int32 ReadPeriod
        {
            get 
            {
                //Check that the object is Initialized
                this.CheckInitialized();

                //Return the value
                return this._readPeriod;
            }
            set 
            {
                //Check that the object is Initialized
                this.CheckInitialized();

                //Save the value 
                this._readPeriod = value;
 
                //Set the Period on the Timer
                this._timer.Change(this._readPeriod, this._readPeriod);
            }
        }


        /// <summary>
        /// Frequency returns the determined frequency of the input
        /// signal in Hz.
        /// </summary>
        public Double Frequency
        {
            get 
            {
                //Check that the object is Initialized
                this.CheckInitialized();

                //Declare a variable to return
                Double rtn = 0.0;

                //Return the value
                lock (this._freqLock)
                {
                    if (Math.Abs(this._frequency - this._lastFrequency) < 1)
                        rtn = this._frequency;
                }

                //Return the result
                return rtn;
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// CheckInitialized checks to make sure that the Initialized
        /// method has been called prior to setting or getting any values.
        /// </summary>
        private void CheckInitialized()
        {
            //If the FrequencyDetector hasn't been initialized, throw an Exception
            if (this._isInitialized == false)
                throw new InvalidOperationException("Frequency Detector has not been Initialized.");
        }
        #endregion


        #region IDisposable Members
        /// <summary>
        /// Dispose cleans up the objects that were created to
        /// run the FrequencyDetector.
        /// </summary>
        public void Dispose()
        {
            //Kill the Timer
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        #endregion
    }
}
