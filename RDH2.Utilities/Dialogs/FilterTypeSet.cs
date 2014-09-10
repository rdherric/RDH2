using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Utilities.Dialogs
{
    /// <summary>
    /// FilterTypeSet Event Handler definition.  Fired when the
    /// user changes the Filter Type on the dialog box.
    /// </summary>
    /// <param name="sender">The Dialog Box that had the Type Changed</param>
    /// <param name="e">The EventArgs sent by the FileDialog</param>
    public delegate void FilterTypeSetEventHandler(Object sender, FilterTypeSetEventArgs e);


    /// <summary>
    /// FilterTypeSetEventArgs are sent by the Dialog Box 
    /// when the Filter has been set -- either when the 
    /// user changes the Filter or when the File has been
    /// Selected.
    /// </summary>
    public class FilterTypeSetEventArgs
    {
        #region Member Variables
        private String _extension = String.Empty;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the TypeChangedEventArgs class.
        /// </summary>
        /// <param name="extension"></param>
        public FilterTypeSetEventArgs(String extension)
        {
            //Save the member variables
            this._extension = extension;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// Extension determines the Filter that has been 
        /// selected by the User.
        /// </summary>
        public String Extension
        {
            get { return this._extension; }
        }
        #endregion
    }
}
