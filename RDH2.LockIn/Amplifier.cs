using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RDH2.LockIn
{
    /// <summary>
    /// The Amplifier class collects modulated data from 
    /// the ILockInSource interface and turns it into a 
    /// low-noise signal.
    /// </summary>
    public class Amplifier : IDisposable
    {
        #region Member Variables
        private ILockInDataSource _dataSource = null;
        private ILockInModulationSource _modSource = null;
        private Util.ReferenceGenerator _refGen = null;
        private Util.Butterworth _butterworth = null;
        private Boolean _isRunning = false;

        //Data Acquisition objects
        private Object _valueLock = new Object();
        private Double _currentValue = 0.0;
        private Timer _timer = null;
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor for the LockIn.Amplifier.
        /// </summary>
        /// <param name="dataSource">The Data Source interface from which the Amplifier receives signal data</param>
        /// <param name="modSource">The Modulation Source interface from which the Amplifier recieves moduation data</param>
        public Amplifier(ILockInDataSource dataSource, ILockInModulationSource modSource)
        {
            //Check the input for validity
            if (dataSource == null)
                throw new System.ArgumentNullException("dataSource", "No ILockInDataSource is configured for use with the LockIn.Amplifier.");

            if (modSource == null)
                throw new System.ArgumentNullException("modSource", "No ILockInModulationSource is configured for use with the LockIn.Amplifier.");

            //Save the member variables
            this._dataSource = dataSource;
            this._modSource = modSource;

            //Hook up the event for synchronizing the 
            //Reference Generation
            this._modSource.ModulationHigh += new EventHandler(_modSource_ModulationHigh);
        }
        #endregion


        #region Startup / Shutdown Methods
        /// <summary>
        /// Start does any setup that is required prior 
        /// to using the Lock-In Amplifer.
        /// </summary>
        public void Start()
        {
            //Create the ReferenceGenerator and initialize it
            this._refGen = new Util.ReferenceGenerator(this._dataSource.CycleRate, this._modSource.InputFrequency);
            this._refGen.Initialize();

            //Create the Butterworth Filter
            this._butterworth = new Util.Butterworth(
                Convert.ToInt32(this._modSource.InputFrequency * 10d), 
                Convert.ToDouble(this._modSource.InputFrequency / 2));

            //Start the Timer going to begin acquiring data
            this._timer = new
                Timer(new TimerCallback(this.ProcessValue), null, this._dataSource.CycleInterval, this._dataSource.CycleInterval);

            //Set the value of the Running flag
            this._isRunning = true;
        }


        /// <summary>
        /// Stop stops the acquisition from the ILockInDataSource
        /// object.
        /// </summary>
        public void Stop()
        {
            //If the Amplifier isn't running, just return
            //with no further processing
            if (this._isRunning == false)
                return;

            //Cut off the Timer
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        #endregion


        #region Timer Callback
        /// <summary>
        /// ProcessValue does the actual work of the 
        /// Lock-In Amplifier.  It is called into periodically
        /// when the Timer fires.
        /// </summary>
        /// <param name="state">NULL state of the Timer</param>
        private void ProcessValue(Object state)
        {
            //Turn off the Timer so that the function doesn't
            //end up re-entrant
            this._timer.Change(Timeout.Infinite, Timeout.Infinite);

            //Get the reference values from the ReferenceGenerator
            Double[] cosRef = this._refGen.CalculateWave(this._dataSource.PointsPerCycle, 0.0);
            Double[] sinRef = this._refGen.CalculateWave(this._dataSource.PointsPerCycle, 90.0);

            //Read an Array of data from the Board
            Double[] input = this._dataSource.GetDataArray();

            //Multiply the input by twice the reference
            Double[] cosProduct = this.MultiplyArrays(input, cosRef);
            Double[] sinProduct = this.MultiplyArrays(input, sinRef);

            //Boxcar average the data to a different sampling rate
            Double[] cosBoxcar = Util.Boxcar.Average(cosProduct, this._dataSource.CycleRate, this._modSource.InputFrequency);
            Double[] sinBoxcar = Util.Boxcar.Average(sinProduct, this._dataSource.CycleRate, this._modSource.InputFrequency);

            //Run the result through a low-pass filter
            Double[] cosFiltered = Util.LowPass.Filter(cosBoxcar, this._modSource.InputFrequency / 4d, this._modSource.InputFrequency * 10d);
            Double[] sinFiltered = Util.LowPass.Filter(sinBoxcar, this._modSource.InputFrequency / 4d, this._modSource.InputFrequency * 10d);
            //Double[] cosFiltered = this._butterworth.LowPass(cosBoxcar);
            //Double[] sinFiltered = this._butterworth.LowPass(sinBoxcar);

            //Average the final data
            Double cosAvg = Util.Boxcar.AverageArray(cosFiltered, 0, cosFiltered.GetLength(0));
            Double sinAvg = Util.Boxcar.AverageArray(sinFiltered, 0, sinFiltered.GetLength(0));

            //Calculate the Magnitude of the Vector
            Double result = Math.Sqrt((cosAvg * cosAvg) + (sinAvg * sinAvg));

            //Finally, put the value in the member variable
            lock (this._valueLock)
            {
                this._currentValue = result;
            }

            //Turn the Timer back on
            this._timer.Change(this._dataSource.CycleInterval, this._dataSource.CycleInterval);
        }
        #endregion


        #region Phase Synchronization EventHandlers
        /// <summary>
        /// _source_ModulationHigh reinitializes the ReferenceGenerator
        /// to synchronize it with the high of an input signal.
        /// </summary>
        /// <param name="sender">The ILockInSource that reports the modulation high</param>
        /// <param name="e">The EventArgs sent by the System</param>
        void _modSource_ModulationHigh(object sender, EventArgs e)
        {
            //Initialize the ReferenceGenerator
            this._refGen.Initialize();

            //Get rid of the event now
            this._modSource.ModulationHigh -= this._modSource_ModulationHigh;
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
                return this._modSource.InputFrequency;
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
            if (this._isRunning == false)
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
            Double[] rtn = new Double[this._dataSource.PointsPerCycle];

            //Iterate through the values and multiply them
            for (Int32 i = 0; i < this._dataSource.PointsPerCycle; i++)
                rtn[i] = input[i] * (2 * reference[i]);

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
        }
        #endregion
    }
}
