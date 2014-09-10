using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;

namespace RDH2.Configuration
{
    /// <summary>
    /// ConfigHelper is used to retrieve ConfigurationSections
    /// from the app.config file.  ConfigHelper uses Reflection
    /// to determine the Group and Section names, then uses the
    /// System.ConfigurationManager class to get the section.
    /// If the ConfigurationSection is not found in the app.config
    /// file, ConfigHelper uses Reflection to create and fill an
    /// instance of the object with default values.
    /// </summary>
    /// <typeparam name="T">The ConfigurationSection Type to retrieve</typeparam>
    public class ConfigHelper<T> where T : ConfigurationSection, new()
    {
        #region Member variables
        private ConfigLocation _location = ConfigLocation.Application;
        #endregion


        #region Constructors
        /// <summary>
        /// Default Constructor for the ConfigHelper class.  Implies 
        /// that the Application Configuration location should be 
        /// used to read settings.
        /// </summary>
        public ConfigHelper() { }


        /// <summary>
        /// Constructor for ConfigHelper that specifies the location of 
        /// the Configuration file to use for reading and writing
        /// settings.
        /// </summary>
        /// <param name="location">The location of the Configuration file</param>
        public ConfigHelper(ConfigLocation location)
        {
            //Save the member variable
            this._location = location;
        }
        #endregion


        #region ConfigurationSection Get and Set
        /// <summary>
        /// GetSection retrieves the ConfigurationSection from
        /// the app.config file.
        /// </summary>
        /// <returns>A ConfigurationSection derived configuration object</returns>
        public T GetConfig()
        {
            //Declare a T to return
            T rtn = null;

            //Get the path to the Section
            String path = this.ConstructSectionPath(this.GetGroupName(), this.GetSectionName());

            //Get the Configuration object for the appropriate location
            System.Configuration.Configuration config = this.GetLocatedConfiguration();

            //If the Configuration object could be retrieved, try to
            //get the ConfigurationSection.  If an Exception occurs, 
            //it is probably due to an invalid configuration file, 
            //so just swallow the Exception and return a default.
            if (config != null)
            {
                try
                {
                    rtn = config.GetSection(path) as T;
                }
                catch { }
            }

            //If the ConfigurationSection was not found, create a
            //new T and fill it with the default values
            if (rtn == null)
                rtn = this.CreateConfig();

            //Return the result
            return rtn;
        }


        /// <summary>
        /// SetConfig sets the values in the app.config file.
        /// </summary>
        /// <param name="section">The ConfigurationSection Properties to save</param>
        public void SetConfig(T section)
        {
            //Get the Group and Section names
            String groupName = this.GetGroupName();
            String sectName = this.GetSectionName();

            //Set the ForceSave Boolean so that the info
            //goes to the app.config file
            section.SectionInformation.ForceSave = true;

            //Open app.config of the application
            System.Configuration.Configuration config = this.GetLocatedConfiguration();

            //Get the Group from the Config File
            ConfigurationSectionGroup group = config.SectionGroups[groupName];

            //If the Group wasn't found, create it
            if (group == null)
            {
                group = new ConfigurationSectionGroup();
                config.SectionGroups.Add(groupName, group);
            }

            //Set the Section in the app.config
            group.Sections.Remove(sectName);
            group.Sections.Add(sectName, section);

            //Save the configuration file
            config.Save();

            //Get the full path
            String path = this.ConstructSectionPath(groupName, sectName);

            //Refresh the Settings object
            ConfigurationManager.RefreshSection(path);
        }


        /// <summary>
        /// GetWritableConfig gets the ConfigurationSection, creates
        /// a blank ConfigurationSection, and copies all of the data
        /// from the real Config to the writeable copy.  This is used
        /// as an input for SetConfig().
        /// </summary>
        /// <returns>A writeable copy of the ConfigurationSection</returns>
        public T GetWriteableConfig()
        {
            //Declare a variable to return
            T rtn = new T();

            //Get the current Configuration
            T currCfg = this.GetConfig();

            //Get a list of the Properties for the Object
            Type tType = typeof(T);
            PropertyInfo[] piArray = tType.GetProperties();

            //Iterate through the PropertyInfo objects and 
            //fill them with the current values from the 
            //ConfigurationProperty attribute
            foreach (PropertyInfo pi in piArray)
            {
                //Try to get the ConfigurationProperty Attribute
                ConfigurationPropertyAttribute cpa =
                    Attribute.GetCustomAttribute(pi, typeof(ConfigurationPropertyAttribute)) as ConfigurationPropertyAttribute;

                //If the Attribute was retrieved, set the Property
                if (cpa != null)
                    pi.SetValue(rtn, pi.GetValue(currCfg, null), null);
            }

            //Return the result
            return rtn;
        }
        #endregion


