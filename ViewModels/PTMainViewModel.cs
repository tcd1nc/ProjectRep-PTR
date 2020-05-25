using System;
using System.Windows.Input;
using static PTR.StaticCollections;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using static PTR.EnumerationManager;
//using System.Linq.Dynamic;

namespace PTR.ViewModels
{
    public class PTMainViewModel : FilterModule
    {
        string title = "Project Tracker";
        private readonly string[] excludedcols = { "Customer" };

        public PTMainViewModel()
        {
            try {
                ExecuteApplyModuleFilter = ExecuteApplyFilter;
                ExecuteFMOpenProject = ExecuteOpenProject;
                ExecuteRFExportToExcel = ExecuteExportToExcel;
                ExecuteClearPopupFilter = ExecuteClearFilterPopup;
                ExecuteResetPopupFilter = ExecuteResetFilterPopup;
                ExecuteApplyPopupFilter = ExecuteApplyFilterPopup;
                ExecuteClearFilters = ExecuteClearDataFilters;

                Title = (CurrentUser.Administrator) ? title + " - " + CurrentUser.Name + " (Administrator)" : title + " - " + CurrentUser.Name;

                SetAccess();
                SetControlState();
                FilterData();
                InitializePopupFilters();                
                ApplyPopupFilter();
            }
            catch
            {
                App.splashScreen.AddMessage("Unable to start Project Tracker", 3000);
                App.splashScreen?.LoadComplete();
                App.CloseProgram();

                //throw new Exception("Unable to start\n" + e.Message.ToString());
            }
            finally
            {
               App.splashScreen?.LoadComplete();
            }
        }
                         
        #region Properties

        public string Title
        {
            get { return title; }
            set { SetField(ref title, value); }
        }

        DataTable userprojects;
        public DataTable UserProjects
        {
            get { return userprojects; }
            set { SetField(ref userprojects, value); }
        }

        decimal sumsalesUSD;
        public decimal SumSalesUSD
        {
            get { return sumsalesUSD; }
            set { SetField(ref sumsalesUSD, value); }
        }
        
        DataRowView selectedproject;
        public DataRowView SelectedProject
        {
            get { return selectedproject; }
            set {                               
                  SetField(ref selectedproject, value);
                  ExportIsEnabled = (value != null);               
            }
        }

        bool isdirtydata = false;
        public bool IsDirtyData
        {
            get { return isdirtydata; }
            set { SetField(ref isdirtydata, value); }
        }

        bool cansave = false;
        public bool CanSave
        {
            get { return cansave; }
            set { SetField(ref cansave, value); }
        }

        ProjectStatusType prstatus = ProjectStatusType.Active;
        public ProjectStatusType ProjectStatus
        {
            get { return prstatus; }
            set {
                if (value != prstatus)
                {                                        
                    try
                    {
                        //Update UserProjects to display correct status
                        SelectedProject["ProjectStatus"] = GetEnumDescription(value);
                        //update masterprojectlist to ensure filtering is correct when next applied
                        DataRow idx = masterprojectlist.Rows.Find(ConvertObjToInt(SelectedProject["ProjectID"]));
                        idx["ProjectStatus"] = GetEnumDescription(value);
                    }
                    catch
                    {
                    }
                    SetField(ref prstatus, value);
                }
            }
        }

        bool exportisenabled = false;
        public bool ExportIsEnabled
        {
            get { return exportisenabled; }
            set { SetField(ref exportisenabled, value); }
        }
       
        FullyObservableCollection<MaintenanceModel> incompleteeps;
        public FullyObservableCollection<MaintenanceModel> IncompleteEPs
        {
            get { return incompleteeps; }
            set { SetField(ref incompleteeps, value); }
        }

        private FullyObservableCollection<MaintenanceModel> missingeps;
        public FullyObservableCollection<MaintenanceModel> MissingEPs
        {
            get { return missingeps; }
            set { SetField(ref missingeps, value); }
        }

        FullyObservableCollection<MaintenanceModel> requiringcompletion;
        public FullyObservableCollection<MaintenanceModel> RequiringCompletion
        {
            get { return requiringcompletion; }
            set { SetField(ref requiringcompletion, value); }
        }

