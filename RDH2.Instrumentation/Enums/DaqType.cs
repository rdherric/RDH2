using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Instrumentation.Enums
{
    /// <summary>
    /// DaqType is the primary method for determining what
    /// kind of DAQ card is configured in the application.
    /// </summary>
    public enum DaqType
    {
        Invalid = -1,
        MCC = 0,
        NI = 1
    }
}
