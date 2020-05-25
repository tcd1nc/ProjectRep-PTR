using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Collections.ObjectModel;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Controls;
using System.Windows.Data;
using System.Data;
using System;
using System.Linq;

namespace PTR
{
    public static class StaticCollections
    {
        public static Collection<ActivityStatusCodesModel> ActivityStatusCodes { get; private set; }
        public static UserModel CurrentUser { get; private set; }
        public static FullyObservableCollection<UserCustomerAccessModel> UserCustomerAccess { get; private set; }
        public static SetupModel Config { get; set; }
        public static FullyObservableCollection<TrialStatusModel> TrialStatuses { get; private set; }
        public static Dictionary<string, string> DictConstants { get; private set; }
        public static FullyObservableCollection<ProjectTypeModel> ProjectTypes { get; set; }
        public static FullyObservableCollection<ModelBaseVM> BusinessUnits { get; set; }

        public static void InitializeApp()
        {
            try
            {
                if (GetCurrentUser())
                {
                    LogAccess(CurrentUser.ID);
                    var (versionok, mustupgrade, url, executable) = GetVersion();
                                       
                    if (!versionok)
                    {
                        if(!string.IsNullOrEmpty(url))
                            Clipboard.SetText(url);

                        if (mustupgrade)
                        {
                            UpdateVersion.Update(url, executable);

                            //App.splashScreen.AddMessage("This version has expired.\nPlease upgrade now\nThe program will shut down in 3 seconds.", 3000);
                            //App.splashScreen?.LoadComplete();
                            //App.CloseProgram();

                        }
                        else                                                   
                            App.splashScreen.AddMessage("This version has been retired.\nPlease upgrade", 3000);                        
                    }

                    try
                    {
                        GetUserPermissions();
                        CreateSystemConstants();
                        Config = GetSetup();                        
                        ActivityStatusCodes = GetActivityStatusCodes();
                        TrialStatuses = GetTrialStatuses();
                        ProjectTypes = GetProjectTypes();
                        BusinessUnits = GetBusinessUnits();
                    }
                    catch
                    {
                        try
                        {
                            App.splashScreen.AddMessage("Unable to load settings.\nProgram is shutting down.", 3000);
                            App.splashScreen?.LoadComplete();
                            App.CloseProgram();
                        }
                        catch
                        {
                            Environment.Exit(-1);
                        }
                    }
                }
                else
                {
                    try
                    {
                        App.splashScreen.AddMessage("The user was unable to be authenticated\nCheck the internet and VPN connection\nThe program will now close", 4000);
                        App.splashScreen?.LoadComplete();
                        App.CloseProgram();
                    }
                    catch
                    {
                        Environment.Exit(-1);
                    }
                }
            }
            catch
            {
                try
                {
                    App.splashScreen.AddMessage("The program was unable to start", 4000);
                    App.splashScreen?.LoadComplete();
                    App.CloseProgram();
                }
                catch
                {
                    Environment.Exit(-1);
                }
            }
        }
        
        private static bool GetCurrentUser()
        {
#if DEBUG
            CurrentUser = GetUserFromLogin("tcdeed");
#else          
            CurrentUser = DatabaseQueries.GetUserFromLogin(Environment.UserName);
#endif                         
            return (CurrentUser.ID > 0);
        }

        public static void GetUserPermissions()
        {
           UserCustomerAccess = DatabaseQueries.GetUserCustomerAccess(CurrentUser.ID);                         
        }

        #region Create local collections for unchanging parameters
                       
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
        
        private static void CreateSystemConstants()
        {
            DictConstants = GetSystemConstants();
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
            set { localconn = value; }
        }

        #endregion

        #region Grid formatting

        public static void FormatWithStatusColumn(ref DataTable dt, Window wd, ref DataGridAutoGeneratingColumnEventArgs e, ref Dictionary<string, FilterPopupModel> dictFilterPopup, List<string> filterfields)
        {
            try
            {
                var f = new FrameworkElementFactory(typeof(TextBlock));
                Binding b = new Binding(e.Column.Header.ToString())
                {
                    Mode = BindingMode.OneTime                   
                };

                string colname = e.PropertyName;
                switch (colname)
                {
                    case "Status":                      
                        f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                        f.SetValue(TextBlock.TextProperty, b);
                        if (!dictFilterPopup.ContainsKey(colname))
                            dictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = dt.Columns[colname].Caption, IsApplied = false });

                        FilterPopupModel s = new FilterPopupModel();
                        bool success = dictFilterPopup.TryGetValue(colname, out s);

                        foreach (DataRow dr in dt.Rows)
                            if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
                                s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });

