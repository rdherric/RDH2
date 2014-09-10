using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;


namespace RDH2.Utilities.Controls
{
    /// <summary>
    /// The EditableListBox Control is actually a ListView
    /// that has been configured to look like a ListBox.
    /// The advantage of doing things that way is that the
    /// Labels of the ListViewItems are by default editable,
    /// which saves a lot of trouble in the long run.
    /// </summary>
    public class EditableListBox : ListView
    {
        #region Member Variables
        private EditableListBox.ListViewItemCollection _items = null;
        private const Int32 _checkBoxWidth = 15;
        #endregion


        #region Constructor
        /// <summary>
        /// Default Constructor for the EditableListBox
        /// </summary>
        public EditableListBox() : base()
        {
            //Create a new ListViewItemCollection for this Control
            this._items = new EditableListBox.ListViewItemCollection(this);
            this._items.ItemAdded += new ListViewItemAddedEventHandler(_items_ItemAdded);

            //Setup the ListView with basic properties
            //to make it look like a ListBox
            this.SetupListBoxProperties();
        }
        #endregion


        #region Setup Methods
        /// <summary>
        /// SetupListBoxProperties does the initial setup
        /// on the ListView.
        /// </summary>
        private void SetupListBoxProperties()
        {
            //Set the properties to make this look like a 
            //standard ListBox
            this.Activation = ItemActivation.Standard;
            this.Alignment = ListViewAlignment.Left;
            this.AllowColumnReorder = false;
            this.AllowDrop = false;
            this.AutoArrange = true;
            this.Columns.Add("Default");
            this.FullRowSelect = true;
            this.HeaderStyle = ColumnHeaderStyle.None;
            this.HideSelection = false;
            this.HotTracking = false;
            this.HoverSelection = false;
            this.LabelEdit = true;
            this.LabelWrap = false;
            this.MultiSelect = false;
            this.Scrollable = true;
            this.ShowItemToolTips = true;
            this.View = View.Details;
        }
        #endregion


        #region Overrides of default functionality
        /// <summary>
        /// FindItemWithText overrides the default find, which 
        /// appears to use the internal ListViewItemCollection 
        /// instead of the Event-enabled one.
        /// </summary>
        /// <param name="itemText">The String to search in the ListView</param>
        /// <returns>ListViewItem if found, null otherwise</returns>
        public new ListViewItem FindItemWithText(String text)
        {
            //Declare a variable to return
            ListViewItem rtn = null;

            //Iterate through the ListView and search for the String
            foreach (ListViewItem item in this._items)
            {
                //If the String is found, set the return and break
                if (item.Text == text)
                {
                    rtn = item;
                    break;
                }
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// Hide of the Items property so that the new Collection
        /// is used instead of the old.
        /// </summary>
        public new ListViewItemCollection Items
        {
            get { return this._items; }
        }
        #endregion


        #region Event Handlers
        /// <summary>
        /// _items_ItemAdded checks the size of the String that is
        /// being added and resizes the ColumnHeader if necessary.
        /// </summary>
        /// <param name="sender">The ListViewItemCollection that fired the event</param>
        /// <param name="e">The EventArgs sent by the System</param>
        void _items_ItemAdded(object sender, EditableListBox.ListViewItemAddedEventArgs e)
        {
            //Get a Graphics object to measure a String
            Graphics g = this.CreateGraphics();

            //Measure the string 
            SizeF stringSize = g.MeasureString(e.Item.Text, e.Item.Font);

            //Turn the value to an Int32
            Int32 width = Convert.ToInt32(stringSize.Width);

            //If the ListView has checkboxes, add the extra width
            if (this.CheckBoxes == true)
                width += EditableListBox._checkBoxWidth;

            //Resize the columns if necessary
            if (width > this.Columns[0].Width)
                this.Columns[0].Width = width;
        }
        #endregion


        #region Custom ListViewItemCollection that has an ItemAdded event
        /// <summary>
        /// ListViewItemCollection is exactly like the ListView.ListViewItemCollection
        /// except that it contains an event for ItemAdded.
        /// </summary>
        public new class ListViewItemCollection : ListView.ListViewItemCollection
        {
            #region Constructor
            /// <summary>
            /// Public Constructor for the ListViewItemCollection
            /// </summary>
            /// <param name="owner">The ListView that owns the Collection</param>
            public ListViewItemCollection(ListView owner)
                : base(owner)
            {
            }
            #endregion


            #region Event Definition
            /// <summary>
            /// Fired when a new Item is added to the ListViewItemCollection.
            /// </summary>
            public event ListViewItemAddedEventHandler ItemAdded;
            #endregion


            #region Method Overrides
            /// <summary>
            /// Override of the Add Method to allow for an Event to be fired.
            /// </summary>
            /// <param name="newItem">The new ListViewItem that is being added</param>
            /// <returns>The ListViewItem that is being added</returns>
            public override ListViewItem Add(ListViewItem newItem)
            {
                //Fire the event if anybody is subscribed
                if (this.ItemAdded != null)
                    this.ItemAdded(this, new ListViewItemAddedEventArgs(newItem));

                //Call the base class method to do the real work
                return base.Add(newItem);
            }
            #endregion
        }


        /// <summary>
        /// EventHandler definition for the ListViewItemCollection object.
        /// </summary>
        /// <param name="sender">The ListViewItemCollection that fires the event</param>
        /// <param name="e">The EventArgs sent by the ListViewItemCollection</param>
        public delegate void ListViewItemAddedEventHandler(Object sender, ListViewItemAddedEventArgs e);


        /// <summary>
        /// ListViewItemAddedEventArgs is used to pass the ListViewItem
        /// that was created by the User back to the ListView.
        /// </summary>
        public class ListViewItemAddedEventArgs
        {
            #region Member Variables
            private ListViewItem _item = null;
            #endregion


            #region Constructor
            /// <summary>
            /// Default constructor for the ListViewItemAddedEventArgs
            /// </summary>
            /// <param name="item">The Item to pass to the Subscriber</param>
            public ListViewItemAddedEventArgs(ListViewItem item)
            {
                //Save the Member variable
                this._item = item;
            }
            #endregion


            #region Public Properties
            /// <summary>
            /// Item returns the ListViewItem that was just added.
            /// </summary>
            public ListViewItem Item
            {
                get { return this._item; }
            }
            #endregion
        }
        #endregion
    }
}
