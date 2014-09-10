using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Utilities.Format
{
    /// <summary>
    /// TimeSpanConverter takes a TimeSpan and converts the
    /// value into a human-readable String.
    /// </summary>
    public static class TimeSpanFormatter
    {
        /// <summary>
        /// ToHMSString takes a TimeSpan and converts the 
        /// value into a String in the HH:MM:SS format.
        /// </summary>
        /// <param name="ts">The TimeSpan to convert</param>
        /// <returns>HMS String of the TimeSpan</returns>
        public static String ToHMSString(TimeSpan ts)
        {
            //Format the String
            return ts.Hours.ToString("00") + ":" + ts.Minutes.ToString("00") + ":" + ts.Seconds.ToString("00");
        }
    }
}
