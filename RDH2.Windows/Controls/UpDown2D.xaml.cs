using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using RDH2.Windows.Markup;

namespace RDH2.Windows.Controls
{
    /// <summary>
    /// Interaction logic for UpDown2D.xaml
    /// </summary>
    public partial class UpDown2D : UserControl
    {
        #region Member Variables 
        private const Double _defaultButtonSize = 20.0;
        private const Double _defaultStep = 0.1;

        //DependencyProperties
        public static DependencyProperty ButtonHeightProperty = 
            DependencyProperty.Register("ButtonHeight", typeof(Double), typeof(UpDown2D),
                new PropertyMetadata(UpDown2D._defaultButtonSize,
                    new PropertyChangedCallback(UpDown2D.OnButtonSizeChanged),
                    new CoerceValueCallback(UpDown2D.CoerceButtonSize)));
        
        public static DependencyProperty ButtonWidthProperty =
            DependencyProperty.Register("ButtonWidth", typeof(Double), typeof(UpDown2D),
                new PropertyMetadata(UpDown2D._defaultButtonSize,
                    new PropertyChangedCallback(UpDown2D.OnButtonSizeChanged),
                    new CoerceValueCallback(UpDown2D.CoerceButtonSize)));

        public static DependencyProperty StepProperty =
            DependencyProperty.Register("Step", typeof(Double), typeof(UpDown2D),
                new PropertyMetadata(UpDown2D._defaultStep,
                    new PropertyChangedCallback(UpDown2D.OnStepChanged),
                    new CoerceValueCallback(UpDown2D.CoerceStep)));
        #endregion


        #region Constructor
        /// <summary>
        /// Default constructor for the UpDown2D Control.
        /// </summary>
        public UpDown2D()
        {
            InitializeComponent();
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// ButtonHeight defines the height of the arrow Buttons
        /// on the Control.
        /// </summary>
        public Double ButtonHeight
        {
            get { return (Double)this.GetValue(UpDown2D.ButtonHeightProperty); }
            set { this.SetValue(UpDown2D.ButtonHeightProperty, value); }
        }


        /// <summary>
        /// ButtonWidth defines the width of the arrow Buttons
        /// on the Control.
        /// </summary>
        public Double ButtonWidth
        {
            get { return (Double)this.GetValue(UpDown2D.ButtonWidthProperty); }
            set { this.SetValue(UpDown2D.ButtonWidthProperty, value); }
        }


        /// <summary>
        /// Step determines the amount that the property
        /// should change when a Button is clicked.
        /// </summary>
        public Double Step
        {
            get { return (Double)this.GetValue(UpDown2D.StepProperty); }
            set { this.SetValue(UpDown2D.StepProperty, value); }
        }
        #endregion


        #region Event Handlers
        /// <summary>
        /// ButtonClicked is fired whenever a button on the 
        /// UpDown2D Control is clicked.
        /// </summary>
        public event UpDown2DButtonClickEventHandler ButtonClicked;


        /// <summary>
        /// HandleButtonClicked is used to handle all of the
        /// separate button clicks as a single event.
        /// </summary>
        /// <param name="type">The Type of Button that was clicked</param>
        private void HandleButtonClicked(ButtonType type)
        {
            //If the event has been subscribed to, fire it
            if (this.ButtonClicked != null)
                this.ButtonClicked(this, new UpDown2DButtonClickEventArgs(type, (Double)this.GetValue(UpDown2D.StepProperty)));
        }

        /// <summary>
        /// LeftButtonClick is fired when the Left Button 
        /// is clicked.
        /// </summary>
        /// <param name="sender">The Button that was clicked</param>
        /// <param name="e">The EventArgs sent by the System</param>
        private void LeftButtonClick(object sender, RoutedEventArgs e)
        {
            //Fire the event
            this.HandleButtonClicked(ButtonType.Left);
        }


        /// <summary>
        /// UpButtonClick is fired when the Up Button 
        /// is clicked.
        /// </summary>
        /// <param name="sender">The Button that was clicked</param>
        /// <param name="e">The EventArgs sent by the System</param>
        private void UpButtonClick(object sender, RoutedEventArgs e)
        {
            //Fire the event
            this.HandleButtonClicked(ButtonType.Up);
        }


        /// <summary>
        /// RightButtonClick is fired when the Right Button 
        /// is clicked.
        /// </summary>
        /// <param name="sender">The Button that was clicked</param>
        /// <param name="e">The EventArgs sent by the System</param>
        private void RightButtonClick(object sender, RoutedEventArgs e)
        {
            //Fire the event
            this.HandleButtonClicked(ButtonType.Right);
        }


        /// <summary>
        /// DownButtonClick is fired when the Down Button 
        /// is clicked.
        /// </summary>
        /// <param name="sender">The Button that was clicked</param>
        /// <param name="e">The EventArgs sent by the System</param>
        private void DownButtonClick(object sender, RoutedEventArgs e)
        {
            //Fire the event
            this.HandleButtonClicked(ButtonType.Down);
        }
        #endregion


        #region Static DependencyProperty Methods
        /// <summary>
        /// OnButtonSizeChanged is called when the user has 
        /// set a value on the ButtonHeight or ButtonWidth
        /// properties.
        /// </summary>
        /// <param name="property">The DependencyProperty that was changed</param>
        /// <param name="e">The EventArgs sent by the System</param>
        private static void OnButtonSizeChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            //Cast the DependencyObject to an UpDown2D Control
            UpDown2D ctrl = obj as UpDown2D;

            //Get the values from the Control
            Double width = Convert.ToDouble(ctrl.GetValue(UpDown2D.ButtonWidthProperty));
            Double height = Convert.ToDouble(ctrl.GetValue(UpDown2D.ButtonHeightProperty));

            //Set the values on the Buttons
            ctrl.left.Width = width;
            ctrl.up.Width = width;
            ctrl.right.Width = width;
            ctrl.down.Width = width;

            ctrl.left.Height = height;
            ctrl.up.Height = height;
            ctrl.right.Height = height;
            ctrl.down.Height = height;
        }


