using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;

using RDH2.Utilities.Configuration;

namespace RDH2.Instrumentation.Config
{
    /// <summary>
    /// SoftwareConfigBase is the base class for the Software
    /// ConfigurationSettings.  It is used primarily to set
    /// the Group Name of the Sections.
    /// </summary>
    [ConfigurationGroup(SoftwareConfigBase._groupName)]
    public class SoftwareConfigBase : ConfigurationSection
    {
        #region Member Variables
        private const String _groupName = "software";
        #endregion
    }
}