                        //sort!
                        s.FilterData = SortSFStagePopup(s.FilterData);

                        e.Column = new DataGridTemplateColumn()
                        {
                            Header = s,
                            HeaderStyle = wd.FindResource("ColumnHeaderStyle") as Style,
                            HeaderTemplate = wd.FindResource("ColumnHeaderFilterTemplate") as DataTemplate,
                            CellTemplate = wd.FindResource("StatusTemplate") as DataTemplate
                        };
                        e.Column.SortMemberPath = colname;
                        break;
                    case "StatusColour":
                        e.Cancel = true;
                        break;
                    case "CultureCode":
                        e.Cancel = true;
                        break;
                    case "ProjectTypeColour":
                        e.Cancel = true;
                        break;

                    default:
                        if (dt.Columns[colname].ExtendedProperties.ContainsKey("FieldType")
                            && (int)dt.Columns[colname].ExtendedProperties["FieldType"] != (int)ReportFieldType.SystemAndHidden
                            && (int)dt.Columns[colname].ExtendedProperties["FieldType"] != (int)ReportFieldType.SystemAndRemoved)
                        {
                            FormatGridColumn(ref dt, colname, ref b, ref f);
                            FormatWithNoStatusFilterTemplate(colname, ref dt, wd, ref e, ref f, ref dictFilterPopup, filterfields);
                        }
                        else
                            e.Cancel = true;
                        break;
                }
            }
            catch {

            }
        }

        public static void FormatGridColumn(ref DataTable dt, string colname, ref Binding b, ref FrameworkElementFactory f)
        {
            f.SetValue(TextBlock.PaddingProperty, new Thickness(2, 0, 2, 0));           

            if (dt.Columns[colname].ExtendedProperties.ContainsKey("Format"))
                b.StringFormat = dt.Columns[colname].ExtendedProperties["Format"].ToString();

            if (dt.Columns[colname].ExtendedProperties.ContainsKey("Alignment"))
            {
                if (dt.Columns[colname].ExtendedProperties["Alignment"].ToString() == "Left")
                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Left);
                else
                if (dt.Columns[colname].ExtendedProperties["Alignment"].ToString() == "Right")
                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
                else
                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
            }
            else
                f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Left);                    
            
            f.SetValue(TextBlock.TextProperty, b);
        }                             

        public static void FormatWithNoStatusFilterTemplate(string colname, ref DataTable dt, Window wd, ref DataGridAutoGeneratingColumnEventArgs e, ref FrameworkElementFactory f, ref Dictionary<string, FilterPopupModel> dictFilterPopup, List<string> filterfields)
        {
            if (filterfields.Contains(colname))
            {
                if (!dictFilterPopup.ContainsKey(colname))
                    dictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = dt.Columns[colname].Caption, IsApplied = false });

                FilterPopupModel s = new FilterPopupModel();
                bool success = dictFilterPopup.TryGetValue(colname, out s);

                foreach (DataRow dr in dt.Rows)
                    if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
                        s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });

                //sort!
                if(colname == "Customer" || colname == "UserName")
                    s.FilterData = SortPopup(s.FilterData);

                e.Column = new DataGridTemplateColumn()
                {
                    Header = s,
                    HeaderStyle = wd.FindResource("ColumnHeaderStyle") as Style,
                    HeaderTemplate = wd.FindResource("ColumnHeaderFilterTemplate") as DataTemplate,
                    CellTemplate = new DataTemplate() { VisualTree = f },
                    CellStyle = wd.FindResource("CellStyle") as Style
                };
                e.Column.SortMemberPath = colname;                
            }
            else
            {
                e.Column = new DataGridTemplateColumn()
                {
                    Header = dt.Columns[colname].Caption,
                    HeaderStyle = wd.FindResource("ColumnHeaderStyle") as Style,
                    CellTemplate = new DataTemplate() { VisualTree = f },
                    CellStyle = wd.FindResource("CellStyle") as Style
                };
                e.Column.SortMemberPath = colname;
            }
        }

        public static FullyObservableCollection<FilterPopupDataModel> SortPopup(FullyObservableCollection<FilterPopupDataModel> coll)
        {
            try
            {                
                if (coll?.Count > 0)
                {
                    List<FilterPopupDataModel> sorted = coll.OrderBy(x => x.Description).ToList();
                    for (int i = 0; i < sorted.Count(); i++)
                        coll.Move(coll.IndexOf(sorted[i]), i);
                }
            }
            catch
            {
            }
            return coll;
        }

        public static FullyObservableCollection<FilterPopupDataModel> SortSFStagePopup(FullyObservableCollection<FilterPopupDataModel> coll)
        {
            try
            {
                if (coll?.Count > 0)
                {                
                    List<FilterPopupDataModel> sorted = coll.OrderBy(x => Convert.ToInt16(x.Description.Remove(x.Description.IndexOf("-")))).ToList();
                    if(sorted == null || sorted.Count == 0)                    
                        sorted = coll.OrderBy(x => x.Description).ToList();
                    
                    for (int i = 0; i < sorted.Count(); i++)
                        coll.Move(coll.IndexOf(sorted[i]), i);               
                }
            }
            catch
            {
            }
            return coll;
        }

        public static void ReFormatColumns(ref DataTable source, ref DataTable target)
        {
            foreach (DataColumn dc in target.Columns)
            {
                try
                {
                    if (source.Columns.Contains(dc.ColumnName))
                    {
                        DataColumn dc1 = source.Columns[dc.ColumnName];

                        dc.Caption = dc1.Caption;
                        dc.DataType = dc1.DataType;

                        if (dc1.ExtendedProperties.Contains("Alignment"))
                            dc.ExtendedProperties.Add("Alignment", dc1.ExtendedProperties["Alignment"]);
                        if (dc1.ExtendedProperties.Contains("Format"))
                            dc.ExtendedProperties.Add("Format", dc1.ExtendedProperties["Format"]);
                        if (dc1.ExtendedProperties.Contains("FieldType"))
                            dc.ExtendedProperties.Add("FieldType", dc1.ExtendedProperties["FieldType"]);
                    }
                }
                catch {}                
            }
            try
            {
                target.TableName = source.TableName;
            }
            catch
            {
                target.TableName = "Default";
            }
        }

        #endregion

        //check that popup filter contains the names of all the filter fields
        public static bool PopupFilterDictContains(List<string> names, Dictionary<string, FilterPopupModel> dictFilterPopup)
        {
            bool contains = true;

            if(names != null && dictFilterPopup != null)                
                foreach (string name in names)
                    if (!dictFilterPopup.Keys.Contains(name))
                        return false;

            return contains;
        }

    }

  
    //global constants
    static class Constants
    {
        public const string UserProjectList = "UserProjectList";
        public const string BasicProjectDetails = "ProjectData";
        public const string SalesFunnelXLTab = "SalesFunnelXLTab";// "A-Sls Fnl";
        public const string SalesFunnelReport = "SalesFunnel";
        //public const string NewBusinessXLTab = "NewBusinessXLTab";// "F-New Bsn";
        //public const string NewBusinessReport = "NewBusiness";
        public const string SalesPipeline = "Projects Value";
        public const string SalesPipelineCount = "Number of Projects";
        public const string SalesPipelineTooltip = "SalesPipelineTooltip";
        public const string StatusReport = "StatusReport";
        public const string ProjectReport = "ProjectsList";
        public const string ProjectReportActivities = "Activities";
        public const string EvaluationPlanReport = "EPReport";
        public const string SingleProjectReport = "ProjectReport";
        public const string SingleProjectReportActivities = "Activities";
        public const string PlayBookReportName = "Playbook";
        public const string MasterProjectReportName = "MasterProjectList";
        public const string SalesPipelineReportName = "Sales Pipeline Report";

        //Popup filters
        public static List<string> MainviewPopupList { get { return new List<string> { "SalesDivision", "KPM", "ProjectStatus", "Customer", "ProjectType" }; } }
        public static List<string> MasterProjectsPopupList { get { return new List<string> { "SalesFunnelStage", "SalesDivision", "IndustrySegment", "KPM", "Customer", "UserName", "DifferentiatedTechnology", "Application", "ProjectStatus", "ProjectType", "SMCode", "NewBusinessCategory" }; } }
        //public static List<string> NewBusinessProjectsPopupList { get { return new List<string> { "SalesDivision", "IndustrySegment", "KPM", "Customer", "UserName", "DifferentiatedTechnology", "Application", "ProjectType", "SMCode" }; } }
        public static List<string> StatusReportPopupList { get { return new List<string> { "SalesFunnelStage", "SalesDivision", "KPM", "ProjectStatus", "Customer", "UserName", "ProjectType" }; } }
        public static List<string> ProjectListReportPopupList { get { return new List<string> { "SalesDivision", "ProjectStatus", "ProjectType", "UserName", "Customer" }; } }
        public static List<string> EvaluationPlansListReportPopupList { get { return new List<string> { "SalesDivision", "ProjectStatus", "ProjectType", "UserName", "Customer" }; } }
    }
}