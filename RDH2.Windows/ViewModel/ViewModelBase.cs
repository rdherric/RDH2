using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace RDH2.Windows.ViewModel
{
    /// <summary>
    /// ViewModelBase is a base class for a ViewModel
    /// to encapsulate the INotifyPropertyChanged event
    /// and other common tasks.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members
        /// <summary>
        /// The PropertyChanged event is fired when a 
        /// property is changed to notify any subscribers.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion


        #region Helper Methods
        /// <summary>
        /// FirePropertyChanged is called when a property 
        /// changes and needs to notify subscribers.
        /// </summary>
        /// <param name="propName">The name of the Property that changed</param>
        protected void FirePropertyChanged(String propName)
        {
            //Fire the event if necessary
            if (this.PropertyChanged != null)
                this.PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }


        /// <summary>
        /// GetWindowParent 
        /// </summary>
        /// <returns>The top-level Window object of the Application</returns>
        protected Window GetWindowParent(FrameworkElement element)
        {
            //Declare a variable to return 
            Window rtn = null;

            //If this Element is a Window, return it.  Otherwise, 
            //recurse through the Parent.
            if (element is Window)
                rtn = element as Window;
            else
                rtn = this.GetWindowParent(element.Parent as FrameworkElement);

            //Return the result
            return rtn;
        }
        #endregion
    }
}
