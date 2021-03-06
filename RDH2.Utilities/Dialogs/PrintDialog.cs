﻿using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Text;
using System.Windows.Forms;

using RDH2.Utilities.Win32;

namespace RDH2.Utilities.Dialogs
{
    /// <summary>
    /// PrintDialog overrides the Common Print Dialog
    /// by using P/Invoke to do the hard part of setting
    /// up the Dialog.
    /// </summary>
    public class PrintDialog : CommonDialog
    {
        #region Member variables
        private Control _ctrl = null;
        private PrintDocument _document = null;
        private Boolean _printToFile = true;
        private UInt16 _fromPage = UInt16.MinValue;
        private UInt16 _toPage = UInt16.MinValue;
        private UInt16 _minPage = UInt16.MinValue;
        private UInt16 _maxPage = UInt16.MinValue;
        private UInt16 _copies = UInt16.MinValue;

        //Control Disabling Booleans
        private Boolean _disablePageNums = false;
        private Boolean _disableSelection = false;
        private Boolean _showHelp = false;

        //Contron Padding const
        private const Int32 CtrlPadding = 5;
        #endregion


        #region CommonDialog Implementation
        /// <summary>
        /// Reset does the initial setup of the PrintDialog so that
        /// a User can go back to the defaults.
        /// </summary>
        public override void Reset()
        {
            
        }


