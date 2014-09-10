using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RDH2.Windows.Markup
{
    /// <summary>
    /// ImageResourceExtension is a XAML Extension 
    /// for taking a Resource string and converting it into
    /// an ImageSource that is a Resource in the local 
    /// Assemply.
    /// </summary>
    public class ImageResourceExtension : MarkupExtension
    {
        #region Member Variables
        private String _assembly = String.Empty;
        private String _imgPath = String.Empty;
        private static String _icoExt = ".ICO";
        #endregion


        #region MarkupExtension overrides
        /// <summary>
        /// ProvideValue does the actual work of changing
        /// the Path to an ImageSource object.
        /// </summary>
        /// <param name="serviceProvider">IServiceProvider sent by the System</param>
        /// <returns>ImageSource if input is valid, null otherwise.</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            //Declare an object to return
            Object rtn = null;

            //Build a new BitmapImage from the URI
            try
            {
                //Load the Resource -- if the resource is an Icon, 
                //put a frame around it
                if (Path.GetExtension(this._imgPath).ToUpper() == ImageResourceExtension._icoExt)
                    rtn = BitmapFrame.Create(new PackUri(this._assembly, this._imgPath));
                else
                    rtn = new BitmapImage(new PackUri(this._assembly, this._imgPath));
            }
            catch { }

            //Return the result
            return rtn;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// AssemblyName determines the Name of an Assembly
        /// referenced by the Application.
        /// </summary>
        public String AssemblyName
        {
            get { return this._assembly; }
            set { this._assembly = value; }
        }


        /// <summary>
        /// ImagePath determines the Path of the Image
        /// to turn into an ImageSource.
        /// </summary>
        public String ImagePath
        {
            get { return this._imgPath; }
            set { this._imgPath = value; }
        }
        #endregion
    }
}
