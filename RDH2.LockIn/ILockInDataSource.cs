using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.LockIn
{
    /// <summary>
    /// ILockInSource is implemented by another class to 
    /// provide data to a LockIn.Amplifier so that it can
    /// do the calculational work.
    /// </summary>
    public interface ILockInDataSource
    {
        /// <summary>
        /// GetDataArray returns an Array of data at the specified
        /// sampling rate that is being modulated externally.
        /// </summary>
        /// <returns>Array of modulated data from the system</returns>
        Double[] GetDataArray();


        /// <summary>
        /// CycleInterval determines the number of ms between 
        /// acquisition cycles on the LockIn.Amplifier.
        /// </summary>
        Int32 CycleInterval { get; }


        /// <summary>
        /// PointsPerCycle determines the number of points to 
        /// acquire per cycle by the LockIn.Amplifier.
        /// </summary>
        Int32 PointsPerCycle { get; }


        /// <summary>
        /// CycleRate determines the rate in Hz that data 
        /// should be acquired from the ILockInDataSource.
        /// </summary>
        Int32 CycleRate { get; }
    }
}