        /// <summary>
        /// RunDialog does the actual work of creating and showing
        /// the Print Dialog.  This is called by the System.
        /// </summary>
        /// <param name="hwndOwner">The HWnd of the owning Form</param>
        /// <returns>Boolean TRUE if the Dialog is set up properly, FALSE otherwise</returns>
        protected override Boolean RunDialog(IntPtr hwndOwner)
        {
            //Declare a value to return
            Boolean rtn = false;

            //Create a new PRINTDLG struct
            PRINTDLG pd = new PRINTDLG();
            pd.Flags = this.GeneratePrintFlag();
            pd.hDevMode = this._document.PrinterSettings.GetHdevmode();
            pd.hDevNames = this._document.PrinterSettings.GetHdevnames();
            pd.hwndOwner = hwndOwner;
            pd.lpfnPrintHook = new HookProcDelegate(this.HookProc);
            pd.nCopies = this._copies;
            pd.nFromPage = this._fromPage;
            pd.nMaxPage = this._maxPage;
            pd.nMinPage = this._minPage;
            pd.nToPage = this._toPage;
            pd.lStructSize = System.Runtime.InteropServices.Marshal.SizeOf(pd);

            try
            {
                //Call the PrintDlg function
                rtn = WndHelper.PrintDlg(ref pd);

                //Set the values in the PrintDocument if the user 
                //clicked OK
                if (rtn == true)
                {
                    //Set the Member variables
                    this._copies = pd.nCopies;
                    this._fromPage = pd.nFromPage;
                    this._maxPage = pd.nMaxPage;
                    this._minPage = pd.nMinPage;
                    this._toPage = pd.nToPage;

                    //Set the PrintDocument with the hDCs returned
                    this._document.PrinterSettings.SetHdevmode(pd.hDevMode);
                    this._document.PrinterSettings.SetHdevnames(pd.hDevNames);
                }
            }
            catch (Exception ex)
            {
                //If there is an Exception, it is likely caused by a bad 
                //printer driver, so throw an Exeption to the effect
                throw new ApplicationException("The current printer caused a Windows error:\n ", ex);
            }

            //If the PrintDlg function returned false, figure out why
            if (rtn == false)
            {
                //Call the CommDlgExtendedError function
                Int32 errCode = WndHelper.CommDlgExtendedError();

                //If the error is 0, then the user Canceled out of
                //the dialog, so do nothing.  Otherwise, throw an
                //Exception.
                if (errCode != 0)
                    throw new ApplicationException("Error in Print Dialog: " + errCode.ToString());
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// HookProc does the actual work of the CommonDialog.  It is used
        /// in PrintDialog to do the setup of the Print Common Dialog.
        /// </summary>
        /// <param name="hWnd">The Handle to the Dialog</param>
        /// <param name="msg">The Windows Message</param>
        /// <param name="wparam">The WPARAM sent by the System</param>
        /// <param name="lparam">The LPARAM sent by the System</param>
        /// <returns></returns>
        protected override IntPtr HookProc(IntPtr hWnd, Int32 msg, IntPtr wparam, IntPtr lparam)
        {
            //If this is the INITDIALOG message, do the customization
            //of the Print Dialog
            if (msg == Convert.ToInt32(WndMsg.WM_INITDIALOG))
            {
                //Hide the Print to File control if necessary
                if (this._printToFile == false)
                    WndHelper.HideControl(hWnd, ControlID.cbPrintToFile);

                //Finally, add the UserControl
                if (this._ctrl != null)
                {
                    //Make the Print Dialog the parent of the Control
                    WndHelper.SetParent(this._ctrl.Handle, hWnd);

                    //Get the rectangle of the Print Range GroupBox
                    //and put the Control below it plus Padding.
                    RECT gbRangeRECT = WndHelper.GetWindowRECT(hWnd, ControlID.gbPrintRange);

                    //Set the location of the UserControl
                    this._ctrl.Location = new System.Drawing.Point(gbRangeRECT.left, gbRangeRECT.bottom + PrintDialog.CtrlPadding);
                    
                    //Get the rectangle of the Printers GroupBox
                    RECT gbPrintersRECT = WndHelper.GetWindowRECT(hWnd, ControlID.gbPrinter);

                    //Scale the control to the size of the Printers GroupBox
                    Single widthFactor = Convert.ToSingle(gbPrintersRECT.Width) / Convert.ToSingle(this._ctrl.Width);
                    this._ctrl.Scale(new System.Drawing.SizeF(widthFactor, 1));

                    //The Control is placed, so resize the Dialog 
                    //and move the Buttons 
                    this.ResizeDialogWindow(hWnd);
                }
            }

            //Pass the message on the base class
            return base.HookProc(hWnd, msg, wparam, lparam);
        }
        #endregion


        #region Public Properties
        /// <summary>
        /// Document determines the actual PrintDocument to be
        /// setup by the Dialog.
        /// </summary>
        public PrintDocument Document
        {
            get { return this._document; }
            set { this._document = value; }
        }


        /// <summary>
        /// CustomControl gets or sets the Custom Control to add to 
        /// the bottom of the Print Dialog.
        /// </summary>
        public Control CustomControl
        {
            get { return this._ctrl; }
            set { this._ctrl = value; }
        }

        
        /// <summary>
        /// EnablePrintToFile determines if the user can select
        /// the Print To File checkbox.
        /// </summary>
        public Boolean EnablePrintToFile
        {
            get { return this._printToFile; }
            set { this._printToFile = value; }
        }


        /// <summary>
        /// FromPage gets or sets the value that will be put in 
        /// the From control on the Print Dialog.
        /// </summary>
        public UInt16 FromPage
        {
            get { return this._fromPage; }
            set { this._fromPage = value; }
        }


        /// <summary>
        /// ToPage gets or sets the value that will be put in 
        /// the To control on the Print Dialog.
        /// </summary>
        public UInt16 ToPage
        {
            get { return this._toPage; }
            set { this._toPage = value; }
        }


        /// <summary>
        /// MinPage gets or sets the minimum value that can be put in 
        /// the to / From control on the Print Dialog.
        /// </summary>
        public UInt16 MinPage
        {
            get { return this._minPage; }
            set { this._minPage = value; }
        }


        /// <summary>
        /// MaxPage gets or sets the maximum value that can be put in 
        /// the to / From control on the Print Dialog.
        /// </summary>
        public UInt16 MaxPage
        {
            get { return this._maxPage; }
            set { this._maxPage = value; }
        }


        /// <summary>
        /// Copies gets or sets the number of copies shown in the
        /// Print Dialog.
        /// </summary>
        public UInt16 Copies
        {
            get { return this._copies; }
            set { this._copies = value; }
        }


        /// <summary>
        /// DisablePageNums determines if the Page Number controls
        /// are enabled or not.
        /// </summary>
        public Boolean DisablePageNums
        {
            get { return this._disablePageNums; }
            set { this._disablePageNums = value; }
        }


        /// <summary>
        /// DisableSelection determines if the Selection control
        /// is enabled or not.
        /// </summary>
        public Boolean DisableSelection
        {
            get { return this._disableSelection; }
            set { this._disableSelection = value; }
        }


        /// <summary>
        /// ShowHelp determines if the Help button is 
        /// shown or not.
        /// </summary>
        public Boolean ShowHelp
        {
            get { return this._showHelp; }
            set { this._showHelp = value; }
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// GeneratePrintFlag creates the bitflag used to setup the
        /// Print Common Dialog.
        /// </summary>
        /// <returns>PrintFlag bitmask that represents the state of the object</returns>
        private PrintFlag GeneratePrintFlag()
        {
            //Declare a PrintFlag to return
            PrintFlag rtn = PrintFlag.PD_ENABLEPRINTHOOK;

            //Disable the Print to File if necessary
            if (this._printToFile == false)
                rtn |= PrintFlag.PD_HIDEPRINTTOFILE;

            //Disable the PageNums controls if necessary
            if (this._disablePageNums == true)
                rtn |= PrintFlag.PD_NOPAGENUMS;

            //Disable the Selection controls if necessary
            if (this._disableSelection == true)
                rtn |= PrintFlag.PD_NOSELECTION;

            //If the Help button should be shown, show it
            if (this._showHelp == true)
                rtn |= PrintFlag.PD_SHOWHELP;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// ResizeDialogWindow resizes the Dialog Window so that it 
        /// holds the CustomControl well, then moves the buttons
        /// accordingly.
        /// </summary>
        /// <param name="hWnd">The hWnd of the Dialog box</param>
        private void ResizeDialogWindow(IntPtr hWndOwner)
        {
            //Get the RECT of the overall Dialog Window
            RECT dlgRECT = WndHelper.GetWindowRECT(hWndOwner, ControlID.Invalid);

            //Get the RECT of the Printer GroupBox
            RECT printerRECT = WndHelper.GetWindowRECT(hWndOwner, ControlID.gbPrinter);

            //Get the RECT of the Print Range GroupBox
            RECT rangeRECT = WndHelper.GetWindowRECT(hWndOwner, ControlID.gbPrintRange);

            //Get the RECTs of the Buttons
            RECT okRECT = WndHelper.GetWindowRECT(hWndOwner, ControlID.btnOK);
            RECT cancelRECT = WndHelper.GetWindowRECT(hWndOwner, ControlID.btnCancel);

            //Add everything up to a nice height with the ControlPadding
            Int32 winHeight =
                PrintDialog.CtrlPadding +           //Padding above Printer GroupBox
                printerRECT.Height +                //Height of the Printer GroupBox
                PrintDialog.CtrlPadding +           //Padding between Printer and Range GBs
                rangeRECT.Height +                  //Height of the Range GroupBox
                PrintDialog.CtrlPadding +           //Padding between Range GroupBox and CustomControl
                this._ctrl.Bounds.Height +          //Height of the CustomControl
                (PrintDialog.CtrlPadding * 2) +     //Padding between CustomControl and buttons
                okRECT.Height +                     //Height of the Buttons
                (PrintDialog.CtrlPadding * 2) +     //Padding between Buttons and Frame
                Math.Abs(dlgRECT.top) +             //Height of the top of Frame
                Math.Abs(dlgRECT.left);             //Height of bottom of Frame

            //Set the size of the Dialog Window
            WndHelper.SetWindowPos(hWndOwner, IntPtr.Zero, 0, 0, dlgRECT.Width, winHeight, WndPos.SWP_NOMOVE);

            //Calculate the top of the buttons
            Int32 buttonTop =
                this._ctrl.Bounds.Bottom +          //Bottom of the CustomControl
                (PrintDialog.CtrlPadding * 2);     //Padding between CustomControl and Buttons

            //Get the hWnds of the buttons
            IntPtr hOK = WndHelper.GetDlgItem(hWndOwner, ControlID.btnOK);
            IntPtr hCancel = WndHelper.GetDlgItem(hWndOwner, ControlID.btnCancel);

            //Set the position of the Buttons
            WndHelper.SetWindowPos(hOK, IntPtr.Zero, okRECT.left, buttonTop, 0, 0, WndPos.SWP_NOSIZE);
            WndHelper.SetWindowPos(hCancel, IntPtr.Zero, cancelRECT.left, buttonTop, 0, 0, WndPos.SWP_NOSIZE);
        }
        #endregion
    }
}