        /// <summary>
        /// CoerceButtonSize checks to make sure that the 
        /// value that is entered is bigger than the default
        /// size.
        /// </summary>
        /// <param name="obj">The DependencyObject that owns the property</param>
        /// <param name="value">The value that was entered</param>
        /// <returns>Coerced value if necessary</returns>
        private static Object CoerceButtonSize(DependencyObject obj, Object value)
        {
            //Declare a variable to return
            Double rtn = UpDown2D._defaultButtonSize;

            //Cast the input to the proper type
            Double input = Convert.ToDouble(value);

            //If the value is too small, coerce it.  Otherwise,
            //set the value as specified.
            if (input > rtn)
                rtn = input;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// OnStepChanged is called when the user changes
        /// the Step property.  
        /// </summary>
        /// <param name="obj">The DependencyObject that owns the property</param>
        /// <param name="e">The EventArgs sent by the System</param>
        private static void OnStepChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
        }


        /// <summary>
        /// CoerceStep is called when the user changes the
        /// Step property to make sure that the value is valid.
        /// </summary>
        /// <param name="obj">The DependencyObject that owns the property</param>
        /// <param name="value">The value that was set</param>
        /// <returns>Coerced value if necessary</returns>
        private static Object CoerceStep(DependencyObject obj, Object value)
        {
            //Declare a variable to return
            Double rtn = UpDown2D._defaultStep;

            //Cast the input to the proper type
            Double input = Convert.ToDouble(value);

            //If the value is too small, coerce it.  Otherwise,
            //set the value as specified.
            if (input > rtn)
                rtn = input;

            //Return the result
            return rtn;
        }
        #endregion


        #region Enumeration for determining the type of Button Click
        /// <summary>
        /// The ButtonType enum is sent to the consumer of 
        /// the ButtonClick event when it occurs.
        /// </summary>
        public enum ButtonType
        {
            None = -1,
            Left,
            Up,
            Right,
            Down
        }
        #endregion
    }
}
