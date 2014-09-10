using System;
using System.Collections.Generic;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.DAQ
{
    /// <summary>
    /// DaqBase defines the interface for a Data Acquisition
    /// board and the various functions that it provides.
    /// </summary>
    public abstract class DaqBase
    {
        #region Member Variables
        private Config.DaqInterface _daqConfig = null;
        private Double _stepsPerNm = 20;
        private Double _partialPulse = 0;
        private Enums.LockInType _liaType = Enums.LockInType.Hardware;

        protected UInt32 _mirrorMin = 0;
        protected UInt32 _mirrorMax = 10000;

        //Lock objects to keep multiple threads from 
        //messing up the DAQ card...just in case
        private Object _daqReadLock = new Object();
        private Object _daqWriteLock = new Object();
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the DaqBase object.
        /// </summary>
        internal DaqBase()
        {
            //Get the Monochromator setup and save the
            //value of Steps Per nm
            ConfigHelper<Config.Monochromator> mcConfig = new ConfigHelper<Config.Monochromator>(ConfigLocation.AllUsers);
            Config.Monochromator mc = mcConfig.GetConfig();
            this._stepsPerNm = mc.StepsPerNm;

            //Get the Lock-In Amplifier config and check
            //to see if the software LIA is being used
            ConfigHelper<Config.LockInAmp> liaConfig = new ConfigHelper<Config.LockInAmp>(ConfigLocation.AllUsers);
            Config.LockInAmp lia = liaConfig.GetConfig();
            this._liaType = lia.Type;
        }
        #endregion


        #region Voltage Read Methods
        /// <summary>
        /// ReadVoltage acquires and averages data from the DAQ
        /// board.
        /// </summary>
        /// <param name="samplesToAvg">The Number of samples to average into the output</param>
        /// <returns>Double voltage from the DAQ card</returns>
        public Double ReadVoltage(Int32 samplesToAvg)
        {
            //Declare a variable to return
            Double rtn = 0.0;

            //If the Lock-In Amplifier type is Software, get
            //the value from the Singleton.  Otherwise, get 
            //the value from the DAQ input.
            if (this._liaType == Enums.LockInType.Software)
                rtn = LockIn.Amplifier.Current.SignalVoltage;
            else
            {
                //Lock the card
                lock (this._daqReadLock)
                {
                    rtn = this.InternalReadVoltage(samplesToAvg);
                }
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// ReadVoltageArray reads the required data into and Array
        /// for processing by the LockIn.Amplifier.  This function
        /// is internal for that reason.
        /// </summary>
        /// <param name="pointsToRead">The number of points to read from the card</param>
        /// <param name="samplingRate">The sampling rate at which to read the data</param>
        /// <returns>The array of data from the card</returns>
        internal Double[] ReadVoltageArray(UInt32 pointsToRead, UInt32 samplingRate)
        {
            return this.InternalReadVoltageArray(pointsToRead, samplingRate);
        }


        /// <summary>
        /// InternalReadVoltage is implemented by the inherited class
        /// to do the actual reading.
        /// </summary>
        /// <param name="samplesToAvg">The number of samples to average into the output</param>
        /// <returns></returns>
        protected abstract Double InternalReadVoltage(Int32 samplesToAvg);


        /// <summary>
        /// InternalReadVoltage is implemented by the inherited class 
        /// to do the actual reading.
        /// </summary>
        /// <param name="pointsToRead">The number of points to read</param>
        /// <param name="samplingRate">The sampling rate at which to read the data</param>
        /// <returns>The array of data returned by the card</returns>
        protected abstract Double[] InternalReadVoltageArray(UInt32 pointsToRead, UInt32 samplingRate);
        #endregion


        #region 2-D Spectra Methods
        /// <summary>
        /// SetupMonochromator sets up the monochromator to
        /// take a scan.
        /// </summary>
        /// <param name="direction"></param>
        public void SetupMonochromator(Int32 direction)
        {
            //Lock the DAQ
            lock (this._daqWriteLock)
            {
                this.InternalSetupMonochromator(direction);
            }
        }


        /// <summary>
        /// MoveMonochromator moves the monochromator by the
        /// specified number of nm.
        /// </summary>
        public void MoveMonochromator(Int32 numNm)
        {
            //Lock the DAQ
            lock (this._daqWriteLock)
            {
                this.InternalMoveMonochromator(numNm);
            }
        }


        /// <summary>
        /// ShutdownMonochromator shuts down the monochromator 
        /// after a scan.
        /// </summary>
        public void ShutdownMonochromator()
        {
            //Lock the DAQ
            lock (this._daqWriteLock)
            {
                this.InternalShutdownMonochromator();
            }
        }


        /// <summary>
        /// InternalSetupMonochromator is implemented by the inherited
        /// class and does the actual setup.
        /// </summary>
        /// <param name="direction">Positive to scan up, negative to scan down</param>
        protected abstract void InternalSetupMonochromator(Int32 direction);


        /// <summary>
        /// InternalMoveMonochromator is implemented by the inherited
        /// class and does the actual moving.
        /// </summary>
        /// <param name="numNm">The number of nm to move the monochromator</param>
        protected abstract void InternalMoveMonochromator(Int32 numNm);


        /// <summary>
        /// InternalShutdownMonochromator is implemented by the inherited
        /// class and does the actual shutdown.
        /// </summary>
        protected abstract void InternalShutdownMonochromator();
        #endregion


        #region 3-D Methods
        /// <summary>
        /// MoveMirror moves either an X or Y-Axis mirror 
        /// to the desired offset in a 3-D Data Acquisition.
        /// </summary>
        /// <param name="axis">The Axis to move</param>
        /// <param name="offset">The offset to which to move it: 0 - 5000</param>
        public void MoveMirror(Axis axis, UInt32 offset)
        {
            //Lock the DAQ
            lock (this._daqWriteLock)
            {
                this.InternalMoveMirror(axis, offset);
            }
        }


        /// <summary>
        /// InternalMoveMirror is implemented by the inherited class
        /// and does the actual move.
        /// </summary>
        /// <param name="axis">The Axis for which to move the mirror</param>
        /// <param name="offset">The offset to which to move the mirror</param>
        protected abstract void InternalMoveMirror(Axis axis, UInt32 offset);
        #endregion


        #region Software Lock-In Amplifier Methods
        /// <summary>
        /// ReadLockInCounter reads the value of the counter 
        /// used to detect input modulation frequency.
        /// </summary>
        /// <returns>Int32 counter value</returns>
        public Int32 ReadLockInCounter()
        {
            return this.InternalReadLockInCounter();
        }


        /// <summary>
        /// ClearLockInCounter clears the value of the counter 
        /// used to detect input modulation frequency.
        /// </summary>
        public void ClearLockInCounter()
        {
            this.InternalClearLockInCounter();
        }


        /// <summary>
        /// InternalReadLockInCounter is implemented by the inherited
        /// class and does the actual read.
        /// </summary>
        /// <returns>Int32 counter value</returns>
        protected abstract Int32 InternalReadLockInCounter();


        /// <summary>
        /// InternalClearLockInCounter is implemented by the inherited
        /// class and does the clearing of the Counter.
        /// </summary>
        protected abstract void InternalClearLockInCounter();
        #endregion


        #region Public Properties
        /// <summary>
        /// BoardName returns the name of the actual DAQ board.
        /// </summary>
        public abstract String BoardName { get; }


        /// <summary>
        /// Type returns the actual type of the Data Acquisition
        /// card as an Enum
        /// </summary>
        public abstract Enums.DaqType Type { get; }


        /// <summary>
        /// AnalogInPorts returns the available analog in
        /// ports on the board.
        /// </summary>
        public abstract String[] AnalogInPorts { get; }


        /// <summary>
        /// AnalogInPorts returns the available analog in
        /// ports on the board.
        /// </summary>
        public abstract String[] AnalogOutPorts { get; }

        
        /// <summary>
        /// DigitalOutBytes returns the available 8-bit digital
        /// out ports on the board.
        /// </summary>
        public abstract String[] DigitalOutBytes { get; }


        /// <summary>
        /// CounterPorts returns the available Counter ports
        /// on the board.
        /// </summary>
        public abstract String[] CounterPorts { get; }


        /// <summary>
        /// DigitalOutPorts returns the available Digital Out
        /// ports on the board,
        /// </summary>
        public abstract String[] DigitalOutPorts { get; }
        #endregion


        #region Protected Properties
        /// <summary>
        /// Config returns the DaqInterface Configuration 
        /// object to the derived classes.
        /// </summary>
        protected Config.DaqInterface DaqInterfaceConfig
        {
            get
            {
                //If the Config object hasn't been retrieved,
                //retrieve it.
                if (this._daqConfig == null)
                {
                    ConfigHelper<Config.DaqInterface> config = new ConfigHelper<Config.DaqInterface>(ConfigLocation.AllUsers);
                    this._daqConfig = config.GetConfig();
                }

                //Return the result
                return this._daqConfig;
            }
        }


        /// <summary>
        /// AcquisitionDelay returns the number of ms to wait in
        /// order to sample at the desired sampling rate.
        /// </summary>
        protected Int32 AcquisitionDelay
        {
            get
            {
                //Declare a variable to return
                Int32 rtn = 0;

                //Return the result
                return rtn;
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// GetNumSteps calculates the number of steps to move the
        /// monochromator by the specified number of nm.
        /// </summary>
        /// <param name="numNm">The number of nm to move the monochromator</param>
        /// <returns>The Integer number of steps to move</returns>
        protected Int32 GetNumSteps(Int32 numNm)
        {
            //Calculate the number of steps from the Config
            Double numSteps = this._stepsPerNm * Convert.ToDouble(Math.Abs(numNm));

            //Add the current number of steps to the Partial member
            this._partialPulse += numSteps;

            //Get the integer value of the pulses
            Int32 rtn = Convert.ToInt32(this._partialPulse);

            //Set the partial pulse back in the member
            this._partialPulse -= rtn;

            //Return the result
            return rtn;
        }
        #endregion

        
        #region AxisType Enum
        /// <summary>
        /// The Axis Enum is used to determine which 
        /// Mirror to move in a 3-D data acquisition.
        /// </summary>
        public enum Axis
        {
            X,
            Y
        }
        #endregion
    }
}
