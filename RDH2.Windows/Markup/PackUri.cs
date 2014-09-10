using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDH2.Windows.Markup
{
    /// <summary>
    /// PackUri is used to create a URI to a 
    /// Resource in the Application.
    /// </summary>
    public class PackUri : Uri
    {
        #region Member Variables
        private static String _packURI = "pack://application:,,,/{0};component{1}";
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the PackUri object.
        /// </summary>
        /// <param name="assembly">The Assembly to which to point</param>
        /// <param name="path">The Path to the resource</param>
        public PackUri(String assembly, String path) : 
            base(String.Format(PackUri._packURI, assembly, path))
        {
        }
        #endregion
    }
}
