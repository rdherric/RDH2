using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.DAQ
{
    /// <summary>
    /// DaqFactory does the work of figuring out what DAQ
    /// cards are installed on the computer and returning
    /// classes to interact with them.
    /// </summary>
    public class DaqFactory
    {
        #region Public Properties
        /// <summary>
        /// InstalledCards discovers all of the installed 
        /// Data Acquisition cards and returns them in a 
        /// List of DaqBase objects.
        /// </summary>
        public static List<DaqBase> InstalledCards
        {
            get
            {
                //Declare a variable to return
                List<DaqBase> rtn = new List<DaqBase>();

                //Get the MCC cards installed on the computer
                try
                {
                    rtn.AddRange(DaqFactory.GetMCCCards());
                }
                catch { }

                //Get the NI cards installed on the computer
                try
                {
                    rtn.AddRange(DaqFactory.GetNICards());
                }
                catch { }

                //Return the result
                return rtn;
            }
        }


        /// <summary>
        /// ConfiguredCard checks the DAQ configuration in the 
        /// app.config file and returns the card that has been
        /// configured for use with PhotoCurrent.
        /// </summary>
        public static DaqBase ConfiguredCard
        {
            get
            {
                //Declare a variable to return
                DaqBase rtn = null;

                //Get the configuration from the app.config
                ConfigHelper<Config.DaqInterface> config = new ConfigHelper<Config.DaqInterface>(ConfigLocation.AllUsers);
                Config.DaqInterface di = config.GetConfig();

                //If the card has been configured, create the appropriate
                //derived class object
                try
                {
                    if (di.Type == Enums.DaqType.MCC)
                        rtn = DaqFactory.CreateMCCCard(di);
                    else if (di.Type == Enums.DaqType.NI)
                        rtn = DaqFactory.CreateNICard(di);
                }
                catch (Exception e)
                {
                    throw e;
                }

                //Return the result
                return rtn;
            }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// GetMCCCards retrieves all of the configured MCC cards
        /// on the computer.
        /// </summary>
        /// <returns>Array of MCC objects</returns>
        private static MCC[] GetMCCCards()
        {
            //Declare a variable to return
            List<MCC> rtn = new List<MCC>();

            //Get the max number of boards
            Int32 maxBoards = MccDaq.GlobalConfig.NumBoards;

            //Iterate through the Max Number of boards and 
            //create new MCC objects from them
            for (Int32 i = 0; i < maxBoards; i++)
            {
                //Attempt to make the MccBoard
                MccDaq.MccBoard board = new MccDaq.MccBoard(i);

                //If the Board doesn't appear to exist, continue
                MccDaq.ErrorInfo ei = board.FlashLED();
                if (ei.Value == MccDaq.ErrorInfo.ErrorCode.BadBoard)
                    continue;

                //Add the board to the List
                rtn.Add(new MCC(board));
            }

            //Return the result
            return rtn.ToArray();
        }


        /// <summary>
        /// GetNICards retrieves all of the configured NI cards
        /// on the computer.
        /// </summary>
        /// <returns>Array of NI objects</returns>
        private static NI[] GetNICards()
        {
            //Declare a variable to return
            List<NI> rtn = new List<NI>();

            //Iterate through the Devices and add them to the List
            foreach (String devName in NationalInstruments.DAQmx.DaqSystem.Local.Devices)
                rtn.Add(new NI(NationalInstruments.DAQmx.DaqSystem.Local.LoadDevice(devName)));

            //Return the result
            return rtn.ToArray();
        }


        /// <summary>
        /// CreateMCCCard creates an MCC card.  This is encapsulated
        /// so that the JIT compiler doesn't complain about the NI
        /// drivers.
        /// </summary>
        /// <param name="di">The DaqInterface Configureation</param>
        /// <returns>The Configured Card</returns>
        private static DaqBase CreateMCCCard(Config.DaqInterface di)
        {
            return new MCC(new MccDaq.MccBoard(Convert.ToInt32(di.BoardName)));
        }


        /// <summary>
        /// CreateNICard creates an NI card.  This is encapsulated
        /// so that the JIT compiler doesn't complain about the MCC
        /// drivers.
        /// </summary>
        /// <param name="di">The DaqInterface Configureation</param>
        /// <returns>The Configured Card</returns>
        private static DaqBase CreateNICard(Config.DaqInterface di)
        {
            return new NI(NationalInstruments.DAQmx.DaqSystem.Local.LoadDevice(di.BoardName));
        }
        #endregion
    }
}
