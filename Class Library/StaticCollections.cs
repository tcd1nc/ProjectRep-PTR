using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Media;
using System.Collections.ObjectModel;
using static PTR.DatabaseQueries;
using PTR.Models;
using System;

namespace PTR
{
    public static class StaticCollections
    {
        public static Collection<ActivityStatusCodesModel> ActivityStatusCodes { get; private set; }
        public static Collection<GenericObjModel> NewBusinessCategories { get; private set; }
        public static Collection<SMCodeModel> SMCodes { get; private set; }
        public static FullyObservableCollection<CountryModel> Countries { get; set; }
        public static FullyObservableCollection<GenericObjModel> SalesDivisions { get; private set; }
        public static FullyObservableCollection<CustomerModel> Customers { get; set; }
        public static FullyObservableCollection<MarketSegmentModel> MarketSegments { get; private set; }
        public static FullyObservableCollection<GenericObjModel> ProjectTypes { get; private set; }
        public static UserModel CurrentUser;
        public static FullyObservableCollection<UserCustomerAccessModel> UserCustomerAccess { get; private set; }
        public static Dictionary<string, SolidColorBrush> ColorDictionary = new Dictionary<string, SolidColorBrush>();
        public static Collection<string> ProductGroupNames { get; private set; }
        public static List<int> RequiredTrialStatuses { get; private set; }
        public static Collection<GenericObjModel> CDPCCP { get; private set; }
        public static SetupModel Config { get; private set; }
        public static FullyObservableCollection<SalesRegionModel> SalesRegions { get; set; }

        public static void InitializeApp()
        {            
            if (GetCurrentUser())
            {                
                LogAccess(CurrentUser.GOM.ID);
                var (versionok, mustupgrade) = GetVersion();

                if (mustupgrade)
                {
                    App.splashScreen.AddMessage("This version has expired.\nProgram now shutting down.",3000);
                    App.splashScreen?.LoadComplete();
                    App.CloseProgram();
                }
                else
                if (!versionok)
                {
                    App.splashScreen.AddMessage("This version has been retired. \nPlease upgrade now",5000);                
                }                             
                
                try
                {                                           
                    GetUserPermissions();
                    SMCodes = GetSMCodes();
                    Countries = GetCountries();
                    MarketSegments = GetMarketSegments();
                    Customers = GetCustomers();
                    SalesDivisions = GetSalesDivisions();
                    ActivityStatusCodes = GetActivityStatusCodes();
                    CreateColorDict();
                    NewBusinessCategories = GetNewBusinessCategories();
                    ProjectTypes = GetProjectTypes();
                    ProductGroupNames = GetProductGroupNames();
                    RequiredTrialStatuses = GetRequiredTrialStatuses();
                    CDPCCP = GetCDPCCP();
                    Config = GetSetup();
                    SalesRegions = GetSalesRegions();
                }
                catch 
                {
                    App.splashScreen.AddMessage("Unable to load settings.\nProgram is shutting down.",3000);
                    App.splashScreen?.LoadComplete();
                    App.CloseProgram();
                }
            }
            else
            {
                App.splashScreen.AddMessage("User not Authenticated\nThe user was not able to be authenticated \nand the program will close.",3000);
                App.splashScreen?.LoadComplete();
                App.CloseProgram();
            }           
        }
        
        private static bool GetCurrentUser()
        {
#if DEBUG
            CurrentUser = GetUserFromLogin("tcdeed");
#else
          //   CurrentUser = GetUserFromLogin("tcdeed");
           CurrentUser = DatabaseQueries.GetUserFromLogin(Environment.UserName);
#endif                       
            return (CurrentUser.GOM.ID > 0);
        }

        public static void GetUserPermissions()
        {
           UserCustomerAccess = DatabaseQueries.GetUserCustomerAccess(CurrentUser.GOM.ID);                         
        }
        
        #region Create local collections for unchanging parameters
                
        private static void CreateColorDict()
        {
            foreach (ActivityStatusCodesModel ac in ActivityStatusCodes)            
                ColorDictionary.Add(ac.GOM.Name, new SolidColorBrush((Color)ColorConverter.ConvertFromString(ac.Colour)));            
        }

        private static List<int> GetRequiredTrialStatuses()
        {
            List<int> statuses = new List<int>();
            foreach (ActivityStatusCodesModel asc in ActivityStatusCodes)
                if (asc.ReqTrialStatus == true)
                    statuses.Add(asc.GOM.ID);
            return statuses;
        }

        public static string GetActivityDescriptionFromID(int id)
        {
            foreach (ActivityStatusCodesModel asc in ActivityStatusCodes)
                if (asc.GOM.ID == id)
                    return asc.PlaybookDescription;
            return string.Empty;
        }

        public static string GetActivityStatusFromID(int id)
        {
            foreach (ActivityStatusCodesModel asc in ActivityStatusCodes)            
                if (asc.GOM.ID == id)
                    return asc.GOM.Name;            
            return string.Empty;
        }

        public static string GetActivityStatusColorFromID(int id)
        {
            foreach (ActivityStatusCodesModel asc in ActivityStatusCodes)            
                if (asc.GOM.ID == id)
                    return asc.Colour;            
            return string.Empty;
        }

        public static string GetTrialStatusFromID(object obj)
        {
            bool isnumber = int.TryParse(obj.ToString(), out int id);
            foreach (EnumValue ag in EnumerationLists.TrialStatusTypesList)
                if (id == ag.ID)
                    return ag.Description;
            return string.Empty;
        }
        
        public static bool CheckUserAccess(UserPermissionsType permtype)
        {            
            foreach (UserCustomerAccessModel up in UserCustomerAccess)                            
                if (up.AccessID == (int)permtype)
                    return true;            
            return false;
        }

        public static Collection<int> GetUserCountryAccess()
        {
            Collection<int> availcountries = new Collection<int>();
            foreach (UserCustomerAccessModel up in UserCustomerAccess)            
                if (!availcountries.Contains(up.CountryID))
                    availcountries.Add(up.CountryID);            
            return availcountries;
        }

        public static int GetUserCustomerAccess(int customerid)
        {            
            foreach (UserCustomerAccessModel up in UserCustomerAccess)
                if (customerid == up.CustomerID)
                    return up.AccessID;                   
            return 0;
        }

        #endregion

        #region Connection properties

        /// <summary>
        /// Set connection to database
        /// used with connectionstring
        /// </summary>

        private static SqlConnection localconn;

        public static SqlConnection Conn
        {
            get
            {
                if (localconn == null)
                {                   
                    localconn = new SqlConnection
                    {
                        ConnectionString = Application.Current.Resources["DatabaseConnectionString"].ToString()
                    };
                    localconn.Open();                        
                }
                return localconn;
            }
            set
            {
                localconn = value;
            }
        }

        #endregion
    }

    //global constants
    static class Constants
    {
        public const string SalesFunnel = "A-Sls Fnl";
        public const string NewBusiness = "F-New Bsn";
    }
}