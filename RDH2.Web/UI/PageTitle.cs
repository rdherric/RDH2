using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace RDH2.Web.UI
{
    /// <summary>
    /// The PageTitle control pulls all of the Title information
    /// from the page hierarchy and formats it into a breadcrumb-
    /// type of title.
    /// </summary>
    public class PageTitle : Control
    {
        #region Member variables
        private String _baseTitle = String.Empty;
        private String _separator = " - ";

        private static String _openTag = "<title>";
        private static String _closeTag = "</title>";
        #endregion


        #region Overrides for rendering content
        /// <summary>
        /// CreateChildControls performs the actual creation
        /// of content to render.
        /// </summary>
        protected override void CreateChildControls()
        {
            //Iterate through all of the Parent Controls until
            //the MasterPage object is found
            MasterPage mp = this.FindTopMasterPage(this.Page.Master);

            //Build the Literal and add it to Controls
            Literal lit = new Literal();
            lit.Text = PageTitle._openTag + 
                this._baseTitle + this._separator + mp.Page.Title + 
                PageTitle._closeTag;

            this.Controls.Add(lit);
        }        
        #endregion


        #region Public Properties
        /// <summary>
        /// BaseTitle determines the Title that should
        /// be at the beginning of every breadcrumb.
        /// </summary>
        public String BaseTitle
        {
            get { return this._baseTitle; }
            set { this._baseTitle = value; }
        }


        /// <summary>
        /// Separator determines the string that should 
        /// separate all of the sub-Titles.
        /// </summary>
        public String Separator
        {
            get { return this._separator; }
            set { this._separator = value; }
        }
        #endregion


        #region Recursive Method to find MasterPage
        /// <summary>
        /// FindMasterPage interates recursively through the Control
        /// Hierarchy to find the MasterPage.  If one isn't found, 
        /// then NULL is returned.
        /// </summary>
        /// <param name="ctrl">The MasterPage that may be embedded</param>
        /// <returns>MasterPage if found, NULL otherwise</returns>
        private MasterPage FindTopMasterPage(MasterPage mp)
        {
            //If the MasterPage of this MasterPage isn't null, 
            //get the next level up
            if (mp.Master != null)
                return this.FindTopMasterPage(mp.Master);
            else
                return mp;
        }
        #endregion
    }
}