        #region Reflection Methods
        /// <summary>
        /// GetGroupName retrieves the name of the ConfigurationGroup
        /// from the class using Reflection.
        /// </summary>
        /// <returns>The Group Name if found, String.Empty otherwise</returns>
        private String GetGroupName()
        {
            //Declare a variable to return
            String rtn = String.Empty;

            //Get the list of Custom Attributes
            ConfigurationGroupAttribute cfgGroup = 
                Attribute.GetCustomAttribute(typeof(T), typeof(ConfigurationGroupAttribute)) as ConfigurationGroupAttribute;

            //If the attribute was returned, get the GroupName
            if (cfgGroup != null)
                rtn = cfgGroup.GroupName;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// GetSectionName retrieves the name of the ConfigurationSection
        /// from the class using Reflection.
        /// </summary>
        /// <returns>The Section Name if found, String.Empty otherwise</returns>
        private String GetSectionName()
        {
            //Declare a variable to return
            String rtn = String.Empty;

            //Get the list of Custom Attributes
            ConfigurationSectionAttribute cfgSection =
                Attribute.GetCustomAttribute(typeof(T), typeof(ConfigurationSectionAttribute)) as ConfigurationSectionAttribute;

            //If the attribute was returned, get the SectionName
            if (cfgSection != null)
                rtn = cfgSection.SectionName;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// CreateConfig creates a new T object and fills it
        /// with the default values in the Attributes.
        /// </summary>
        /// <returns></returns>
        private T CreateConfig()
        {
            //Declare a variable to return
            T rtn = new T();

            //Get a list of the Properties for the Object
            Type tType = typeof(T);
            PropertyInfo[] piArray = tType.GetProperties();

            //Iterate through the PropertyInfo objects and 
            //fill them with the default values from the 
            //ConfigurationProperty attribute
            foreach (PropertyInfo pi in piArray)
            {
                //Try to get the ConfigurationProperty Attribute
                ConfigurationPropertyAttribute cpa = 
                    Attribute.GetCustomAttribute(pi, typeof(ConfigurationPropertyAttribute)) as ConfigurationPropertyAttribute;

                //If the Attribute was retrieved, set the Property
                if (cpa != null)
                    pi.SetValue(rtn, cpa.DefaultValue, null);
            }

            //Return the result
            return rtn;
        }


        /// <summary>
        /// GetProgramDataPath uses Reflection on the Assembly to 
        /// retrieve the Application name and Company to create the
        /// appropriate path to the EXE config file that can be 
        /// written to.
        /// </summary>
        /// <returns>String to ProgramData directory</returns>
        private String GetProgramDataPath()
        {
            //Declare a variable to return
            String rtn = String.Empty;

            //Get the ProgramData directory
            String programData = System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);

            //Use Reflection to get the Assembly properties
            Assembly exe = Assembly.GetEntryAssembly();
            String appName = exe.GetName().Name;
            
            //Get the custom attributes from the Assembly
            Object[] attrs = exe.GetCustomAttributes(true);

            //Iterate through the Custom Attributes and try
            //to find the Company Name
            String companyName = String.Empty;
            foreach (Object attr in attrs)
            {
                if (attr is AssemblyCompanyAttribute)
                {
                    companyName = ((AssemblyCompanyAttribute)attr).Company;
                    break;
                }
            }

            //Put together the whole shebang
            rtn = Path.Combine(programData, Path.Combine(companyName, Path.Combine(appName, appName + ".exe.config")));

            //Return the result
            return rtn;
        }
        #endregion


        #region Helper Methods
        /// <summary>
        /// ConstructSectionPath creates the complete path to the
        /// Section based on the parameters.
        /// </summary>
        /// <param name="groupName">The name of the ConfigurationGroup</param>
        /// <param name="sectionName">The name of the ConfigurationSection</param>
        /// <returns>Complete path to the Section</returns>
        private String ConstructSectionPath(String groupName, String sectionName)
        {
            //Declare a variable to return
            String rtn = groupName;

            //Add a / if necessary
            if (rtn != String.Empty)
                rtn += "/";

            //Add the Section name
            rtn += sectionName;

            //Return the result
            return rtn;
        }


        /// <summary>
        /// GetLocatedConfiguration retrieves the Configuration object 
        /// specified by the ConfigLocation member variable.
        /// </summary>
        /// <returns></returns>
        private System.Configuration.Configuration GetLocatedConfiguration()
        {
            //Declare a variable to return
            System.Configuration.Configuration rtn = null;

            //Get the Configuration object based on the Location
            if (this._location == ConfigLocation.Application)
                rtn = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            else if (this._location == ConfigLocation.AllUsers)
            {
                //Create the ExeConfigurationFileMap
                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = this.GetProgramDataPath();

                //Get the Configuration
                rtn = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            }
            else if (this._location == ConfigLocation.CurrentUser)
                rtn = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoaming);

            //Return the result
            return rtn;
        }
        #endregion
    }


    #region Enum to determine which Config file to use
    /// <summary>
    /// ConfigLocation determines the location of the Config
    /// file to use for opening, reading, and writing 
    /// Configuration Settings.
    /// </summary>
    public enum ConfigLocation
    {
        /// <summary>
        /// The Application base directory is used.  This file usually
        /// cannot be written to in Vista+.
        /// </summary>
        Application = 0,

        /// <summary>
        /// The Program Data directory used for machine-level configuration
        /// of an application.  Supports read/write operations.
        /// </summary>
        AllUsers = 1,

        /// <summary>
        /// The User Data directory used for user-level configuration of
        /// an application.  Supports read/write operations.
        /// </summary>
        CurrentUser = 2
    }
    #endregion
}
