using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace RDH2.Windows.Threading
{
    /// <summary>
    /// DispatcherHelper is used to empty a Thread
    /// of all of the messages in the queue.  This
    /// can be used for very long-running processes
    /// to update the UI.
    /// </summary>
    public class DispatcherHelper
    {
        #region Member Variables
        private FrameworkElement _element = null;
        private delegate void ExitFrameDelegate(DispatcherFrame frame);
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the DispatcherHelper object.
        /// </summary>
        /// <param name="element">The FrameworkElement to help Dispatching</param>
        public DispatcherHelper(FrameworkElement element)
        {
            //Save the input in the member variables
            this._element = element;
        }
        #endregion


        #region EmptyMessageQueue Method
        /// <summary>
        /// EmptyMessageQueue empties the Message Queue
        /// of the FrameworkElement that was supplied as
        /// an argument.
        /// </summary>
        public void EmptyMessageQueue()
        {
            //Create the DispatcherFrame to nest
            DispatcherFrame nestedFrame = new DispatcherFrame();

            //Dispatch a callback to the FrameworkElement's 
            //message queue
            DispatcherOperation exitOperation = this._element.Dispatcher.BeginInvoke(
                new ExitFrameDelegate(this.ExitFrame), DispatcherPriority.Background, nestedFrame);

            // pump the nested message loop, the nested message loop will immediately
            // process the messages left inside the message queue.
            Dispatcher.PushFrame(nestedFrame);

            //If the callback is not finished, Abort it.
            if (exitOperation.Status != DispatcherOperationStatus.Completed)
                exitOperation.Abort();
        }
        #endregion


        #region Multi-Threading Methods
        /// <summary>
        /// ExitFrame simply exits the Nested Frame.
        /// </summary>
        /// <param name="frame">The Frame to exit</param>
        private void ExitFrame(DispatcherFrame frame)
        {
            //Exit the nested message loop
            frame.Continue = false;
        }
        #endregion
    }
}
