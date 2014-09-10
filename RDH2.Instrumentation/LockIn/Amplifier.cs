using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.LockIn
{
    /// <summary>
    /// The Amplifier class is the main class used to 
    /// resolve a signal from a modulated input.
    /// </summary>
    public class Amplifier : IDisposable
    {
        #region Static Singleton Instance and Access Property
        private static Amplifier _singleton = null;
        private static Object _singletonLock = new Object();


        /// <summary>
        /// The Current property is the way to access the 
        /// LockIn.Amplifier object as the constructor is 
        /// private.  This ensures that there is only ever
        /// one Amplifier in existance at one time.
        /// </summary>
        public static Amplifier Current
        {
            get
            {
                //Lock the property so that multiple threads don't
                //try to create the Singleton as the same time
                lock (Amplifier._singletonLock)
                {
                    //If the singleton is null, create and Initialize it
                    if (Amplifier._singleton == null)
                    {
                        Amplifier._singleton = new Amplifier(DAQ.DaqFactory.ConfiguredCard);
                        Amplifier._singleton.Initialize();
                    }
                }

                //Return the result
                return Amplifier._singleton;
            }
        }
        #endregion


        #region Member Variables
        private DAQ.DaqBase _board = null;
        private FrequencyDetector _freqDet = null;
        private ReferenceGenerator _refGen = null;
        private Boolean _isInitialized = false;

        //Data Acquisition objects
        private Double _phase = 0.0;
        private UInt32 _cycleInterval = 250U;
        private UInt32 _pointsPerCycle = 250U;
        private UInt32 _cycleRate = 1000U;
        private Object _valueLock = new Object();
        private Double _currentValue = 0.0;
        private Timer _timer = null;
        #endregion


        #region Constructor 
        /// <summary>
        /// Default Constructor for the Amplifier class
        /// </summary>
        /// <param name="board">The DaqBase used to acquire the data</param>
        private Amplifier(DAQ.DaqBase board)
        {
            //Check the input
            if (board == null)
                throw new System.ArgumentNullException("board", "No card is configured for use with the LockIn.Amplifier.");

            //Save the member variables
            this._board = board;
        }
        #endregion


        #region Startup / Shutdown Methods
        /// <summary>
        /// Initialize does any setup that is required prior 
        /// to using the Lock-In Amplifer.
        /// </summary>
        private void Initialize()
        {
            //Create the FrequencyDetector and initialize it
            this._freqDet = new FrequencyDetector(this._board);
            this._freqDet.Initialize();

            //Create the ReferenceGenerator and initialize it
            this._refGen = new ReferenceGenerator();
            this._refGen.Initialize();

            //Get the config and setup the values -- these are
            //unable to change during the lifetime of the Amplifier
            ConfigHelper<Config.LockInAmp> liaConfig = new ConfigHelper<Config.LockInAmp>(ConfigLocation.AllUsers);
            Config.LockInAmp lia = liaConfig.GetConfig();
            this._cycleInterval = lia.DataAcquisitonCycleInterval;
            this._pointsPerCycle = lia.DataAcquisitonPointsPerCycle;
            this._cycleRate = lia.DataAcquisitionCycleRate;

            //Start the Timer going to begin acquiring data
            this._timer = new 
                Timer(new TimerCallback(this.ProcessValue), null, this._cycleInterval, this._cycleInterval);

            //Set the value of the Initialized flag
            this._isInitialized = true;
        }
        #endregion


        #region Timer Callback
        /// <summary>
        /// ProcessValue does the actual work of the 
        /// Lock-In Amplifier.  It is called into periodically
        /// when the Timer fires.
        /// </summary>
        /// <param name="state"></param>
        private void ProcessValue(Object state)
        {
            //Turn off the Timer so that the function doesn't
            //end up re-entrant
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);

            //Get the Sin values from the ReferenceGenerator
            Double[] reference = this._refGen.CalculateWave(this._pointsPerCycle, this._freqDet.Frequency, this._phase);

            //Read an Array of data from the Board
            Double[] input = //new Double[this._pointsPerCycle];  
                this._board.ReadVoltageArray(this._pointsPerCycle, this._cycleRate);

            //Multiply the input by twice the reference
            Double[] product = this.MultiplyArrays(input, reference);

            //Run the result through a low-pass filter
            Double[] filtered = LowPass.Filter(product, this._freqDet.Frequency, this._cycleRate);

            //Finally, put the value in the member variable
            lock (this._valueLock)
            {
                this._currentValue = this.AverageArray(filtered);
            }

            //Turn the Timer back on
            this._timer.Change(this._cycleInterval, this._cycleInterval);
        }
        #endregion


        #region Lock-In Amp Phase Operations
        /// <summary>
        /// OptimizePhase adjusts the phase of the reference signal
        /// until the detected signal is optimized.
        /// </summary>
        public void OptimizePhase()
        {
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// SignalVoltage is the measured input voltage of the 
        /// DC signal deconvolved from the modulation.
        /// </summary>
        public Double SignalVoltage
        {
            get
            {
                //Lock on the Value Lock
                lock (this._valueLock)
                {
                    return this._currentValue;
                }
            }
        }


        /// <summary>
        /// InputFrequency is the frequency of the signal input
        /// into the Lock-In Amplifier to provide a reference 
        /// modulation signal.
        /// </summary>
        public Double InputFrequency
        {
            get 
            {
                //If the LIA hasn't been initialized, throw an Exception
                this.CheckInitialized();

                //Return the detected Frequency
                return this._freqDet.Frequency; 
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// CheckInitialized checks to make sure that the Initialize
        /// method has been called prior to setting or getting any values.
        /// </summary>
        private void CheckInitialized()
        {
            //If the Amplifier hasn't been initialized, throw an Exception
            if (this._isInitialized == false)
                throw new InvalidOperationException("Lock-In Amplifier has not been Initialized.");
        }


        /// <summary>
        /// MultiplyArrays takes the input and multiplies
        /// it by twice the reference.
        /// </summary>
        /// <param name="input">The read data from the board</param>
        /// <param name="reference">The reference data from the generator</param>
        /// <returns>Multiplied data in an Array</returns>
        private Double[] MultiplyArrays(Double[] input, Double[] reference)
        {
            //Declare a variable to return
            Double[] rtn = new Double[this._pointsPerCycle];

            //Iterate through the values and multiply them
            for (Int32 i = 0; i < this._pointsPerCycle; i++)
                rtn[i] = input[i] * (2 * reference[i]);

            //Return the result
            return rtn;
        }


        /// <summary>
        /// AverageArray takes the input data and averages all of
        /// the values into a single point.
        /// </summary>
        /// <param name="input">The Array of data to average</param>
        /// <returns>The average value of the data</returns>
        private Double AverageArray(Double[] input)
        {
            //Declare a value to return
            Double rtn = 0.0;

            //Get the number of points in the Array
            Int32 points = input.GetLength(0);

            //If there is data in the Array, process it
            if (points > 0)
            {
                //Get the sum of the data
                Double sum = 0.0;
                for (Int32 i = 0; i < points; i++)
                    sum += input[i];

                //Calculate the Average
                rtn = sum / points;
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region IDisposable Members
        /// <summary>
        /// Dispose cleans up the objects that were created 
        /// to run the LockIn.Amplifier.
        /// </summary>
        public void Dispose()
        {
            //Kill off the acquistion Timer
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);

            //Kill off the FrequencyDetector
            this._freqDet.Dispose();
        }
        #endregion
    }
}
