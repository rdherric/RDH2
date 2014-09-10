using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Configuration
{
    /// <summary>
    /// ConfigurationGroupAttribute is a custom Attribute
    /// used to label a ConfigurationSection-derived class
    /// so that the ConfigHelper can determine how to 
    /// retrieve the section at runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ConfigurationSectionAttribute : Attribute
    {
        #region Member Variables
        private String _sectName = String.Empty;
        #endregion

        
        #region Constructor
        /// <summary>
        /// Default Constructor for the ConfigurationSectionAttribute
        /// class.
        /// </summary>
        /// <param name="sectName">The name of the ConfigurationSection</param>
        public ConfigurationSectionAttribute(String sectName)
        {
            //Save the member variables
            this._sectName = sectName;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// SectionName determines the name of the Section
        /// to be found.
        /// </summary>
        public String SectionName
        {
            get { return this._sectName; }
            set { this._sectName = value; }
        }
        #endregion
    }
}
