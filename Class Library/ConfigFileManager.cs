using System.Data.OleDb;
using System.Xml;

namespace Project_Tracker
{
    public class ConfigFileManager
    {
        XmlDocument _xmlDoc;
        string _xmlFileName;

        public ConfigFileManager(string configpath)
        {
            _xmlFileName = configpath;
        }

        #region Properties

        //public bool UseDefaultSettings
        //{
        //    get { return UseDefSettings(); }
        //}

        public bool ConfigIsWellFormed
        {
            get { return XMLIsWellFormed(); }           
        }

        public bool ConfigFileExists
        {
            get { return ConfigFlExists(); }
        }

        public bool DatabaseFileExists
        {
            get { return DBFileExists(GetConnectionStringFromConfig("ProjectTrackerConnectionString")); }
        }

        #endregion

        #region Public functions

        public string GetConnectionStringFromConfig(string _databaseName)
        {            
            return GetParameter("connectionStrings", "add", "name", _databaseName, "connectionString");
        }

        //public string GetBuckmanConnectURLFromConfig()
        //{
        //    return GetParameter("applicationSettings", "add", "name", "BuckmanConnectURL", "URL");
        //}

        public string GetParameter(string _section, string _elementName, string _paramName, string _paramCriteria, string _attributeName)
        {
            _xmlDoc = GetRootNode();
            if (!string.IsNullOrEmpty(_section))
                _section = _section + "/";

            try
            {
                return _xmlDoc.DocumentElement.SelectSingleNode("//" + _section + _elementName + "[@" + _paramName + "='" + _paramCriteria + "']/@" + _attributeName).Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        public XmlDocument GetRootNode()
        {
            _xmlDoc = new XmlDocument();         
            _xmlDoc.Load(_xmlFileName);
            return _xmlDoc;
        }       

        #endregion

        #region Private functions

        private bool ConfigFlExists()
        {
            return System.IO.File.Exists(_xmlFileName);                       
        }

        private bool DBFileExists(string _databaseConnectionstring)
        {
            string _databaseFileName;

            OleDbConnection conn = new OleDbConnection();
            conn.ConnectionString = _databaseConnectionstring;
            _databaseFileName = conn.DataSource;
            conn.Dispose();
            
            if (System.IO.File.Exists(_databaseFileName))
                return true;
            else
                return false;
        }
        
        private bool XMLIsWellFormed()
        {
            bool _IsWellFormed = false;

            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(_xmlFileName);
                _IsWellFormed = true;
            }
            catch 
            {
                _IsWellFormed =  false;
            }

            return _IsWellFormed;
        }
        
        //private bool UseDefSettings()
        //{
        //    if (System.IO.File.Exists(ConfigFileClass.DefaultDatabasePath))                                      
        //        return true;                            
        //    else
        //        return false;
        //}

        #endregion
    }


    //public static class ConfigFileClass
    //{
          
    //    public static string DatabaseConnectionString
    //    {
    //        get; set;
    //    }
        
    //    public static string DefaultDatabasePath
    //    {
    //        get
    //        {
    //            System.Reflection.Assembly asmly = System.Reflection.Assembly.GetExecutingAssembly();
    //            //To get the Directory path
    //            string theDirectory = System.IO.Path.GetDirectoryName(asmly.Location);
    //            return theDirectory + "\\ProjectTrackerBuffer.accdb;Persist Security Info=False";               
    //        }
    //    }

    //    public static string DefaultDatabaseConnectionString 
    //    { 
    //        get 
    //        {
    //            return "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + DefaultDatabasePath + "'";                
    //        }
    //    }
        
    //    public static string ConfigFilePath
    //    {
    //        get; set;  
    //    }

    //    public static string DBFileName
    //    {
    //        get
    //        {
    //            string _databaseFileName = string.Empty;
    //            OleDbConnection conn = new OleDbConnection();
    //            conn.ConnectionString = DatabaseConnectionString;
    //            _databaseFileName = conn.DataSource;
    //            return _databaseFileName;
    //        }
    //    }
    //}

}
