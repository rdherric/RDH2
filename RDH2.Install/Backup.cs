using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

namespace RDH2.Install
{
    /// <summary>
    /// The Backup class does Backup operations within
    /// an installation.
    /// </summary>
    public class Backup
    {
        #region Member variables
        private static String _appRootKey = "APPLICATIONROOTDIR";
        #endregion


        /// <summary>
        /// BackupDirectory gets the current application directory
        /// from the Session and backs up the files in it to the 
        /// Desktop.  This is required because the code does not 
        /// access to the Program Files directory.
        /// </summary>
        /// <param name="session">The MSI Session variable</param>
        /// <returns>ActionResult to indicate success or failure</returns>
        [CustomAction]
        public static ActionResult BackupDirectory(Session session)
        {
            //Big ol' try-catch just in case
            try
            {
                //Get the Root directory from the MSI Property
                String rootDir = session[Backup._appRootKey];

                //If the directory doesn't exist, then there is no work
                //necessary here, so return
                if (Directory.Exists(rootDir) == false)
                    return ActionResult.Success;

                //Get the list of files in the root directory
                String[] files = Directory.GetFiles(rootDir);

                //Get the first EXE in the Directory
                String exePath = String.Empty;
                foreach (String file in files)
                {
                    if (Path.GetExtension(file).ToUpper() == ".EXE")
                    {
                        exePath = file;
                        break;
                    }
                }

                //If the EXE file was found, create the Backup
                if (exePath != String.Empty)
                {
                    //Get just the EXE file name
                    String exeName = Path.GetFileNameWithoutExtension(exePath);

                    //Get the Version number of the EXE
                    String exeVersion = System.Reflection.Assembly.LoadFile(exePath).GetName().Version.ToString();

                    //Create the full directory name
                    String backupDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Path.Combine(exeName + " Backup", exeVersion));

                    //Create the directory if it doesn't exist
                    if (Directory.Exists(backupDir) == false)
                        Directory.CreateDirectory(backupDir);

                    //Iterate through the files and copy them
                    foreach (String file in files)
                        File.Copy(file, Path.Combine(backupDir, Path.GetFileName(file)), true);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Could not Back Up the Existing Files:\n" + e.Message);
                session.Log(e.Message);
            }

            //Return success
            return ActionResult.Success;
        }
    }
}
