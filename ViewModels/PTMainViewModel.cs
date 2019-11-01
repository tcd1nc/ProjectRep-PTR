using System;
using System.Windows.Input;
using static PTR.StaticCollections;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows;
using System.Collections.Generic;
using System.Linq;

namespace PTR.ViewModels
{
    public class PTMainViewModel : FilterModule
    {
        FullyObservableCollection<ProjectReportSummary> associatesprojects = new FullyObservableCollection<ProjectReportSummary>();
       
        string title = "Project Tracker";

        public PTMainViewModel()
        {
            try {
                ExecuteApplyModuleFilter = ExecuteApplyFilter;
                ExecuteFMOpenProject = ExecuteOpenProject;

                Title = (CurrentUser.Administrator) ? title + " - " + CurrentUser.GOM.Name + " (Administrator)" : title + " - " + CurrentUser.GOM.Name;

                SetAccess();
                SetControlState();
                FilterData();
                
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

        public FullyObservableCollection<ProjectReportSummary> AssociatesProjects
        {
            get { return associatesprojects; }
            set { SetField(ref associatesprojects, value); }
        }

        decimal sumsalesUSD;
        public decimal SumSalesUSD
        {
            get { return sumsalesUSD; }
            set { SetField(ref sumsalesUSD, value); }
        }   

        ProjectReportSummary obj;
        public ProjectReportSummary SelectedProject
        {
            get { return obj;}
            set {
                if (value != null)
                {                   
                    SetField(ref obj, value);
                    ExportIsEnabled = true;
                }
            }
        }

        bool exportisenabled = false;
        public bool ExportIsEnabled
        {
            get { return exportisenabled; }
            set { SetField(ref exportisenabled, value); }
        }

        FullyObservableCollection<ClassTreeItem> todolist;
        public FullyObservableCollection<ClassTreeItem> ToDoList
        {
            get { return todolist; }
            set { SetField(ref todolist, value); }
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
      
        private void ExecuteClearActivities()
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
        
        private void ExecuteUpdateActivities()
        {
            UpdateActivities = true;
            UpdateActivities = false;
        }

        #endregion

        #region Private functions for Properties

        public void ShowReminderScreen()
        {
            LoadToDoList();
            if (CurrentUser.ShowNagScreen)
            {
                IMessageBoxService msgbox = new MessageBoxService();
                bool result = msgbox.OpenReminderDlg(this);
                msgbox = null;
            }
           
        }             

        private void GetSumSales()
        {
            decimal sumUSD = 0;
            if (AssociatesProjects != null && AssociatesProjects.Count > 0)
            {
                foreach (ProjectReportSummary row in AssociatesProjects)
                    sumUSD = sumUSD + row.EstimatedAnnualSales;// * row.ExRate;
             
                SumSalesUSD = sumUSD;
            }
        }
        
        private void LoadToDoList()
        {
            CheckActions(GetOverdueMonthlyUpdates(), GetProjectsRequiringCompletion(), GetIncompleteEPs(), GetMissingEPs(), GetOverdueMilestones());

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
                        Selected = false,
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
            int accessid = 0;
            foreach (ProjectReportSummary ps in AssociatesProjects)
            {
                accessid = StaticCollections.GetUserCustomerAccess(ps.CustomerID);
                if (accessid == (int)UserPermissionsType.FullAccess)
                {
                    foreach(MaintenanceModel mm in overdueactivities)
                        if(mm.ProjectID == ps.ID)
                            ps.OverdueActivity = true;
                    foreach (MaintenanceModel rc in requiringcompletion)
                        if (rc.ProjectID == ps.ID)
                            ps.RequiringCompletion = true;
                    foreach (MaintenanceModel ieps in incompleteeps)
                        if (ieps.ProjectID == ps.ID)
                            ps.IncompleteEP = true;
                    foreach (MaintenanceModel meps in missingeps)
                        if (meps.ProjectID == ps.ID)
                            ps.EPRequired = true;
                    foreach (MaintenanceModel md in milestonesdue)
                        if (md.ProjectID == ps.ID)
                            ps.MilestoneDue = true;

                }
            }            
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

        bool enablesavebtn;
        public bool EnableSaveBtn
        {
            get { return enablesavebtn; }
            set { SetField(ref enablesavebtn, value); }
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
               
        private void SetControlState()
        {
            EnableProjectList = true;
            EnableActivities = true;
            EnableFilters = true;           
            EnableSaveBtn = true;
            EnableNewProjectBtn = true;           
        }

        //private void UpdateAccess(int countryid, string projectStatus)
        //{           
        //    if (CheckUserCountryAccess(countryid, UserPermissionsType.FullAccess))
        //    {
        //        EnableSaveBtn = (projectStatus == EnumerationManager.GetEnumDescription(ProjectStatusType.Active));
        //        EnableActivities = true;
        //    }
        //    else
        //        if (CheckUserCountryAccess(countryid, UserPermissionsType.ReadOnly))
        //    {
        //        EnableSaveBtn = false;
        //        EnableActivities = false;
        //    }
        //    else
        //    {
        //        EnableSaveBtn = false;
        //        EnableActivities = false;
        //    }
        //}

        private void SetAccess()
        {            
            List<string> adminmnu = CurrentUser.AdministrationMnu.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            ShowCountryMnu = adminmnu.Contains(((int)AdministrationMenuOptions.CountryMnu).ToString());
            ShowCustomerMnu = adminmnu.Contains(((int)AdministrationMenuOptions.CustomerMnu).ToString());
            ShowExchangeRateMnu = adminmnu.Contains(((int)AdministrationMenuOptions.ExchangeRateMnu).ToString());
            ShowUserMnu = adminmnu.Contains(((int)AdministrationMenuOptions.UserMnu).ToString());
            ShowSalesRegionMnu = adminmnu.Contains(((int)AdministrationMenuOptions.SalesRegionMnu).ToString());
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
            ShowReportsMnu = (reportsmnu.Count > 0);
            
            //if any country allows create own or other project then enable New Project button                     

            if (CheckUserAccess(UserPermissionsType.FullAccess))
            {
                EnableSaveBtn = true;
                EnableNewProjectBtn = true;                           
            }

            ShowNewProject = true;
            ShowSave = true;
        }

        #endregion
                            
        #region Commands
  
        private bool caneditmonthlydata = false;
                       
        private void ExecuteOpenProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    ExecuteUpdateActivities();
                                       
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedProject.ID;
                    bool result = msgbox.OpenProjectDlg((Window)parameter,id);
                    //if return value is true then Refresh list
                    if (result == true)
                    {
                        FilterData();
                    }
                    LoadToDoList();
                    //LoadTodoListTree();
                    
                    msgbox = null;
                }
            }
            catch { }
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
            {
                LoadToDoList();
                //LoadTodoListTree();
            }


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

        ICommand saveall;
        public ICommand SaveAll
        {
            get
            {
                if (saveall == null)
                    saveall = new DelegateCommand(CanExecute, ExecuteSaveAll);
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
                bool result = msgbox.OpenProjectDlg((Window)parameter, 0);
                if (result == true)
                {
                    FilterData();
                    LoadToDoList();
                    //LoadTodoListTree();
                }

                msgbox = null;
            }
            catch { }
        }
        


        #region Treeview Commands

        ICommand tvproject;
        public ICommand OpenTVProject
        {
            get
            {
                if (tvproject == null)
                    tvproject = new DelegateCommand(CanExecute, ExecuteOpenTVProject);
                return tvproject;
            }
        }

        private void ExecuteOpenTVProject(object parameter)
        {
            try
            {               
                if (parameter.GetType().Equals(typeof(MaintenanceModel)))
                {
                    int id = ((MaintenanceModel)parameter).ID;
                    int projectid = ((MaintenanceModel)parameter).ProjectID;
                    bool result = false;
                    IMessageBoxService msgbox = new MessageBoxService();
                    if (((MaintenanceModel)parameter).MaintenanceTypeID == MaintenanceType.MissingEP)
                    {
                        result = msgbox.OpenProjectDlg(null, projectid);
                    }
                    else
                    if (((MaintenanceModel)parameter).MaintenanceTypeID == MaintenanceType.IncompleteEP)
                    {
                        result = msgbox.EvaluationPlanDialog(null,id, projectid);
                    }
                    else
                    if (((MaintenanceModel)parameter).MaintenanceTypeID == MaintenanceType.RequiringCompletion)
                    {
                        result = msgbox.OpenProjectDlg(null, projectid);
                    }
                    else
                    if (((MaintenanceModel)parameter).MaintenanceTypeID == MaintenanceType.MilestoneDue)
                    {
                        result = msgbox.MilestoneDialog(null, id, projectid);
                    }
                    else
                    if (((MaintenanceModel)parameter).MaintenanceTypeID == MaintenanceType.OverdueActivity)
                    {
                        result = msgbox.OpenProjectCommentsDlg(null, projectid);
                    }
                    
                    //if return value is true then Refresh list
                    if (result == true)
                    {
                        ExecuteUpdateActivities();

                        FilterData();
                        LoadToDoList();
                        //LoadTodoListTree();
                    }
                    msgbox = null;
                }
            }
            catch { }
        }

        #endregion

        #region Refresh To Do List

        ICommand refreshtodolist;
        public ICommand RefreshToDoList
        {
            get
            {
                if (refreshtodolist == null)
                    refreshtodolist = new DelegateCommand(CanExecute, ExecuteRefreshToDoList);
                return refreshtodolist;
            }
        }

        private void ExecuteRefreshToDoList(object parameter)
        {
            try
            {                
                LoadToDoList();
                //LoadTodoListTree();                 
            }
            catch { }
        }

        #endregion

        #region Export to XL

        public bool CanExecuteExportToExcel(object parameter)
        {
            if (SelectedProject != null)
                return true;
            else
                return false;
        }
        
        ICommand exporttoexcel;
        public ICommand ExportToExcel
        {
            get
            {
                if (exporttoexcel == null)
                    exporttoexcel = new DelegateCommand(CanExecuteExportToExcel, ExecuteExportToExcel);
                return exporttoexcel;
            }
        }

        private void ExecuteExportToExcel(object parameter)
        {
            try
            {
                if (SelectedProject != null)
                {
                    ExcelLib xl = new ExcelLib();
                    xl.MakeProjectReport(GetProjectReport(SelectedProject.ID));
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

        #endregion

        #region Filter Commands

        private void ExecuteApplyFilter(object parameter)
        {
            ExecuteUpdateActivities();
            ExecuteClearActivities();

            FilterData();
            ExportIsEnabled = false;
        }

        private void FilterData()
        {
            ExecuteClearActivities();

            if (string.IsNullOrEmpty(AssociatesSrchString) 
                || string.IsNullOrEmpty(ProjectStatusTypesSrchString) 
                || string.IsNullOrEmpty(ProjectTypesSrchString) 
                || string.IsNullOrEmpty(CountriesSrchString))
                AssociatesProjects?.Clear();
            else
                AssociatesProjects = GetProjectList(CountriesSrchString, ProjectTypesSrchString, AssociatesSrchString, ProjectStatusTypesSrchString, SelectedDivisionID);
            GetSumSales();


            LoadToDoList();

        }     

        #endregion        

    }
   
}
