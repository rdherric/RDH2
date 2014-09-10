using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// General holds the base configuration properties
    /// of the Hardware.
    /// </summary>
    [ConfigurationSection(General._sectName)]
    public class General : HardwareConfigBase
    {
        #region Member Variables
        private const String _sectName = "general";
        private const String _isConfiguredKey = "isConfigured";
        private const String _doNotConfigureKey = "doNotConfigure";
        #endregion


        #region Public Properties
        /// <summary>
        /// IsConfigured determines whether the hardware has been 
        /// configured or not.
        /// </summary>
        [ConfigurationProperty(General._isConfiguredKey, DefaultValue = false, IsRequired = false)]
        public Boolean IsConfigured
        {
            get { return Convert.ToBoolean(this[General._isConfiguredKey]); }
            set { this[General._isConfiguredKey] = value; }
        }


        /// <summary>
        /// DoNotConfigure determines whether the hardware Wizard 
        /// should be opened or not if the Hardware has not been
        /// configured.
        /// </summary>
        [ConfigurationProperty(General._doNotConfigureKey, DefaultValue = false, IsRequired = false)]
        public Boolean DoNotConfigure
        {
            get { return Convert.ToBoolean(this[General._doNotConfigureKey]); }
            set { this[General._doNotConfigureKey] = value; }
        }
        #endregion
    }
}
