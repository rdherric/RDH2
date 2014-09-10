using System;
using System.Collections.Generic;
using System.Text;

namespace RDH2.Utilities.Win32
{
    /// <summary>
    /// The ControlID enum is used to encapsulate the Control IDs
    /// of the Common Dialogs.
    /// </summary>
    internal enum ControlID
    {
        Invalid = 0x0000,

        //Open File Dialog Controls
        cbFileName = 0x047C,
        txtFileName = 0x0480,

        //Print Dialog Controls
        gbPrinter = 0x0433,
        cbPrintToFile = 0x0410,
        gbPrintRange = 0x0430,
        rbAllPages = 0x0420,
        rbPages = 0x0422,
        lblFrom = 0x0441,
        txtFrom = 0x0480,
        lblTo = 0x0442,
        txtTo = 0x0481,
        rbSelection = 0x421,
        gbCopies = 0x0431,
        lblNumCopies = 0x0444,
        txtCopies = 0x0482,
        iconCollate = 0x043e,
        cbCollate = 0x0411,

        //Common Controls
        btnOK = 0x001,
        btnCancel = 0x002
    }
}
