﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using RDH2.Win32.Enums;
using RDH2.Win32.Structs;

namespace RDH2.Win32.PInvoke
{
    /// <summary>
    /// User32 wraps the P/Invoke calls to the 
    /// user32.DLL library.
    /// </summary>
    internal class User32
    {
        #region P/Invoke Definitions
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, String lpszClass, String lpszWindow);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetDlgItem(IntPtr hwnd, ControlID item_id);

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean GetWindowRect(IntPtr hwnd, ref RECT rect);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern Int32 GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean ScreenToClient(IntPtr hwnd, ref POINT point);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        public static extern IntPtr SendMessage(IntPtr hWnd, WndMsg Msg, Int32 wParam, Int32 lParam);

        [DllImport("user32.dll")]
        public static extern Boolean SetDlgItemText(IntPtr hDlg, ControlID nIDDlgItem, String lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SetParent(IntPtr hwnd, IntPtr parent);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean SetWindowPos(IntPtr hwnd, IntPtr insert_after, Int32 x, Int32 y, Int32 width, Int32 height, WndPos flags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Boolean SetWindowText(IntPtr hwnd, String lpString);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern Int32 MapWindowPoints(IntPtr hwndFrom, IntPtr hwndTo, ref POINT lpPoints, UInt32 cPoints);
        #endregion


        #region Helper Methods
        /// <summary>
        /// GetWindowRECT gets the RECT associated with the given
        /// ControlID.
        /// </summary>
        /// <param name="ctrlID">The ID of the Control to retrieve</param>
        /// <returns>A RECT with the appropriate control data in it</returns>
        public static RECT GetWindowRECT(IntPtr hWndOwner, ControlID ctrlID)
        {
            //Declare a variable to return
            RECT rtn = new RECT();

            //Get the hWnd of the Control if the ID is valid.
            //Otherwise get the actual size of the Window.
            IntPtr hCtrl = hWndOwner;
            if (ctrlID != ControlID.Invalid)
                hCtrl = User32.GetDlgItem(hWndOwner, ctrlID);

            //If the Control was found, get the RECT
            if (hCtrl != IntPtr.Zero)
            {
                //Get the RECT in Screen terms
                User32.GetWindowRect(hCtrl, ref rtn);

                //Get the RECT in client terms
                POINT pt = new POINT();
                pt.x = rtn.left;
                pt.y = rtn.top;

                User32.ScreenToClient(hWndOwner, ref pt);

                //Reset the values in the RECT
                Int32 width = rtn.Width;
                Int32 height = rtn.Height;

                rtn.left = pt.x;
                rtn.top = pt.y;
                rtn.right = pt.x + width;
                rtn.bottom = pt.y + height;
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// HideControl takes the ControlID of the specified Control
        /// and hides it.
        /// </summary>
        /// <param name="hwndOwner">The Print Dialog hWnd</param>
        /// <param name="ctrlID">The ID of the Control to hide</param>
        public static void HideControl(IntPtr hwndOwner, ControlID ctrlID)
        {
            //Get the Handle of the Control to hide
            IntPtr hctrl = User32.GetDlgItem(hwndOwner, ctrlID);

            //Set the Control to hidden
            if (hctrl != IntPtr.Zero)
            {
                User32.SetWindowPos(hctrl, IntPtr.Zero, 0, 0, 0, 0,
                    WndPos.SWP_HIDEWINDOW | WndPos.SWP_NOMOVE | WndPos.SWP_NOSIZE);
            }
        }
        #endregion
    }
}
