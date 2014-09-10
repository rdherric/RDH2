using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.LockIn
{
    /// <summary>
    /// ILockInModulationSource is used to set up a 
    /// LockIn.Amplifier with the source of the 
    /// signal modulation.
    /// </summary>
    public interface ILockInModulationSource
    {
        /// <summary>
        /// InputFrequency returns the value of the modulation
        /// signal that is input to the system.
        /// </summary>
        Double InputFrequency { get; }

        
        /// <summary>
        /// The ModulationHigh event is fired when the signal
        /// is in a high state to synchronize the reference
        /// generation.
        /// </summary>
        event EventHandler ModulationHigh;
    }
}