        FullyObservableCollection<MaintenanceModel> overdueactivities;
        public FullyObservableCollection<MaintenanceModel> OverdueActivities
        {
            get { return overdueactivities; }
            set { SetField(ref overdueactivities, value); }
        }

        FullyObservableCollection<MaintenanceModel> milestonesdue;
        public FullyObservableCollection<MaintenanceModel> MilestonesDue
        {
            get { return milestonesdue; }
            set { SetField(ref milestonesdue, value); }
        }
              
        bool clearactivities;
        public bool ClearActivities
        {
            get { return clearactivities; }
            set { SetField(ref clearactivities, value);  }
        }
      
        public void ExecuteClearActivities()
        {
            ClearActivities = true;
            ClearActivities = false;
        }

        bool updateactivities;
        public bool UpdateActivities
        {
            get { return updateactivities; }
            set { SetField(ref updateactivities, value); }
        }
      


        #endregion

        #region Functions for Properties              

        public void ExecuteUpdateActivities()
        {
            UpdateActivities = true;
            UpdateActivities = false;
        }

        private void GetSumSales()
        {
            decimal sumUSD = 0;
            SumSalesUSD = 0;
            if(UserProjects !=null && UserProjects.Rows.Count>0 )
            {
                foreach (DataRow row in UserProjects.Rows)
                    sumUSD = sumUSD + ConvertObjToDecimal(row["EstimatedAnnualSalesUSD"]);
            
                SumSalesUSD = sumUSD;
            }
        }
        
        private void LoadToDoList()
        {
            try
            {
                CheckActions(GetOverdueMonthlyUpdates(), GetProjectsRequiringCompletion(), GetIncompleteEPs(), GetMissingEPs(), GetOverdueMilestones());
            }
            catch
            {                
            }
        }
                     
        public static FullyObservableCollection<MaintenanceModel> LoadFilteredColl(Func<FullyObservableCollection<MaintenanceModel>> fxn)
        {
            FullyObservableCollection<MaintenanceModel> temp = fxn();
            FullyObservableCollection<MaintenanceModel> mmm = new FullyObservableCollection<MaintenanceModel>();
            MaintenanceModel m;
            int accessid = 0;
            foreach (MaintenanceModel mm in temp)
            {
                accessid = StaticCollections.GetUserCustomerAccess(mm.CustomerID);
                if (accessid == (int)UserPermissionsType.FullAccess)
                {
                    m = new MaintenanceModel
                    {
                        ID = mm.ID,
                        ProjectID = mm.ProjectID,
                        ProjectName = mm.ProjectName,
                        IsChecked = false,
                        Email = mm.Email,
                        UserName = mm.UserName,
                        ActionDate = mm.ActionDate,
                        MaintenanceTypeID = mm.MaintenanceTypeID,
                        Enabled = true
                    };
                    mmm.Add(m);
                }
            }
            return mmm;
        }

        public void CheckActions(FullyObservableCollection<MaintenanceModel> overdueactivities, FullyObservableCollection<MaintenanceModel> requiringcompletion,
            FullyObservableCollection<MaintenanceModel> incompleteeps, FullyObservableCollection<MaintenanceModel> missingeps, FullyObservableCollection<MaintenanceModel> milestonesdue)
        {
            try
            {
                int accessid = 0;                
                foreach (DataRow ps in masterprojectlist.Rows)
                {                    
                    accessid = StaticCollections.GetUserCustomerAccess(ConvertObjToInt(ps["CustomerID"]));
                    if (accessid == (int)UserPermissionsType.FullAccess)
                    {
                        ps[GetEnumDescription(MaintenanceType.OverdueActivity)] = false;
                        ps[GetEnumDescription(MaintenanceType.RequiringCompletion)] = false;
                        ps[GetEnumDescription(MaintenanceType.IncompleteEP)] = false;
                        ps[GetEnumDescription(MaintenanceType.MissingEP)] = false;
                        ps[GetEnumDescription(MaintenanceType.MilestoneDue)] = false;

                        foreach (MaintenanceModel mm in overdueactivities)
                            if (mm.ProjectID == ConvertObjToInt(ps["ProjectID"]))                            
                                ps[GetEnumDescription(MaintenanceType.OverdueActivity)] = true;
                            
                        foreach (MaintenanceModel rc in requiringcompletion)
                            if (rc.ProjectID == ConvertObjToInt(ps["ProjectID"]))                            
                                ps[GetEnumDescription(MaintenanceType.RequiringCompletion)] = true;
                            
                        foreach (MaintenanceModel ieps in incompleteeps)
                            if (ieps.ProjectID == ConvertObjToInt(ps["ProjectID"]))                            
                                ps[GetEnumDescription(MaintenanceType.IncompleteEP)] = true;
                            
                        foreach (MaintenanceModel meps in missingeps)
                            if (meps.ProjectID == ConvertObjToInt(ps["ProjectID"]))                            
                                ps[GetEnumDescription(MaintenanceType.MissingEP)] = true;
                            
                        foreach (MaintenanceModel md in milestonesdue)
                            if (md.ProjectID == ConvertObjToInt(ps["ProjectID"]))                            
                                ps[GetEnumDescription(MaintenanceType.MilestoneDue)] = true;
                            
                    }
                }
            }
            catch
            {
            }
        }
               
