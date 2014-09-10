using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace RDH2.Utilities.Win32
{
    /// <summary>
    /// RECT holds the rectangle that a Window occupies.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct RECT
    {
        public Int32 left;
        public Int32 top;
        public Int32 right;
        public Int32 bottom;

        #region Properties to make getting Width and Height easier
        /// <summary>
        /// Width calculates the Width of the rectangle.
        /// </summary>
        public Int32 Width
        {
            get { return this.right - this.left; }
        }


        /// <summary>
        /// Height calculates the Height of the rectangle.
        /// </summary>
        public Int32 Height
        {
            get { return this.bottom - this.top; }
        }
        #endregion
    }
}
