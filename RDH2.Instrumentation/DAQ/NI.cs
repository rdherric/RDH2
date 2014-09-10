using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace RDH2.Instrumentation.DAQ
{
    /// <summary>
    /// NI represents a National Instruments Data Acquistion
    /// board -- derived from DaqBase.
    /// </summary>
    public class NI : DaqBase
    {
        #region Member variables
        private NationalInstruments.DAQmx.Device _board = null;

        //Tasks to automate the hardware and acquire data
        NationalInstruments.DAQmx.Task _readVoltage = null;
        NationalInstruments.DAQmx.Task _setMCScanUp = null;
        NationalInstruments.DAQmx.Task _setMCScanDown = null;
        NationalInstruments.DAQmx.Task _moveMonochromator = null;
        NationalInstruments.DAQmx.Task _moveXMirror = null;
        NationalInstruments.DAQmx.Task _moveYMirror = null;
        #endregion


        #region Internal Constructor
        /// <summary>
        /// The internal constructor makes sure that the NI
        /// class can not be created outside the Hardware DLL.
        /// </summary>
        /// <param name="board">The NI DAQ Board to wrap</param>
        internal NI(NationalInstruments.DAQmx.Device board)
        {
            //Save the member variables
            this._board = board;
        }
        #endregion


        #region Voltage Read Methods
        /// <summary>
        /// ReadVoltage reads the Voltage from the DAQ card.
        /// </summary>
        /// <returns>Double Voltage averaged over the number of samples</returns>
        protected override Double InternalReadVoltage(Int32 samplesToAvg)
        {
            //Declare a variable to return
            Double rtn = 0.0;

            //If the Task doesn't exist, create it
            if (this._readVoltage == null)
            {
                //Create the ReadVoltage Task and configure it
                this._readVoltage = new NationalInstruments.DAQmx.Task();
                this._readVoltage.AIChannels.CreateVoltageChannel(this.VoltageChannel, String.Empty,
                    NationalInstruments.DAQmx.AITerminalConfiguration.Differential, -10, 10, NationalInstruments.DAQmx.AIVoltageUnits.Volts);
            }

            //Create an Analog In reader from the ReadVoltage task
            NationalInstruments.DAQmx.AnalogSingleChannelReader reader =
                new NationalInstruments.DAQmx.AnalogSingleChannelReader(this._readVoltage.Stream);

            //Declare a temp Voltage variable
            Double totalVoltage = 0.0;

            //Iterate through the number of samples to average
            for (Int32 i = 0; i < samplesToAvg; i++)
            {
                //Read the voltage
                totalVoltage += reader.ReadSingleSample();

                //Sleep the expected amount of time
                Thread.Sleep(this.AcquisitionDelay);
            }

            //Now set the return value
            rtn = totalVoltage / samplesToAvg;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// InternalReadVoltageArray reads an array of voltages from the DAQ card.
        /// </summary>
        /// <param name="pointsToRead">The number of points to read</param>
        /// <param name="samplingRate">The rate at which the data is sampled</param>
        /// <returns>The array of data returned by the card</returns>
        protected override double[] InternalReadVoltageArray(UInt32 pointsToRead, UInt32 samplingRate)
        {
            throw new NotImplementedException();
        }
        #endregion


        #region 2-D Spectra Methods
        /// <summary>
        /// SetupMonochromator sets the appropriate bits on the
        /// stepper controller to determine the direction of the
        /// scan.
        /// </summary>
        /// <param name="direction">Int32 direction parameter -- positive for Low -> High, negative for High -> Low</param>
        protected override void InternalSetupMonochromator(Int32 direction)
        {
            //Create the Monochromator Up Task if it hasn't 
            //been created yet
            if (this._setMCScanUp == null)
            {
                this._setMCScanUp = new NationalInstruments.DAQmx.Task();
                this._setMCScanUp.DOChannels.CreateChannel(this.MCScanUpChannel, String.Empty, NationalInstruments.DAQmx.ChannelLineGrouping.OneChannelForEachLine);
            }

            //Create the Monochromator Down Task if it hasn't 
            //been created yet
            if (this._setMCScanDown == null)
            {
                this._setMCScanDown = new NationalInstruments.DAQmx.Task();
                this._setMCScanDown.DOChannels.CreateChannel(this.MCScanDownChannel, String.Empty, NationalInstruments.DAQmx.ChannelLineGrouping.OneChannelForEachLine);
            }

            //Create the Writers
            NationalInstruments.DAQmx.DigitalSingleChannelWriter up = new 
                NationalInstruments.DAQmx.DigitalSingleChannelWriter(this._setMCScanUp.Stream);

            NationalInstruments.DAQmx.DigitalSingleChannelWriter down = new
                NationalInstruments.DAQmx.DigitalSingleChannelWriter(this._setMCScanDown.Stream);

            //Declare values to write to the DO
            Boolean scanUp = true;
            Boolean scanDown = false;

            if (direction < 0)
            {
                scanUp = false;
                scanDown = true;
            }

            //Write the values to the Ports
            up.WriteSingleSampleSingleLine(true, scanUp);
            down.WriteSingleSampleSingleLine(true, scanDown);
        }

        
        /// <summary>
        /// MoveMonochromator moves the monochromator the specified
        /// number of nm by pulsing the stepper controller with the 
        /// appropriate pulse train.
        /// </summary>
        /// <param name="numNm">The number of nm to move -- positive or negative</param>
        protected override void InternalMoveMonochromator(Int32 numNm)
        {
            //Create the MoveMonochromator Task if it hasn't
            //been created yet
            if (this._moveMonochromator == null)
            {
                this._moveMonochromator = new NationalInstruments.DAQmx.Task();
                this._moveMonochromator.COChannels.CreatePulseChannelFrequency(this.MonochromatorCounterChannel, String.Empty,
                    NationalInstruments.DAQmx.COPulseFrequencyUnits.Hertz, NationalInstruments.DAQmx.COPulseIdleState.Low, 0.0, 120.0, 0.12);
            }

            //Setup the Timing with the number of Steps
            this._moveMonochromator.Timing.ConfigureImplicit(NationalInstruments.DAQmx.SampleQuantityMode.FiniteSamples, this.GetNumSteps(numNm));

            //Start the task and wait for it to complete
            this._moveMonochromator.Start();
            this._moveMonochromator.WaitUntilDone();
            this._moveMonochromator.Stop();
        }


        /// <summary>
        /// ShutdownMonochromator resets the bits on the stepper 
        /// controller so that the relay is not used up.
        /// </summary>
        protected override void InternalShutdownMonochromator()
        {
            //Create the Monochromator Up Task if it hasn't 
            //been created yet
            if (this._setMCScanUp == null)
            {
                this._setMCScanUp = new NationalInstruments.DAQmx.Task();
                this._setMCScanUp.DOChannels.CreateChannel(this.MCScanUpChannel, String.Empty, NationalInstruments.DAQmx.ChannelLineGrouping.OneChannelForEachLine);
            }

            //Create the Monochromator Down Task if it hasn't 
            //been created yet
            if (this._setMCScanDown == null)
            {
                this._setMCScanDown = new NationalInstruments.DAQmx.Task();
                this._setMCScanDown.DOChannels.CreateChannel(this.MCScanDownChannel, String.Empty, NationalInstruments.DAQmx.ChannelLineGrouping.OneChannelForEachLine);
            }

            //Create the Writers
            NationalInstruments.DAQmx.DigitalSingleChannelWriter up = new
                NationalInstruments.DAQmx.DigitalSingleChannelWriter(this._setMCScanUp.Stream);

            NationalInstruments.DAQmx.DigitalSingleChannelWriter down = new
                NationalInstruments.DAQmx.DigitalSingleChannelWriter(this._setMCScanDown.Stream);

            //Set 0s to the Ports
            up.WriteSingleSampleSingleLine(true, false);
            down.WriteSingleSampleSingleLine(true, false);
        }
        #endregion


        #region 3-D Methods
        /// <summary>
        /// MoveMIrror moves an X or Y-Axis mirror to the specified
        /// offset in a 3-D Data Acquisition scan.
        /// </summary>
        /// <param name="axis">The Axis of the Mirror to move</param>
        /// <param name="offset">The offset to which to move the mirror: 0 - 5000</param>
        protected override void InternalMoveMirror(DaqBase.Axis axis, UInt32 offset)
        {
            //If the offset is out of bounds, throw an Exception
            if (offset < this._mirrorMin || offset > this._mirrorMax)
                throw new ArgumentException("Offset must be between 0 and 5000.");

            //Create the X-Mirror Task if it hasn't been
            //created yet
            if (this._moveXMirror == null)
            {
                this._moveXMirror = new NationalInstruments.DAQmx.Task();
                this._moveXMirror.AOChannels.CreateVoltageChannel(this.XMirrorChannel, String.Empty, -5.0, 5.0, NationalInstruments.DAQmx.AOVoltageUnits.Volts);
            }

            //Create the Y-Mirror Task if it hasn't been
            //created yet
            if (this._moveYMirror == null)
            {
                this._moveYMirror = new NationalInstruments.DAQmx.Task();
                this._moveYMirror.AOChannels.CreateVoltageChannel(this.YMirrorChannel, String.Empty, -5.0, 5.0, NationalInstruments.DAQmx.AOVoltageUnits.Volts);
            }

            //Calculate the voltage from the offset -- from +/- 5V
            Double voltage = (Convert.ToDouble(offset) / 1000.0) - 5.0;

            //Determine the Task to call based on the Axis
            NationalInstruments.DAQmx.Task task = this._moveXMirror;
            if (axis == Axis.Y)
                task = this._moveYMirror;

            //Create a Writer for the Task
            NationalInstruments.DAQmx.AnalogSingleChannelWriter writer = new
                NationalInstruments.DAQmx.AnalogSingleChannelWriter(task.Stream);

            //Write our the voltage
            writer.WriteSingleSample(true, voltage);
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
            return 0;
        }


        /// <summary>
        /// InternalClearLockInCounter clears the configured Counter
        /// port value.
        /// </summary>
        protected override void InternalClearLockInCounter()
        {
        }
        #endregion

        
        #region Public Properties
        public const String DefaultAIn = "ai1";
        public const String DefaultCounterOut = "ctr0";
        public const String DefaultScanDownOut = "port0/line1";
        public const String DefaultScanUpOut = "port0/line0";
        public const String DefaultSamplingRate = "37";
        public const String DefaultXMirrorOut = "ao1";
        public const String DefaultYMirrorOut = "ao0";
        public const String DefaultCounterIn = "ctr1";


        /// <summary>
        /// BoardName returns the name of the NI DAQ board
        /// </summary>
        public override string BoardName
        {
            get { return this._board.DeviceID; }
        }


        /// <summary>
        /// Type returns the NI DaqType.
        /// </summary>
        public override RDH2.Instrumentation.Enums.DaqType Type
        {
            get { return Enums.DaqType.NI; }
        }


        /// <summary>
        /// AnalogInPorts returns a set of Strings that 
        /// represent the Analog In port numbers.
        /// </summary>
        public override String[] AnalogInPorts
        {
            get 
            {
                //Get the list of Analog In ports
                String[] ports = this._board.AIPhysicalChannels; 

                //Truncate the channel names and return
                this.TruncateNames(ref ports);
                return ports;
            }
        }


        /// <summary>
        /// AnalogOutPorts returns a set of Strings that 
        /// represent the Analog Out port numbers.
        /// </summary>
        public override String[] AnalogOutPorts
        {
            get
            {
                //Get the list of Analog Out ports
                String[] ports = this._board.AOPhysicalChannels;

                //Truncate the channel names and return
                this.TruncateNames(ref ports);
                return ports;
            }
        }


        /// <summary>
        /// DigitalOutBytes returns the names of the 8-bit 
        /// Digital Out ports.
        /// </summary>
        public override String[] DigitalOutBytes
        {
            get 
            {
                //Get the list of DIO Ports
                String[] ports = this._board.DOPorts;

                //Truncate the channel names and return
                this.TruncateNames(ref ports);
                return ports;
            }
        }


        /// <summary>
        /// CounterPorts returns the names of the 1-bit 
        /// Counter ports.
        /// </summary>
        public override String[] CounterPorts
        {
            get
            {
                //Get the list of DIO Ports
                String[] ports = this._board.COPhysicalChannels;

                //Truncate the channel names and return
                this.TruncateNames(ref ports);
                return ports;
            }
        }

        
        /// <summary>
        /// DigitalOutPorts returns the names of the 1-bit
        /// Digital Out ports.
        /// </summary>
        public override String[] DigitalOutPorts
        {
            get
            {
                //Get the list of DIO Ports
                String[] ports = this._board.DOLines;

                //Truncate the channel names and return
                this.TruncateNames(ref ports);
                return ports;
            }
        }
        #endregion


        #region ToString override
        /// <summary>
        /// ToString formats the board information into a 
        /// nice string of information.
        /// </summary>
        /// <returns>String of Board information</returns>
        public override string ToString()
        {
            return this.Type.ToString() + ": " + this._board.DeviceID + " (" + this._board.ProductType + ")";
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// TruncateNames takes an Array of Strings and
        /// iterates through them to chop off the Device
        /// Name from them.
        /// </summary>
        /// <param name="ports">The Array of Strings to truncate</param>
        private void TruncateNames(ref String[] ports)
        {
            //Iterate through the Strings and truncate them
            for (Int32 i = 0; i < ports.Length; i++)
            {
                //Split the string on '/'
                String[] devPort = ports[i].Split(new Char[] {'/'}, 2);

                //If there are two returns, set the value
                if (devPort.Length > 1)
                    ports[i] = devPort[1];
            }
        }
        #endregion


        #region Private Properties
        /// <summary>
        /// VoltageChannel returns the complete path to the 
        /// Analog In Channel that was configured.
        /// </summary>
        private String VoltageChannel
        {
            get { return this.DaqInterfaceConfig.BoardName + "/" + this.DaqInterfaceConfig.VoltageInPort; }
        }

        
        /// <summary>
        /// MCScanUpChannel returns the complete path to the
        /// Digital Out Channel that was configured.
        /// </summary>
        private String MCScanUpChannel
        {
            get { return this.DaqInterfaceConfig.BoardName + "/" + this.DaqInterfaceConfig.ScanUpPort; }
        }


        /// <summary>
        /// MCScanDownChannel returns the complete path to the
        /// Digital Out Channel that was configured.
        /// </summary>
        private String MCScanDownChannel
        {
            get { return this.DaqInterfaceConfig.BoardName + "/" + this.DaqInterfaceConfig.ScanDownPort; }
        }


        /// <summary>
        /// MonochromatorCounterChannel returns the complete path 
        /// to the Counter Out Channel that was configured.
        /// </summary>
        private String MonochromatorCounterChannel
        {
            get { return this.DaqInterfaceConfig.BoardName + "/" + this.DaqInterfaceConfig.ScanCounterPort; }
        }


        /// <summary>
        /// XMirrorChannel returns the complete path to the 
        /// Digital Out Channel that was configured.
        /// </summary>
        private String XMirrorChannel
        {
            get { return this.DaqInterfaceConfig.BoardName + "/" + this.DaqInterfaceConfig.XOutPort; }
        }


        /// <summary>
        /// YMirrorChannel returns the complete path to the 
        /// Digital Out Channel that was configured.
        /// </summary>
        private String YMirrorChannel
        {
            get { return this.DaqInterfaceConfig.BoardName + "/" + this.DaqInterfaceConfig.YOutPort; }
        }
        #endregion
    }
}
