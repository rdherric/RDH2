using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

using RDH2.Utilities.Win32;

namespace RDH2.Utilities.Dialogs
{
    /// <summary>
    /// SaveFileDialog is a more functional version of the WinForms
    /// SaveFileDialog, which is a sealed class that cannot be
    /// extended.
    /// </summary>
    public class SaveFileDialog : FileDialog
    {
        #region Member Variables
        private Boolean _overwritePrompt = true;
        #endregion


        #region CreateDialog Implementation
        /// <summary>
        /// CreateDialog creates and shows the OpenFileDialog.
        /// </summary>
        /// <param name="hwndOwner">The Owner of this Dialog</param>
        /// <returns>Boolean True if the user clicks OK, False otherwise</returns>
        protected override Boolean CreateDialog(IntPtr hwndOwner)
        {
            //Declare a variable to return
            Boolean rtn = true;

            //Declare a new OPENFILENAME struct
            OPENFILENAME ofn = new OPENFILENAME();

            try
            {
                //Get an OPENFILENAME struct
                ofn = this.CreateOPENFILENAME(hwndOwner);

                //Call the OpenFileName function
                rtn = WndHelper.GetSaveFileName(ref ofn);

                //Set the values in the Member Variables if the user 
                //clicked OK
                if (rtn == true)
                    this.FileName = this.ProcessFileIntPtr(ofn.lpstrFile);
                else
                    this.FileName = String.Empty;
            }
            catch { }
            finally
            {
                //Clean up the memory
                this.CleanupOPENFILENAME(ofn);
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// OverwritePrompt determines whether the SaveFileDialog
        /// will prompt prior to overwriting the file.
        /// </summary>
        public Boolean OverwritePrompt
        {
            get { return this._overwritePrompt; }
            set { this._overwritePrompt = value; }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// BuildFlags adds in the Overwrite Prompt flag
        /// if necessary.
        /// </summary>
        /// <returns>OpenFileFlags appropriate to the state</returns>
        internal override OpenFileFlags BuildFlags()
        {
            //Declare a variable to return
            OpenFileFlags rtn = base.BuildFlags();

            //Add the Overwrite Prompt flag if necessary
            if (this._overwritePrompt == true)
                rtn |= OpenFileFlags.OFN_OVERWRITEPROMPT;

            //Add in the Path must exist flag
            rtn |= OpenFileFlags.OFN_PATHMUSTEXIST;

            //Return the result
            return rtn;
        }
        #endregion
    }
}
