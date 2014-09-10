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
    /// FileDialog is the base class for the OpenFileDialog
    /// and SaveFileDialog classes.  It contains the definitions
    /// for common properties.
    /// </summary>
    public abstract class FileDialog : CommonDialog
    {
        #region Member Variables
        private IntPtr _hWnd = IntPtr.Zero;
        private Boolean _addExtenstion = true;
        private Int32 _extIndex = 0;
        private Boolean _checkPathExists = true;
        private String _filter = String.Empty;
        private String _fileName = String.Empty;
        private String _initialDir = String.Empty;
        private String _title = String.Empty;


        /// <summary>
        /// Event fired when the user changes the Filter Type
        /// on the Dialog Box.
        /// </summary>
        public event FilterTypeSetEventHandler FilterTypeChanged;


        /// <summary>
        /// Event fired when the user accepts the file in 
        /// the Dialog Box.
        /// </summary>
        public event FilterTypeSetEventHandler FilterTypeAccepted;
        #endregion


        #region CommonDialog Implementation
        /// <summary>
        /// CreateDialog does the actual work of creating
        /// and showing the Dialog box.  This is done in the
        /// child classes.
        /// </summary>
        /// <returns>Boolean from the Open or Save File Dialog functions</returns>
        protected abstract Boolean CreateDialog(IntPtr hwndOwner);


        /// <summary>
        /// Reset sets the dialog box back to its initial state.
        /// </summary>
        public override void Reset()
        {

        }


        /// <summary>
        /// RunDialog is the method called by the System to perform
        /// the actual creation, showing, and handling of the 
        /// Common File Dialog that is being implemented.
        /// </summary>
        /// <param name="hwndOwner">The Owner of this Dialog</param>
        /// <returns>Boolean True if the user clicks OK, False otherwise</returns>
        protected override Boolean RunDialog(IntPtr hwndOwner)
        {
            //Declare a variable to return
            Boolean rtn = true;

            try
            {
                //Call the CreateDialog abstract function
                rtn = this.CreateDialog(hwndOwner);

                //Fire the TypeAccepted event if the FileName
                //has been set
                if (this.FileName != String.Empty)
                {
                    if (this.FilterTypeAccepted != null)
                        this.FilterTypeAccepted(this, new FilterTypeSetEventArgs(String.Empty));
                }

            }
            catch (Exception ex)
            {
                //If there is an Exception, it would have to be pretty 
                //messed up, so rethrow it.
                throw new ApplicationException("The File Operation caused a Windows error:\n ", ex);
            }

            //If the CreateDialog function returned false, figure out why
            if (rtn == false)
            {
                //Call the CommDlgExtendedError function
                Int32 errCode = WndHelper.CommDlgExtendedError();

                //If the error is 0, then the user Canceled out of
                //the dialog, so do nothing.  Otherwise, throw an
                //Exception.
                if (errCode != 0)
                    throw new ApplicationException("Error in File Dialog: " + errCode.ToString());
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// HookProc does the actual work of the CommonDialog.  It is used
        /// in FileDialog to handle the events of the File Dialog.
        /// </summary>
        /// <param name="hWnd">The Handle to the Dialog</param>
        /// <param name="msg">The Windows Message</param>
        /// <param name="wparam">The WPARAM sent by the System</param>
        /// <param name="lparam">The LPARAM sent by the System</param>
        /// <returns></returns>
        protected override IntPtr HookProc(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)
        {
            //If this is the NOTIFY message, handle it.
            switch (msg)
            {
                case (Int32)WndMsg.WM_INITDIALOG:
                    //Save the hWnd in the member variable
                    this._hWnd = WndHelper.GetParent(hWnd);

                    //End of WM_INITDIALOG
                    break;

                case (Int32)WndMsg.WM_NOTIFY:
                    //Try to handle the WM_NOTIFY
                    try
                    {
                        //Get the OFNOTIFY struct from the lParam
                        OFNOTIFY notify = (OFNOTIFY)Marshal.PtrToStructure(lparam, typeof(OFNOTIFY));

                        //If the Code is TYPECHANGE, fire the event
                        if (notify.hdr.code == OFDMsg.CDN_INITDONE || notify.hdr.code == OFDMsg.CDN_TYPECHANGE)
                        {
                            //Get the OPENFILENAME struct
                            OPENFILENAME ofn = (OPENFILENAME)Marshal.PtrToStructure(notify.lpOFN, typeof(OPENFILENAME));

                            //Save the Extension Index
                            this._extIndex = ofn.nFilterIndex;

                            //Get the Extension
                            String ext = this.GetCurrentExtension();

                            //Fire the TypeChanged event
                            if (this.FilterTypeChanged != null)
                                this.FilterTypeChanged(this, new FilterTypeSetEventArgs(ext));
                        }
                    }
                    catch { }

                    //End of WM_NOTIFY
                    break;
            }

            //Pass the message on the base class
            return base.HookProc(hWnd, msg, wparam, lparam);
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// AddExtension determines if the FileOpenDialog will 
        /// add the current Extension is the user doesn't type 
        /// one in.
        /// </summary>
        public Boolean AddExtension
        {
            get { return this._addExtenstion; }
            set { this._addExtenstion = value; }
        }


        /// <summary>
        /// CheckPathExists ensures that the directory path that
        /// has been entered exists on disk before the user is 
        /// allowed to close the FileOpenDialog.
        /// </summary>
        public Boolean CheckPathExists
        {
            get { return this._checkPathExists; }
            set { this._checkPathExists = value; }
        }


        /// <summary>
        /// FileName returns the selected file or, if there are
        /// multiple files selected, the first in the series.
        /// </summary>
        public String FileName
        {
            get
            {
                //Declare a variable to return
                String rtn = String.Empty;

                //If there are any File Names, return the top
                if (this.FileNames.Length > 0)
                    rtn = this.FileNames[0];

                //Return the result
                return rtn;
            }

            set { this._fileName = value; }
        }


        /// <summary>
        /// FileNames returns all of the the selected file
        /// names. 
        /// </summary>
        public String[] FileNames
        {
            get { return this._fileName.Split('|'); }
            set { this._fileName = String.Join("|", value); }
        }


        /// <summary>
        /// Filter determines the Extensions that are visible and 
        /// allowed in the FileOpenDialog.
        /// </summary>
        public String Filter
        {
            get { return this._filter; }
            set { this._filter = value; }
        }


        /// <summary>
        /// InitialDirectory determines the directory first opened
        /// by the FileOpenDialog.
        /// </summary>
        public String InitialDirectory
        {
            get { return this._initialDir; }
            set { this._initialDir = value; }
        }


        /// <summary>
        /// NewDirectory allows the directory of the FileDialog
        /// to be changed programmatically.
        /// </summary>
        public String NewDirectory
        {
            set
            {
                Directory.SetCurrentDirectory(value);

                ////If the hWnd has been discovered, set the Directory
                //if (this._hWnd != IntPtr.Zero)
                //{
                //    //Get a handle to the ComboBox
                //    IntPtr hwndFileName = WndHelper.GetDlgItem(this._hWnd, ControlID.cbFileName);

                //    //If the ComboBox is found, do the directory change
                //    if (hwndFileName != IntPtr.Zero)
                //    {
                //        //Get the existing text in the ComboBox
                //        StringBuilder sbExisting = new StringBuilder(1024);
                //        WndHelper.GetWindowText(hwndFileName, sbExisting, 1024);

                //        //Set the Directory text in the ComboBox
                //        WndHelper.SetWindowText(hwndFileName, value);

                //        //Send a WM_COMMAND that the Enter key was pushed
                //        WndHelper.SendMessage(this._hWnd, WndMsg.WM_COMMAND, 1, 0);

                //        //Finally, set the old text back in the ComboBox
                //        WndHelper.SetWindowText(hwndFileName, sbExisting.ToString());
                //    }
                //}
            }
        }


        /// <summary>
        /// Title determines the title of the FileOpenDialog.
        /// </summary>
        public String Title
        {
            get { return this._title; }
            set { this._title = value; }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// CreateOPENFILENAME encapsulates the creation of the 
        /// OPENFILENAME struct used by the Open and Save FileDialogs.
        /// </summary>
        /// <returns>Filled OPENFILENAME struct</returns>
        internal OPENFILENAME CreateOPENFILENAME(IntPtr hwndOwner)
        {
            //Declare an OPENFILENAME to return
            OPENFILENAME ofn = new OPENFILENAME();

            //Set the Strings if it has been set
            String tempFilter = this.GenerateFilter();
            if (tempFilter == String.Empty)
                tempFilter = "\0";

            String tempInitDir = "\0";
            if (this.InitialDirectory != String.Empty)
                tempInitDir = this.InitialDirectory;

            String tempTitle = "\0";
            if (this.Title != String.Empty)
                tempTitle = this.Title;

            //Fill the OPENFILENAME struct
            ofn.lStructSize = System.Runtime.InteropServices.Marshal.SizeOf(ofn);
            ofn.hwndOwner = hwndOwner;
            ofn.lpstrFilter = Marshal.StringToHGlobalAuto(tempFilter);
            ofn.lpstrFile = Marshal.StringToHGlobalAuto(String.Format("\0{0}", new String(' ', 1023)));
            ofn.nMaxFile = 1024;
            ofn.Flags = this.BuildFlags();
            ofn.lpstrInitialDir = Marshal.StringToHGlobalAuto(tempInitDir);
            ofn.lpfnHook = new HookProcDelegate(this.HookProc);
            ofn.lpstrTitle = Marshal.StringToHGlobalAuto(tempTitle);
            ofn.lpTemplateName = IntPtr.Zero;
            ofn.pvReserved = IntPtr.Zero;
            ofn.dwReserved = 0;

            //Return the result
            return ofn;
        }


        /// <summary>
        /// CleanupOPENFILENAME takes a filled OPENFILENAME
        /// struct and releases the memory used by the Strings.
        /// </summary>
        /// <param name="ofn">OPENFILENAME struct to clean up</param>
        internal void CleanupOPENFILENAME(OPENFILENAME ofn)
        {
            //Clean up the memory
            Marshal.FreeHGlobal(ofn.lpstrFilter);
            Marshal.FreeHGlobal(ofn.lpstrFile);
            Marshal.FreeHGlobal(ofn.lpstrInitialDir);
            Marshal.FreeHGlobal(ofn.lpstrTitle);
        }


        /// <summary>
        /// BuildFlags returns the flags to apply to a File Open
        /// Dialog based on the values of the Member Variables.
        /// </summary>
        /// <returns>Total OpenFileFlags enum</returns>
        internal virtual OpenFileFlags BuildFlags()
        {
            //Declare the base properties to return
            OpenFileFlags rtn =
                OpenFileFlags.OFN_ENABLEHOOK |
                OpenFileFlags.OFN_ENABLESIZING |
                OpenFileFlags.OFN_EXPLORER |
                OpenFileFlags.OFN_HIDEREADONLY;

            if (this._checkPathExists == true)
                rtn |= OpenFileFlags.OFN_PATHMUSTEXIST;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// GenerateFilter takes the .NET style filter string and
        /// turns it into something that the Win32 API can use.
        /// </summary>
        /// <returns>NULL-replaced string for the Win32 API</returns>
        protected String GenerateFilter()
        {
            return this._filter.Replace('|', '\0') + "\0\0";
        }


        /// <summary>
        /// GetCurrentExtension parses through the Filter String
        /// to figure out what the actual Extension is.
        /// </summary>
        /// <returns>The Extension of the current filter</returns>
        private String GetCurrentExtension()
        {
            //Split the Filter on pipes
            String[] filters = this._filter.Split('|');

            //Get the Filter extension
            String filtExt = filters[((this._extIndex - 1) * 2) + 1];

            //Finally, get rid of the Asterisk to get just
            //the extension value
            String ext = filtExt.Split('.')[1];

            //Return the result
            return "." + ext;
        }


        /// <summary>
        /// ProcessFileIntPtr manipulates the returned memory
        /// until it represents a useful String of file names.
        /// </summary>
        /// <param name="ip">The IntPtr to manipulate</param>
        /// <returns>The list of File Names found in the memory</returns>
        protected String ProcessFileIntPtr(IntPtr ip)
        {
            //Declare a variable to return
            String rtn = String.Empty;

            //Turn the IntPtr into a String by first Copying
            //the memory to a Char Array
            Char[] raw = new Char[1024];
            Marshal.Copy(ip, raw, 0, 1024);
            String strRaw = new String(raw);

            //Find the last NULL character in the string.  This will
            //be used to get rid of the excess characters.
            Int32 lastNullIndex = strRaw.LastIndexOf('\0');

            //If the last NULL is a double, like it would be if 
            //multiple files were selected, get rid of that 
            //character as well
            if (raw[lastNullIndex - 1] == '\0')
                lastNullIndex--;

            //Finally, cleave the excess String
            String clean = strRaw.Substring(0, lastNullIndex);

            //Split the clean String on the NULL character
            String[] fileParts = clean.Split(new Char[] { '\0' }, StringSplitOptions.RemoveEmptyEntries);

            //If there is more than one member in the Array, 
            //the first member is the directory.  Save that 
            //in a local variable.
            String directory = String.Empty;
            Int32 fileIndex = 0;
            if (fileParts.Length > 1)
            {
                directory = fileParts[0];
                fileIndex = 1;
            }

            //Get the current Extension in case it needs to be added
            String ext = this.GetCurrentExtension();

            //Iterate through all of the File Names and add them
            //the return
            for (Int32 i = fileIndex; i < fileParts.Length; i++)
            {
                //Add the separator if necessary
                if (rtn != String.Empty)
                    rtn += "|";

                //Declare a temp string to hold the result
                String temp = fileParts[i];

                //Add the Extension if necessary
                if (temp.ToUpper().EndsWith(ext) == false)
                    temp += ext;

                //Add the path if necessary
                if (directory != String.Empty)
                    temp = Path.Combine(directory, temp);

                //Finally, add it to the return String
                rtn += temp;
            }

            //Return the result
            return rtn;
        }
        #endregion

        
        #region Enumerated OpenFileDialog WM_NOTIFY codes
        /// <summary>
        /// OFDMsg is an enum for the messages that get passed
        /// to the OpenFileDialog to indicate that something 
        /// has happened.
        /// </summary>
        private class OFDMsg
        {
            public static UInt32 CDN_INITDONE = 0xFFFFFDA7;
            public static UInt32 CDN_FILEOK = 0xFFFFFDA5;
            public static UInt32 CDN_TYPECHANGE = 0xFFFFFDA1;
        }
        #endregion
    }
}
