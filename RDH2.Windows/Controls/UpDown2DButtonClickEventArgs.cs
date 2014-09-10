using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RDH2.Windows.Controls
{
    /// <summary>
    /// UpDown2DButtonClickEventArgs is sent to the consumer
    /// of the ButtonClick event on the UpDown2D Control.
    /// </summary>
    public class UpDown2DButtonClickEventArgs : RoutedEventArgs
    {
        #region Member Variables
        private UpDown2D.ButtonType _type = UpDown2D.ButtonType.None;
        private Double _step = 0.0;
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor for the UpDown2DButtonClickEventArgs object.
        /// </summary>
        /// <param name="buttonType"></param>
        /// <param name="step"></param>
        public UpDown2DButtonClickEventArgs(UpDown2D.ButtonType buttonType, Double step)
        {
            //Save the input in the Member Variables
            this._type = buttonType;
            this._step = step;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// ButtonType determines the Button that was Clicked.
        /// </summary>
        public UpDown2D.ButtonType ButtonType
        {
            get { return this._type; }
        }


        /// <summary>
        /// Step determines the amount of change that should
        /// be applied as a result of the Button Click.
        /// </summary>
        public Double Step
        {
            get { return this._step; }
        }
        #endregion
    }


    /// <summary>
    /// Delegate type to define the EventHandler that
    /// uses the UpDown2DButtonClickEventArgs class.
    /// </summary>
    /// <param name="sender">The UpDown2D Control that was Clicked</param>
    /// <param name="e">The EventArgs that describes the event</param>
    public delegate void UpDown2DButtonClickEventHandler(Object sender, UpDown2DButtonClickEventArgs e);
}