        DataTable masterprojectlist;
        private void FilterData()
        {
            ExecuteClearActivities();
            masterprojectlist = GetProjectList(CountriesSrchString, AssociatesSrchString);           
            masterprojectlist.PrimaryKey = new DataColumn[] { masterprojectlist.Columns["ProjectID"] };
            LoadToDoList(); 
            UserProjects = masterprojectlist;       
        }

        private void SetControlState()
        {
            EnableProjectList = true;
            EnableActivities = true;
            EnableFilters = true;
            EnableNewProjectBtn = true;
        }

        private void SetAccess()
        {
            List<string> adminmnu = CurrentUser.AdministrationMnu.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ShowCountryMnu = adminmnu.Contains(((int)AdministrationMenuOptions.CountryMnu).ToString());
            ShowCustomerMnu = adminmnu.Contains(((int)AdministrationMenuOptions.CustomerMnu).ToString());
            ShowBUMnu = adminmnu.Contains(((int)AdministrationMenuOptions.BUMnu).ToString());
            ShowExchangeRateMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ExchangeRateMnu).ToString());
            ShowUserMnu = adminmnu.Contains(((int)AdministrationMenuOptions.UserMnu).ToString());
            ShowSalesRegionMnu = adminmnu.Contains(((int)AdministrationMenuOptions.SalesRegionMnu).ToString());
            ShowActivityStagesMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ActivityStatusesMnu).ToString());
            ShowSetupMnu = adminmnu.Contains(((int)AdministrationMenuOptions.SetupMnu).ToString());
            ShowProductNameMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ProductNameMnu).ToString());
            ShowProjectTypesMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ProjectTypeMnu).ToString());
            ShowApplicationsMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ApplicationsMnu).ToString());
            ShowSMCodesMnu = adminmnu.Contains(((int)AdministrationMenuOptions.SMCodesMnu).ToString());
            ShowTrialStatusesMnu = adminmnu.Contains(((int)AdministrationMenuOptions.TrialStatusesMnu).ToString());
            ShowNewBusinessCategoriesMnu = adminmnu.Contains(((int)AdministrationMenuOptions.NewBusinessCategoriesMnu).ToString());
            ShowIndustrySegmentsApplicationsMnu = adminmnu.Contains(((int)AdministrationMenuOptions.IndustrySegmentsApplicationsMnu).ToString());
            ShowIndustrySegmentsMnu = adminmnu.Contains(((int)AdministrationMenuOptions.IndustrySegmentsMnu).ToString());
            ShowIncompleteProjectReasonsMnu = adminmnu.Contains(((int)AdministrationMenuOptions.IncompleteProjectReasonsMnu).ToString());
            ShowReportFieldsMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ReportFieldsMnu).ToString());
            ShowMiscellaneousDataMnu = adminmnu.Contains(((int)AdministrationMenuOptions.MiscellaneousDataMnu).ToString());
            ShowAdministrationMnu = (adminmnu.Count > 0);

            List<string> projectmnu = CurrentUser.ProjectsMnu.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ShowProjectMaintenanceMnu = projectmnu.Contains(((int)ProjectsMenuOptions.ProjectMaintMnu).ToString());
            ShowProjectMasterListMnu = projectmnu.Contains(((int)ProjectsMenuOptions.ProjectMasterListMnu).ToString());
            ShowProjectsMnu = (projectmnu.Count > 0);

            List<string> reportsmnu = CurrentUser.ReportsMnu.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ShowSalesPipelineMnu = reportsmnu.Contains(((int)ReportsMenuOptions.SalesPipelineMnu).ToString());
            ShowStatusMnu = reportsmnu.Contains(((int)ReportsMenuOptions.StatusMnu).ToString());
            ShowProjectListMnu = reportsmnu.Contains(((int)ReportsMenuOptions.ProjectListMnu).ToString());
            ShowEvaluationPlansMnu = reportsmnu.Contains(((int)ReportsMenuOptions.EvaluationPlans).ToString());
            ShowCustomReportsMnu = reportsmnu.Contains(((int)ReportsMenuOptions.CustomReportsMnu).ToString());
            ShowReportsMnu = (reportsmnu.Count > 0);

            //if any customer has full access then enable New Project button                     
            if (CheckUserAccess(UserPermissionsType.FullAccess))
            {
                EnableNewProjectBtn = true;
            }

            ShowNewProject = true;
            ShowSave = true;
        }
                     
        #endregion
        
        #region Permissions Properties - visibility of controls

        bool enableprojectlist;
        public bool EnableProjectList
        {
            get { return enableprojectlist; }
            set { SetField(ref enableprojectlist, value); }
        }

        bool enableactivities;
        public bool EnableActivities
        {
            get { return enableactivities; }
            set { SetField(ref enableactivities, value); }
        }

        bool showadministrationmnu;
        public bool ShowAdministrationMnu
        {
            get { return showadministrationmnu; }
            set { SetField(ref showadministrationmnu, value); }
        }

        bool showreportsmnu;
        public bool ShowReportsMnu
        {
            get { return showreportsmnu; }
            set { SetField(ref showreportsmnu, value); }
        }
        
        bool showcustomermnu;
        public bool ShowCustomerMnu
        {
            get { return showcustomermnu; }
            set { SetField(ref showcustomermnu, value); }
        }

        bool showbumnu;
        public bool ShowBUMnu
        {
            get { return showbumnu; }
            set { SetField(ref showbumnu, value); }
        }

        bool showcountrymnu;
        public bool ShowCountryMnu
        {
            get { return showcountrymnu; }
            set { SetField(ref showcountrymnu, value); }
        }

        bool showsalesregionmnu;
        public bool ShowSalesRegionMnu
        {
            get { return showsalesregionmnu; }
            set { SetField(ref showsalesregionmnu, value); }
        }

        bool showusermnu;
        public bool ShowUserMnu
        {
            get { return showusermnu; }
            set { SetField(ref showusermnu, value); }
        }

        bool showexchangeratemnu;
        public bool ShowExchangeRateMnu
        {
            get { return showexchangeratemnu; }
            set { SetField(ref showexchangeratemnu, value); }
        }

        bool showactivitystagesmnu;
        public bool ShowActivityStagesMnu
        {
            get { return showactivitystagesmnu; }
            set { SetField(ref showactivitystagesmnu, value); }
        }

        bool showsetupmnu;
        public bool ShowSetupMnu
        {
            get { return showsetupmnu; }
            set { SetField(ref showsetupmnu, value); }
        }

        bool showproductnamemnu;
        public bool ShowProductNameMnu
        {
            get { return showproductnamemnu; }
            set { SetField(ref showproductnamemnu, value); }
        }

        bool showprojecttypesmnu;
        public bool ShowProjectTypesMnu
        {
            get { return showprojecttypesmnu; }
            set { SetField(ref showprojecttypesmnu, value); }
        }
                
        bool showApplicationsmnu;
        public bool ShowApplicationsMnu
        {
            get { return showApplicationsmnu; }
            set { SetField(ref showApplicationsmnu, value); }
        }
        
        bool showsmcodesmnu;
        public bool ShowSMCodesMnu
        {
            get { return showsmcodesmnu; }
            set { SetField(ref showsmcodesmnu, value); }
        }
                
        bool showtrialstatusesmnu;
        public bool ShowTrialStatusesMnu
        {
            get { return showtrialstatusesmnu; }
            set { SetField(ref showtrialstatusesmnu, value); }
        }
        
        bool shownewbusinesscategoriesmnu;
        public bool ShowNewBusinessCategoriesMnu
        {
            get { return shownewbusinesscategoriesmnu; }
            set { SetField(ref shownewbusinesscategoriesmnu, value); }
        }
        
        bool showindustrysegmentsapplicationsmnu;
        public bool ShowIndustrySegmentsApplicationsMnu
        {
            get { return showindustrysegmentsapplicationsmnu; }
            set { SetField(ref showindustrysegmentsapplicationsmnu, value); }
        }        

        bool showindustrysegmentsmnu;
        public bool ShowIndustrySegmentsMnu
        {
            get { return showindustrysegmentsmnu; }
            set { SetField(ref showindustrysegmentsmnu, value); }
        }

        bool showincompleteprojectreasonsmnu;
        public bool ShowIncompleteProjectReasonsMnu
        {
            get { return showincompleteprojectreasonsmnu; }
            set { SetField(ref showincompleteprojectreasonsmnu, value); }
        }
        
        bool showreportfieldsmnu;
        public bool ShowReportFieldsMnu
        {
            get { return showreportfieldsmnu; }
            set { SetField(ref showreportfieldsmnu, value); }
        }
        
        bool showmiscellaneousdatamnu;
        public bool ShowMiscellaneousDataMnu
        {
            get { return showmiscellaneousdatamnu; }
            set { SetField(ref showmiscellaneousdatamnu, value); }
        }

        bool showprojectmasterlistmnu;
        public bool ShowProjectMasterListMnu
        {
            get { return showprojectmasterlistmnu; }
            set { SetField(ref showprojectmasterlistmnu, value); }
        }

        bool showprojectmaintmnu;
        public bool ShowProjectMaintenanceMnu
        {
            get { return showprojectmaintmnu; }
            set { SetField(ref showprojectmaintmnu, value); }
        }

        bool showprojectsmnu;
        public bool ShowProjectsMnu
        {
            get { return showprojectsmnu; }
            set { SetField(ref showprojectsmnu, value); }
        }

        bool showsalespipeline;
        public bool ShowSalesPipelineMnu
        {
            get { return showsalespipeline; }
            set { SetField(ref showsalespipeline, value); }
        }

        bool showcustomreportsmnu;
        public bool ShowCustomReportsMnu
        {
            get { return showcustomreportsmnu; }
            set { SetField(ref showcustomreportsmnu, value); }
        }

        bool showstatusmnu;
        public bool ShowStatusMnu
        {
            get { return showstatusmnu; }
            set { SetField(ref showstatusmnu, value); }
        }

        bool showprojectlistmnu;
        public bool ShowProjectListMnu
        {
            get { return showprojectlistmnu; }
            set { SetField(ref showprojectlistmnu, value); }
        }

        bool showevaluationplansmnu;
        public bool ShowEvaluationPlansMnu
        {
            get { return showevaluationplansmnu; }
            set { SetField(ref showevaluationplansmnu, value); }
        }
        
        bool enablefilters;
        public bool EnableFilters
        {
            get { return enablefilters; }
            set { SetField(ref enablefilters, value); }
        }
        
        bool enablenewprojectbtn;
        public bool EnableNewProjectBtn
        {
            get { return enablenewprojectbtn; }
            set { SetField(ref enablenewprojectbtn, value); }
        }
                
        bool shownewproject;
        public bool ShowNewProject
        {
            get { return shownewproject; }
            set { SetField(ref shownewproject, value); }
        }
               
        public bool ShowSave
        {
            get { return caneditmonthlydata; }
            set { SetField(ref caneditmonthlydata, value); }
        }
                    
        #endregion
                            
        #region Commands - apply filter, open projects, window commands
  
        private void ExecuteApplyFilter(object parameter)
        {
            ExecuteUpdateActivities();
            ExecuteClearActivities();
            FilterData();
            SetPopupFilters(excludedcols, UserProjects);
            ApplyPopupFilter();
            ExportIsEnabled = false;
        }

        private bool caneditmonthlydata = false;
                       
        private void ExecuteOpenProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    object[] values = new object[2];
                    values = parameter as object[];
                    int id = (int)values[0];

                    ExecuteUpdateActivities();                                                                              
                                        
                    if (id > 0)
                    {
                        IMessageBoxService msgbox = new MessageBoxService();  
                        //if return value is true then Refresh list                        
                        if(msgbox.OpenProjectDlg((Window)values[1], id))
                        {
                            FilterData();
                            SetPopupFilters(excludedcols, UserProjects);
                            ApplyPopupFilter();
                        }                        
                        msgbox = null;
                    }
                }
            }
            catch
            {               
            }
         }
                
        ICommand newdialog;
        public ICommand OpenDialog
        {
            get
            {
                if (newdialog == null)
                    newdialog = new DelegateCommand(CanExecute, ExecOpenDialog);
                return newdialog;
            }
        }

        private void ExecOpenDialog(object parameter)
        {
            ExecuteUpdateActivities();

            IMessageBoxService msgbox = new MessageBoxService();
            bool result = msgbox.OpenDialog((string) parameter);
            
            if(((string)parameter) == "MaintenanceDue")            
                LoadToDoList();                
            
            msgbox = null;
        }
               
        ICommand close;
        public ICommand CloseDown
        {
            get
            {
                if (close == null)
                    close = new DelegateCommand(CanExecute, ExecuteClose);
                return close;
            }
        }

        private void ExecuteClose(object parameter)
        {
            ExecuteUpdateActivities();
            CloseWindow();
        }


        private bool CanExecuteSave(object obj)
        {
            return IsDirtyData;
        }

        ICommand saveall;
        public ICommand SaveAll
        {
            get
            {
                if (saveall == null)
                    saveall = new DelegateCommand(CanExecuteSave, ExecuteSaveAll);
                return saveall;
            }
        }

        private void ExecuteSaveAll(object parameter)
        {
            ExecuteUpdateActivities();
        }

        ICommand newproject;
        public ICommand NewProject
        {
            get
            {
                if (newproject == null)
                    newproject = new DelegateCommand(CanExecute, OpenNewProject);
                return newproject;
            }
        }

        private void OpenNewProject(object parameter)
        {
            try
            {
                ExecuteUpdateActivities();

                IMessageBoxService msgbox = new MessageBoxService();
                if (msgbox.OpenProjectDlg((Window)parameter, 0))
                {
                    FilterData();
                    SetPopupFilters(excludedcols, UserProjects);
                    ApplyPopupFilter();
                }
                msgbox = null;
            }
            catch { }
        }

        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (IsDirtyData)
            {
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                    ExecuteUpdateActivities();

                return true;
            }
            else
                return true;
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExecuteClosing);
                return windowclosing;
            }
        }

        private void ExecuteClosing(object parameter)
        {

        }


        ICommand windowcancelclosing;
        private bool CanCancelCloseWindow(object obj)
        {
            return true;
        }

        public ICommand CancelClosingCommand
        {
            get
            {
                if (windowcancelclosing == null)
                    windowcancelclosing = new DelegateCommand(CanCancelCloseWindow, CancelWindowClosing);

                return windowcancelclosing;
            }
        }

        private void CancelWindowClosing(object parameter)
        {

        }

        #endregion

        #region Popup Filter Commands

        private void ExecuteClearFilterPopup(object parameter)
        {
            try
            {
                FilterPopupModel s = new FilterPopupModel();
                bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                if (success)
                {
                    foreach (FilterPopupDataModel fm in s.FilterData)                    
                        fm.IsChecked = false;
                    
                    s.IsApplied = true;
                    ApplyPopupFilter();
                }
            }
            catch
            {
            }
        }

        private void ExecuteResetFilterPopup(object parameter)
        {
            try
            {
                FilterPopupModel s = new FilterPopupModel();
                bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                if (success)
                {
                    foreach (FilterPopupDataModel fm in s.FilterData)
                        fm.IsChecked = true;

                    s.IsApplied = false;
                    ApplyPopupFilter();
                }
            }
            catch
            {
            }
        }
                
        private void ExecuteApplyFilterPopup(object parameter)
        {
            try
            {
                FilterPopupModel s = new FilterPopupModel();
                bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                if (success)
                {
                    s.IsApplied = false;
                    foreach (FilterPopupDataModel f in s.FilterData)
                    {
                        if (!f.IsChecked)
                        {
                            s.IsApplied = true;
                            break;
                        }
                    }
                    ApplyPopupFilter();
                  
                    ExecuteUpdateActivities();
                    ExecuteClearActivities();
                    ExportIsEnabled = false;
                }
            }
            catch
            {
            }
        }
              
        private void ExecuteClearDataFilters(object parameter)
        {
            foreach (KeyValuePair<string, FilterPopupModel> fd in DictFilterPopup)
            {                
                FilterPopupModel fg = fd.Value;
                fg.IsApplied = false;
                foreach (FilterPopupDataModel fdata in fg.FilterData)
                    fdata.IsChecked = true;                                
            }
            ApplyPopupFilter();
        }
        
        private void ApplyPopupFilter()
        {
            try
            {
                ExecuteUpdateActivities();
                //if (PopupFilterDictContains(Constants.MainviewPopupList, DictFilterPopup))
                //{                                       
                    UserProjects = DynamicFilter.FilterDataTable(masterprojectlist, Constants.MainviewPopupList, DictFilterPopup);
                //}
                //else
                //    UserProjects = masterprojectlist;
            }
            catch
            {              
            }
            GetSumSales();
        }

        private void OLDApplyPopupFilter()
        {
            try
            {
                ExecuteUpdateActivities();

                if (PopupFilterDictContains(Constants.MainviewPopupList, DictFilterPopup))
                {
                    Dictionary<string, List<string>> locdictFilterPopup = new Dictionary<string, List<string>>();
                    foreach (string s in Constants.MainviewPopupList)
                        locdictFilterPopup.Add(s, DictFilterPopup[s].FilterData.Where(y => y.IsChecked == true).Select(x => x.Description).ToList<string>());

                    var c1 = masterprojectlist.AsEnumerable()
                    .Where(row => locdictFilterPopup["SalesDivision"].Contains(row["SalesDivision"].ToString())
                       && locdictFilterPopup["KPM"].Contains(row["KPM"].ToString())
                       && locdictFilterPopup["Customer"].Contains(row["Customer"].ToString())
                       && locdictFilterPopup["ProjectStatus"].Contains(row["ProjectStatus"].ToString())
                       && locdictFilterPopup["ProjectType"].Contains(row["ProjectType"].ToString())
                    );

                    if (c1.Count() > 0)
                    {
                        DataTable tblFiltered = c1.CopyToDataTable();
                        ReFormatColumns(ref masterprojectlist, ref tblFiltered);
                        UserProjects = tblFiltered;
                    }
                    else
                        UserProjects = masterprojectlist.Clone();
                }
                else
                    UserProjects = masterprojectlist;
            }
            catch
            {
            }
            GetSumSales();
        }



        private void InitializePopupFilters()
        {
            try
            {
                foreach (string colname in Constants.MainviewPopupList)
                {

                    if (!DictFilterPopup.ContainsKey(colname))
                        DictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = UserProjects.Columns[colname].Caption, IsApplied = false });

                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(colname, out s);

                    if (colname == "ProjectStatus")
                    {
                        foreach (EnumValue ev in EnumerationLists.ProjectStatusTypesList)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Description, IsChecked = (ev.Description == EnumerationManager.GetEnumDescription(ProjectStatusType.Active)) });

                        s.IsApplied = true;
                    }
                    else
                    if (colname == "SalesDivision")
                    {
                        foreach (ModelBaseVM ev in BusinessUnits)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = (ev.Name == "PT") });
                        s.IsApplied = true;
                    }
                    else
                    if (colname == "ProjectType")
                    {
                        foreach (ProjectTypeModel ev in ProjectTypes)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "KPM")
                    {
                        s.FilterData.Add(new FilterPopupDataModel() { Description = "Yes", IsChecked = true });
                        s.FilterData.Add(new FilterPopupDataModel() { Description = "No", IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "Customer")
                    {
                        foreach (DataRow dr in UserProjects.Rows)
                            if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
                                s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });
                        s.IsApplied = false;
                    }
                }
            }
            catch
            {
            }
        }
                    
        #endregion
       
      
        #region Export to XL Command
      
        private void ExecuteExportToExcel(object parameter)
        {
            try
            {
                if (SelectedProject != null)
                {
                    ExcelLib xl = new ExcelLib();
                    xl.MakeProjectReport((Window)parameter, GetProjectReport(ConvertObjToInt(SelectedProject["ProjectID"])));
                    xl = null;
                }
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report", "Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        #endregion
                      
    }
   
}
