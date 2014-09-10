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
    public class ConfigurationGroupAttribute : Attribute
    {
        #region Member Variables
        private String _groupName = String.Empty;
        #endregion

        
        #region Constructor
        /// <summary>
        /// Default Constructor for the ConfigurationGroupAttribute
        /// class.
        /// </summary>
        /// <param name="groupName">The name of the Configuration Group to find the ConfigurationSection</param>
        public ConfigurationGroupAttribute(String groupName)
        {
            //Save the member variables
            this._groupName = groupName;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// GroupName determines the name of the Configuration Group
        /// in which the Section may be found.
        /// </summary>
        public String GroupName
        {
            get { return this._groupName; }
            set { this._groupName = value; }
        }
        #endregion
    }
}
