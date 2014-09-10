using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;

namespace RDH2.Web.UI
{
    /// <summary>
    /// The Copyright control displays a copyright message
    /// that keeps the year up-to-date.
    /// </summary>
    public class Copyright : WebControl
    {
        #region Member Variables
        private String _copyrightText = "All Content Copyright";
        private String _companyName = String.Empty;
        private String _emailAddress = String.Empty;
        private Boolean _showARR = false;

        private static String _copySymbol = "&copy;";
        private static String _mailToText = "mailto:";
        private static String _arrText = "All Rights Reserved";
        #endregion


        #region Overrides of Render methods
        /// <summary>
        /// CreateChildControls adds all of the content
        /// for the Copyright control.
        /// </summary>
        protected override void CreateChildControls()
        {
            //Set up the copyright text
            Literal copyText = new Literal();
            copyText.Text = this._copyrightText + " " + 
                            Copyright._copySymbol + " " + 
                            DateTime.Now.Year.ToString() + " ";

            this.Controls.Add(copyText);

            //Set up the company name as either a Literal 
            //or a Hyperlink based on whether there is an
            //Email address defined.
            if (this._companyName != String.Empty)
            {
                //If there is an Email address, add a Hyperlink.
                //Otherwise, add a Literal.
                if (this._emailAddress == String.Empty)
                {
                    Literal company = new Literal();
                    company.Text = this._companyName;
                    this.Controls.Add(company);
                }
                else
                {
                    HyperLink companyHL = new HyperLink();
                    companyHL.NavigateUrl = Copyright._mailToText + this._emailAddress;
                    companyHL.Text = this._companyName;

                    //If the CssClass has been set, add it to the Hyperlink
                    if (this.CssClass != String.Empty)
                        companyHL.CssClass = this.CssClass;

                    this.Controls.Add(companyHL);
                }
            }

            //Add the All Rights Reserved text if necessary
            if (this._showARR == true)
            {
                //Add the text
                Literal arr = new Literal();
                arr.Text = "<br />" + Copyright._arrText;
                this.Controls.Add(arr);
            }
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// CompanyName determines the name of the company 
        /// that will be displayed as copyrighting the web
        /// site.
        /// </summary>
        public String CompanyName
        {
            get { return this._companyName; }
            set { this._companyName = value; }
        }


        /// <summary>
        /// CopyrightText determines the text that will be
        /// shown before the Copyright symbol.
        /// </summary>
        public String CopyrightText
        {
            get { return this._copyrightText; }
            set { this._copyrightText = value; }
        }


        /// <summary>
        /// EmailAddress determines the address to add to the 
        /// Copyright if set.
        /// </summary>
        public String EmailAddress
        {
            get { return this._emailAddress; }
            set { this._emailAddress = value; }
        }

        
        /// <summary>
        /// ShowAllRightsReserved determines if the text 
        /// "All Rights Reserved" is displayed below the
        /// copyright text.
        /// </summary>
        public Boolean ShowAllRightsReserved
        {
            get { return _showARR; }
            set { this._showARR = value; }
        }
        #endregion
    }
}
