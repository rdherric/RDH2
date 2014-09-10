using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace RDH2.Utilities.Controls
{
    /// <summary>
    /// NumericTextBox is a regular TextBox that only 
    /// allows numeric input.  Properties can be set
    /// to allow Decimal and Negative numbers.
    /// </summary>
    public class NumericTextBox : TextBox
    {
        #region Member variables
        private Boolean _allowNegative = false;
        private Boolean _allowDecimal = false;

        //Keys Arrays to search
        private Keys[] _digits = new Keys[] { Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D8, Keys.D9, 
            Keys.NumPad0, Keys.NumPad1, Keys.NumPad2, Keys.NumPad3, Keys.NumPad4, Keys.NumPad5, Keys.NumPad6, Keys.NumPad7, Keys.NumPad8, Keys.NumPad9 };
        private Keys[] _decimals = new Keys[] { Keys.Decimal, Keys.OemPeriod };
        private Keys[] _negatives = new Keys[] { Keys.OemMinus, Keys.Subtract };
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor for the NumericTextBox
        /// </summary>
        public NumericTextBox() : base()
        {
            //Subscribe the KeyDown event 
            this.KeyDown += new KeyEventHandler(NumericTextBox_KeyDown);
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// AllowNegative determines whether negative input is 
        /// allowed in the NumericTextBox.
        /// </summary>
        public Boolean AllowNegative
        {
            get { return this._allowNegative; }
            set { this._allowNegative = value; }
        }


        /// <summary>
        /// AllowDecimal determines whether decimal input is 
        /// allowed in the NumericTextBox.
        /// </summary>
        public Boolean AllowDecimal
        {
            get { return this._allowDecimal; }
            set { this._allowDecimal = value; }
        }
        #endregion


        #region Control Events
        /// <summary>
        /// NumericTextBox_KeyDown rejects any input that isn't
        /// setup by the Properties.
        /// </summary>
        /// <param name="sender">The TextBox that had the KeyDown event</param>
        /// <param name="e">The EventArgs sent by the System</param>
        void NumericTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //Setup return values -- default to suppressing the 
            //keypress from occurring
            Boolean handled = true;
            Boolean suppress = true;

            //If the value is a number, let it pass through.  Otherwise, 
            //check to make sure that it's a negative or decimal sign.
            if ((Array.IndexOf(this._digits, e.KeyCode) > -1) ||
               ((Array.IndexOf(this._negatives, e.KeyCode) > -1) && (this._allowNegative == true) && (this.Text.Contains("-") == false)) ||
               ((Array.IndexOf(this._decimals, e.KeyCode) > -1) && (this._allowDecimal == true) && (this.Text.Contains(".") == false)) ||
               (e.KeyCode == Keys.Back))
            {
                handled = false;
                suppress = false;
            }

            //Setup the return
            e.Handled = handled;
            e.SuppressKeyPress = suppress;            
        }
        #endregion
    }
}
