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
    /// OpenFileDialog is a more functional version of the WinForms
    /// OpenFileDialog, which is a sealed class that cannot be
    /// extended.
    /// </summary>
    public class OpenFileDialog : FileDialog
    {
        #region Member Variables
        private Boolean _checkFileExists = true;
        private Boolean _multiSelect = false;
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
                //Create the OPENFILENAME
                ofn = this.CreateOPENFILENAME(hwndOwner);

                //Call the OpenFileName function
                rtn = WndHelper.GetOpenFileName(ref ofn);

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
                this.CleanupOPENFILENAME(ofn);
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// CheckFileExists ensures that the file that has been
        /// entered exists on disk before the user is allowed to
        /// close the FileOpenDialog.
        /// </summary>
        public Boolean CheckFileExists
        {
            get { return this._checkFileExists; }
            set { this._checkFileExists = value; }
        }


        /// <summary>
        /// Multiselect determines whether the user is allowed
        /// to select multiple files for opening.
        /// </summary>
        public Boolean Multiselect
        {
            get { return this._multiSelect; }
            set { this._multiSelect = value; }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// BuildFlags returns the flags to apply to a File Open
        /// Dialog based on the values of the Member Variables.
        /// </summary>
        /// <returns>Total OpenFileFlags enum</returns>
        internal override OpenFileFlags BuildFlags()
        {
            //Call the base method
            OpenFileFlags rtn = base.BuildFlags();

            //Add the other properties based on the member variables
            if (this._checkFileExists == true)
                rtn |= OpenFileFlags.OFN_FILEMUSTEXIST;

            if (this._multiSelect == true)
                rtn |= OpenFileFlags.OFN_ALLOWMULTISELECT;

            //Return the result
            return rtn;
        }
        #endregion
    }
}
