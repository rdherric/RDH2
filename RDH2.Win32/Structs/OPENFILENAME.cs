using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

using RDH2.Win32.Enums;

namespace RDH2.Win32.Structs
{   
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
    internal struct OPENFILENAME
    {
        public Int32 lStructSize;
        public IntPtr hwndOwner;
        public IntPtr hInstance;
        public IntPtr lpstrFilter;
        public IntPtr lpstrCustomFilter;
        public Int32 nMaxCustFilter;
        public Int32 nFilterIndex;
        public IntPtr lpstrFile;
        public Int32 nMaxFile;
        public IntPtr lpstrFileTitle;
        public Int32 nMaxFileTitle;
        public IntPtr lpstrInitialDir;
        public IntPtr lpstrTitle;
        public OpenFileFlags Flags;
        public UInt16 nFileOffset;
        public UInt16 nFileExtension;
        public IntPtr lpstrDefExt;
        public Int32 lCustData;
        public HookProcDelegate lpfnHook;
        public IntPtr lpTemplateName;
        public IntPtr pvReserved;
        public Int32 dwReserved;
        public OpenFileFlags FlagsEx;
    }


    /// <summary>
    /// OpenFileFlags contains all of the flags that are
    /// defined to setup an OpenFileDialog.
    /// </summary>
    internal enum OpenFileFlags
    {
        OFN_READONLY = 0x00000001,
        OFN_OVERWRITEPROMPT = 0x00000002,
        OFN_HIDEREADONLY = 0x00000004,
        OFN_NOCHANGEDIR = 0x00000008,
        OFN_SHOWHELP = 0x00000010,
        OFN_ENABLEHOOK = 0x00000020,
        OFN_ENABLETEMPLATE = 0x00000040,
        OFN_ENABLETEMPLATEHANDLE = 0x00000080,
        OFN_NOVALIDATE = 0x00000100,
        OFN_ALLOWMULTISELECT = 0x00000200,
        OFN_EXTENSIONDIFFERENT = 0x00000400,
        OFN_PATHMUSTEXIST = 0x00000800,
        OFN_FILEMUSTEXIST = 0x00001000,
        OFN_CREATEPROMPT = 0x00002000,
        OFN_SHAREAWARE = 0x00004000,
        OFN_NOREADONLYRETURN = 0x00008000,
        OFN_NOTESTFILECREATE = 0x00010000,
        OFN_NONETWORKBUTTON = 0x00020000,
        OFN_NOLONGNAMES = 0x00040000,     // force no long names for 4.x modules
        OFN_EXPLORER = 0x00080000,     // new look commdlg
        OFN_NODEREFERENCELINKS = 0x00100000,
        OFN_LONGNAMES = 0x00200000,     // force long names for 3.x modules
        OFN_ENABLEINCLUDENOTIFY = 0x00400000,     // send include message to callback
        OFN_ENABLESIZING = 0x00800000,
        OFN_DONTADDTORECENT = 0x02000000,
        OFN_FORCESHOWHIDDEN = 0x10000000,    // Show All files including System and hidden files
        OFN_EX_NOPLACESBAR = 0x00000001,
    }


    /// <summary>
    /// OFNOTIFY is used by the OpenFileDialog when any 
    /// action is taken.  It is sent in the lParam 
    /// parameter to the HookProc.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Auto)]
    internal struct OFNOTIFY
    {
        public NMHDR hdr;
        public IntPtr lpOFN;
        public IntPtr pszFile;        // May be NULL
    }
}