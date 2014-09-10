using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// HardwareConfigBase is the base class for the Hardware
    /// ConfigurationSettings.  It is used primarily to set
    /// the Group Name of the Sections.
    /// </summary>
    [ConfigurationGroup(HardwareConfigBase._groupName)]
    public class HardwareConfigBase : ConfigurationSection
    {
        #region Member Variables
        private const String _groupName = "hardware";
        #endregion
    }
}
