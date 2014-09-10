using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Instrumentation.Enums
{
    /// <summary>
    /// AcquisitionType determines the type of Dara 
    /// Acquisition that is being performed:  2-D, 
    /// 3-D Research, or 3-D LEGO.
    /// </summary>
    public enum AcquisitionType
    {
        Invalid = 0,
        TwoD = 1,
        ThreeD = 2,
        LEGO = 3
    }
}
