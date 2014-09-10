using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.DAQ
{
    /// <summary>
    /// MCC is a DaqBase-derived class that represents a 
    /// Measurement Computing Data Acquisition board and
    /// the various functions that it provides.
    /// </summary>
    public class MCC : DaqBase, IDisposable
    {
        #region Member Variables
        private MccDaq.MccBoard _board = null;

        //Delay in ms between PortVal pulses
        private Int32 _mcDelay = 100;

	    //Declare an array of UInt16s to be passed to the DOut function
	    private UInt16[] _portVals = new UInt16[] {1, 9, 8, 10, 2, 6, 4, 5};

        //Member variables for continuous operation
        private Int32 _bufHandle = Int32.MinValue;
        private Boolean _contReadRunning = false;
        #endregion


        #region Internal Constructor
        /// <summary>
        /// The internal constructor makes sure that the MCC
        /// class can not be created outside the Hardware DLL.
        /// </summary>
        /// <param name="board">The MccBoard to wrap</param>
        internal MCC(MccDaq.MccBoard board)
        {
            //Save the member variables
            this._board = board;
        }
        #endregion


        #region Voltage Read Methods
        /// <summary>
        /// ReadVoltage reads the Voltage from the DAQ card.
        /// </summary>
        /// <param name="samplesToAvg">The number of samples to average for the output</param>
        /// <returns>Double Voltage averaged over the number of samples</returns>
        protected override Double InternalReadVoltage(Int32 samplesToAvg)
        {
            //Declare a variable to return
            Double rtn = 0.0;

            //Loop through the Samples to Average and acquire
            //the data
    		Double totalVoltage = 0;

            //Get the Channel number from Config
            Int32 channel = Int32.Parse(this.DaqInterfaceConfig.VoltageInPort);

            //Iterate through the number of steps and get the result
            MccDaq.ErrorInfo ei = null;
            for (Int32 i = 0; i < samplesToAvg; i++)
            {
                //Get the value from the card
                UInt16 cardValue = 0;
                ei = this._board.AIn(channel, MccDaq.Range.Bip10Volts, out cardValue);

                //Declare a double to hold the actual voltage
                Single cardVoltage = 0.0F;

                //If the value was retrieved, translate it
                if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
                    ei = this._board.ToEngUnits(MccDaq.Range.Bip10Volts, cardValue, out cardVoltage);

                //Add the value to the loop output
                if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
                    totalVoltage += Convert.ToDouble(cardVoltage);
                else
                    break;

                //Wait for the specified # of ms
                Thread.Sleep(this.AcquisitionDelay);
            }

            //If the calls have not returned an error, set the output
            if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
                rtn = totalVoltage / samplesToAvg;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// InternalReadVoltageArray reads the data from the card at
        /// the specified sampling rate.  The board must support a FIFO
        /// kind of buffer internally.
        /// </summary>
        /// <param name="pointsToRead">The number of points to read</param>
        /// <param name="samplingRate">The rate at which to read the data</param>
        /// <returns>The data from the card</returns>
        protected override double[] InternalReadVoltageArray(UInt32 pointsToRead, UInt32 samplingRate)
        {
            //Declare a variable to return
            Double[] rtn = new Double[pointsToRead];

            //Convert the UInt32 for the MCC functions
            Int32 numPoints = Convert.ToInt32(pointsToRead);

            //Begin reading data from the board if it hasn't started yet
            this.BeginReadingArray(pointsToRead, samplingRate);

            //If the board is currently running, get the data
            if (this._contReadRunning == true)
            {
                //Declare an Array to hold the data from the 
                //Windows buffer
                Double[,] output = new Double[pointsToRead, pointsToRead];

                //Get the data from the Windows buffer
                MccDaq.ErrorInfo ei =
                    this._board.WinBufToEngArray(MccDaq.Range.Bip10Volts, this._bufHandle, output, 0, numPoints, 1);

                //If the get was successful, copy the data to the return variable
                if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
                {
                    //Copy the data
                    for (Int32 i = 0; i < pointsToRead; i++)
                        rtn[i] = output[0, i];
                }
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// BeginReadingArray starts the MCC board reading the data
        /// that is coming in continuously into a Windows buffer.
        /// </summary>
        /// <param name="pointsToRead">The number of points in the Windows buffer</param>
        /// <param name="samplingRate">The sampling rate at which to save data</param>
        private void BeginReadingArray(UInt32 pointsToRead, UInt32 samplingRate)
        {
            //If the board is already collecting data, just return
            if (this._contReadRunning == true)
                return;

            //Convert the UInt32s to Int32s
            Int32 numPoints = Convert.ToInt32(samplingRate);
            Int32 rate = Convert.ToInt32(samplingRate);

            //Get the Channel number from Config
            Int32 channel = Int32.Parse(this.DaqInterfaceConfig.VoltageInPort);

            //Allocate the buffer in a Windows Memory Handle if 
            //it hasn't been done yet
            if (this._bufHandle == Int32.MinValue)
                this._bufHandle = MccDaq.MccService.WinBufAlloc(numPoints);

            //Begin reading into the Windows buffer
            MccDaq.ErrorInfo ei = this._board.AInScan(channel, channel, numPoints, ref rate, 
                MccDaq.Range.Bip10Volts, this._bufHandle, 
                MccDaq.ScanOptions.Background | MccDaq.ScanOptions.Continuous | MccDaq.ScanOptions.SingleIo);

            //Set the flag so that this is not called again if the
            //start was successful
            if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
                this._contReadRunning = true;
        }
        #endregion


        #region 2-D Spectra Methods
        /// <summary>
        /// SetupMonochromator sets up the Digitial Out Port
        /// used to move the monoschromator.
        /// </summary>
        /// <param name="direction">Int32 direction parameter -- not used</param>
        protected override void InternalSetupMonochromator(int direction)
        {
            //Get the Port from the Config and change it to an Enum
            MccDaq.DigitalPortType dpt = (MccDaq.DigitalPortType)
                Enum.Parse(typeof(MccDaq.DigitalPortType), this.DaqInterfaceConfig.StepperOutPort);

            //Call the function
            MccDaq.ErrorInfo ei = this._board.DConfigPort(dpt, MccDaq.DigitalPortDirection.DigitalOut);
        }

        
        /// <summary>
        /// MoveMonochromator moves the monochromator the specified
        /// number of nm by pulsing the stepper motor with the 
        /// appropriate pulse train.
        /// </summary>
        /// <param name="numNm">The number of nm to move -- positive or negative</param>
        protected override void InternalMoveMonochromator(Int32 numNm)
        {
            //Get the Port Number from the Config
            MccDaq.DigitalPortType dpt = (MccDaq.DigitalPortType)
                Enum.Parse(typeof(MccDaq.DigitalPortType), this.DaqInterfaceConfig.StepperOutPort);

            //Get the DIn value
            UInt16 currVal = 0;
            MccDaq.ErrorInfo ei = this._board.DIn(dpt, out currVal);

            //If the value could be retrieved, put out the portVals
            if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
            {
                //Setup the Port Vals based on the value of numNm
                UInt16[] vals = new UInt16[this._portVals.Length];
                Array.Copy(this._portVals, vals, this._portVals.Length);

                if (numNm < 0)
                    Array.Reverse(vals);

                //Setup a counter for the UInt16 array
                Int32 dinPtr = 0;

                //Initialize the counter by trying to find the value
                //in the portVals array
                if (currVal > 0)
                    dinPtr = Array.IndexOf(this._portVals, currVal);

                //Get the actual number of steps that should be applied
                Int32 totalSteps = this.GetNumSteps(numNm);

                //Iterate through the portVals and apply them to
                //the DOut port
                for (Int32 j = 0; j < totalSteps; j++)
                {
                    //Determine the value to put on the port
                    int modPtr = (j + dinPtr) % 8;

                    //Put out the value on the port
                    ei = this._board.DOut(dpt, vals[modPtr]);

                    //If there was an error, just break
                    if (ei.Value != MccDaq.ErrorInfo.ErrorCode.NoErrors)
                        break;

                    //Wait the mcDelay between M/C pulses
                    Thread.Sleep(this._mcDelay);
                }
            }
        }


        /// <summary>
        /// ShutdownMonochromator does nothing, as there is no
        /// shutdown procedure required for the MCC DAQ.
        /// </summary>
        protected override void InternalShutdownMonochromator()
        {
            //Do nothing
        }
        #endregion


        #region 3-D Methods
        /// <summary>
        /// MoveMirror moves either an X or Y-Axis mirror 
        /// in a 3-D Data Acquisition.
        /// </summary>
        /// <param name="axis">The Axis of the mirror to move</param>
        /// <param name="offset">The Offset to which to move the mirror: 0 - 5000</param>
        protected override void InternalMoveMirror(DaqBase.Axis axis, UInt32 offset)
        {
            //If the offset is out of bounds, throw an Exception
            if (offset < this._mirrorMin || offset > this._mirrorMax)
                throw new ArgumentException("Offset must be between 0 and 5000.");

            //Determine the Axis to move
            Int32 channel = Int32.Parse(this.DaqInterfaceConfig.XOutPort);
            if (axis == Axis.Y)
                channel = Int32.Parse(this.DaqInterfaceConfig.YOutPort);

            //Change the offset to a voltage from +/- 5V
            Double voltage = (Convert.ToDouble(offset) / 1000.0) - 5.0;

            //Change the voltage to a DAQ-understood value
            UInt16 shortVoltage = 0;
            MccDaq.ErrorInfo ei = this._board.FromEngUnits(MccDaq.Range.Bip5Volts, Convert.ToSingle(voltage), out shortVoltage);

            //Finally, set the value on the card
            ei = this._board.AOut(channel, MccDaq.Range.Bip5Volts, shortVoltage);
        }
        #endregion


        #region Counter Methods
        /// <summary>
        /// InternalReadLockInCounter reads the configured 
        /// Counter port for the value.
        /// </summary>
        /// <returns>The Counter value read from the configured port</returns>
        protected override Int32 InternalReadLockInCounter()
        {
            //Declare a variable to return
            Int32 rtn = 0;

            //Get the Counter number from Config
            Int32 ctr = Int32.Parse(this.DaqInterfaceConfig.InputCounterInPort);

            //Get the value from the card
            MccDaq.ErrorInfo ei = null;
            Int32 cardValue = 0;
            ei = this._board.CIn32(ctr, out cardValue);

            //If the calls have not returned an error, set the output
            if (ei.Value == MccDaq.ErrorInfo.ErrorCode.NoErrors)
                rtn = Convert.ToInt32(cardValue);

            //Return the result
            return rtn;
        }


        /// <summary>
        /// InternalClearLockInCounter clears the configured Counter
        /// port value.
        /// </summary>
        protected override void InternalClearLockInCounter()
        {
            //Get the Counter number from Config
            MccDaq.CounterRegister reg = (MccDaq.CounterRegister)Enum.Parse(
                typeof(MccDaq.CounterRegister), this.DaqInterfaceConfig.InputCounterInPort);

            //Set the counter on the card
            MccDaq.ErrorInfo ei = null;
            ei = this._board.CLoad32(reg, 0);
        }
        #endregion


        #region Public Properties
        public const String DefaultAIn = "0";
        public const String DefaultStepperOut = "FirstPortA";
        public const String DefaultSamplingRate = "37";
        public const String DefaultXMirrorOut = "0";
        public const String DefaultYMirrorOut = "1";
        public const String DefaultCounterIn = "1";


        /// <summary>
        /// BoardName returns the name of the MCC DAQ Board
        /// </summary>
        public override string BoardName
        {
            get { return this._board.BoardNum.ToString(); }
        }


        /// <summary>
        /// Type returns the MCC DaqType.
        /// </summary>
        public override Enums.DaqType Type
        {
            get { return Enums.DaqType.MCC; }
        }


        /// <summary>
        /// AnalogInPorts returns a set of Strings that 
        /// represent the Analog In port numbers.
        /// </summary>
        public override String[] AnalogInPorts
        {
            get 
            { 
                //Declare a variable to return
                List<String> rtn = new List<String>();

                //Get the number of A/D channels
                Int32 adChans = 0;
                this._board.BoardConfig.GetNumAdChans(out adChans);

                //Iterate through the channels and add them to 
                //the List
                for (Int32 i = 0; i < adChans; i++)
                    rtn.Add(i.ToString());

                //Return the result
                return rtn.ToArray();
            }
        }


        /// <summary>
        /// AnalogOutPorts returns a set of Strings that 
        /// represent the Analog In port numbers.
        /// </summary>
        public override String[] AnalogOutPorts
        {
            get
            {
                //Declare a variable to return
                List<String> rtn = new List<String>();

                //Get the number of D/A channels
                Int32 daChans = 0;
                this._board.BoardConfig.GetNumDaChans(out daChans);

                //Iterate through the channels and add them to 
                //the List
                for (Int32 i = 0; i < daChans; i++)
                    rtn.Add(i.ToString());

                //Return the result
                return rtn.ToArray();
            }
        }


        /// <summary>
        /// DigitalOutBytes returns the names of the 8-bit
        /// digital out ports.
        /// </summary>
        public override String[] DigitalOutBytes
        {
            get
            {
                //Declare a variable to return
                List<String> rtn = new List<String>();

                //Get the number of DIO channels
                Int32 dioChans = 0;
                this._board.BoardConfig.GetDiNumDevs(out dioChans);

                //Get the Array or enum strings
                String[] portNames = Enum.GetNames(typeof(MccDaq.DigitalPortType));

                //Iterate through the enums and add them to 
                //the List
                for (Int32 i = 0; i < dioChans; i++)
                    rtn.Add(portNames[i]);

                //Return the result
                return rtn.ToArray();
            }
        }


        /// <summary>
        /// DigitalOutPorts returns the names of the 1-bit
        /// digital out ports.
        /// </summary>
        public override String[] DigitalOutPorts
        {
            get
            {
                //Declare a variable to return
                List<String> rtn = new List<String>();

                //Get the number of DIO channels
                Int32 dioChans = 0;
                this._board.BoardConfig.GetNumIoPorts(out dioChans);

                //Iterate through the channels and add them to 
                //the List
                for (Int32 i = 1; i <= dioChans; i++)
                    rtn.Add(i.ToString());

                //Return the result
                return rtn.ToArray();
            }
        }


        /// <summary>
        /// CounterPorts returns the names of the 1-bit
        /// counter ports.
        /// </summary>
        public override String[] CounterPorts
        {
            get
            {
                //Declare a variable to return
                List<String> rtn = new List<String>();

                //Get the number of DIO channels
                Int32 ciChans = 0;
                this._board.BoardConfig.GetCiNumDevs(out ciChans);

                //Iterate through the channels and add them to 
                //the List
                for (Int32 i = 1; i <= ciChans; i++)
                    rtn.Add(i.ToString());

                //Return the result
                return rtn.ToArray();
            }
        }
        #endregion


        #region ToString overload
        /// <summary>
        /// ToString formats the board information into a 
        /// nice string of information.
        /// </summary>
        /// <returns>String of Board information</returns>
        public override string ToString()
        {
            //Get the Board Name from the card.  It's not
            //as easy as it probably should be...
            String name = this._board.BoardName;
            name = name.Split('\0')[0];

            return this.Type.ToString() + ": " + name + " (Board #" + this._board.BoardNum.ToString() + ")";
        }
        #endregion


        #region IDisposable Members
        /// <summary>
        /// Dispose cleans things up if the continuous 
        /// mode was used.
        /// </summary>
        public void Dispose()
        {
            //If the continuous mode was started, stop it
            if (this._bufHandle != Int32.MinValue)
            {
                //Stop the buffer filling
                this._board.StopBackground(MccDaq.FunctionType.AiFunction);

                //Free the Windows Buffer
                MccDaq.MccService.WinBufFree(this._bufHandle);
            }
        }
        #endregion
    }
}
