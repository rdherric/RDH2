using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace RDH2.Windows.ViewModel
{
    /// <summary>
    /// AboutBoxView exposes properties about an 
    /// Application to be consumed by an About Box
    /// View.
    /// </summary>
    public class AboutBox
    {
        #region Member Variables
        private String _title = String.Empty;
        private String _version = String.Empty;
        private String _config = String.Empty;
        private String _copyright = String.Empty;
        private String _companyName = String.Empty;
        private String _description = String.Empty;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the AboutBox object.
        /// </summary>
        public AboutBox()
        {
            //Fill all of the member variables using Reflection
            this.FillMemberVariables();
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// CompanyName returns the name of the Company
        /// that built the Application that is calling 
        /// into this object.
        /// </summary>
        public String CompanyName
        {
            get { return this._companyName; }
        }


        /// <summary>
        /// Copyright returns the copyright information 
        /// of the Application that is calling into this 
        /// object.
        /// </summary>
        public String Copyright
        {
            get { return this._copyright; }
        }


        /// <summary>
        /// Description returns the description of the 
        /// Application that is calling into this object.
        /// </summary>
        public String Description
        {
            get { return this._description; }
        }


        /// <summary>
        /// Title returns the display name of 
        /// the Application that is calling into this 
        /// object.
        /// </summary>
        public String Title
        {
            get { return this._title; }
        }


        /// <summary>
        /// Version returns the complete version string of 
        /// the Application that is calling into this object.
        /// </summary>
        public String Version
        {
            get { return this._version; }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// FillMemberVariables reflects over the calling
        /// Assembly and gets all of the Custom Attributes
        /// to fill the member variables.
        /// </summary>
        private void FillMemberVariables()
        {
            //Get the referencing Assembly
            Assembly assembly = Assembly.GetEntryAssembly();

            //Get all of the Attributes of the Assembly
            Object[] attributes = assembly.GetCustomAttributes(false);

            //Iterate through all of the Attributes 
            //and fill the member variables
            foreach (Object attr in attributes)
            {
                //If structure to determine type of Attribute
                if (attr is AssemblyCompanyAttribute)
                    this._companyName = ((AssemblyCompanyAttribute)attr).Company;
                else if (attr is AssemblyConfigurationAttribute)
                    this._config = ((AssemblyConfigurationAttribute)attr).Configuration;
                else if (attr is AssemblyCopyrightAttribute)
                    this._copyright = ((AssemblyCopyrightAttribute)attr).Copyright;
                else if (attr is AssemblyDescriptionAttribute)
                    this._description = ((AssemblyDescriptionAttribute)attr).Description;
                else if (attr is AssemblyTitleAttribute)
                    this._title = ((AssemblyTitleAttribute)attr).Title;
            }

            //Fill in the Version information
            this._version = assembly.GetName().Version.ToString() + " " + this._config;
        }
        #endregion
    }
}
