using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PTR.DatabaseQueries;
using static PTR.StaticCollections;
using PTR.Models;

namespace PTR.ViewModels
{
    public class PlaybookViewModel : ReportsFilterModule, IDisposable
    {
        DataTable salesfunnel = new DataTable();
        DataTable newbusiness = new DataTable();
        DateTime? startmonthprojects;
        DateTime? firstmonthcomments;
        DateTime? lastmonthcomments;
        private readonly string[] excludedcols = { "Customer", "UserName" };
        //private readonly string[] excludednewbusinesscols = { "Customer", "UserName" };

        public PlaybookViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            ExecuteFMOpenProject = ExecuteOpenProject;
            ExecuteClearPopupFilter = ExecuteClearFilterPopup;
            ExecuteResetPopupFilter = ExecuteResetFilterPopup;
            ExecuteApplyPopupFilter = ExecuteApplyFilterPopup;
            ExecuteClearFilters = ExecuteClearDataFilters;

            firstmonthcomments = new DateTime(DateTime.Now.Year - 1, 12, 1);
            lastmonthcomments = new DateTime(DateTime.Now.Year, 12, 1);
            startmonthprojects = Config.DefaultMasterListStartMonth;// GetStartMonth();
           
            FilterData();
            InitializePopupFilters();
            ApplyPopupFilter();

            //InitializeNewBusinessPopupFilters();
            //ApplyNewBusinessPopupFilter();


        }
                              
        #region Properties

        DataTable tempsalesfunnel;
        //DataTable tempnewbusiness;

        public DataTable SalesFunnel
        {
            get { return salesfunnel; }
            set { SetField(ref salesfunnel, value); }
        }

        //public DataTable NewBusiness
        //{
        //    get { return newbusiness; }
        //    set { SetField(ref newbusiness, value); }
        //}

        bool salesfunnelselected = true;
        public bool SalesFunnelSelected
        {
            get { return salesfunnelselected; }
            set
            {
                SetField(ref salesfunnelselected, value);
                if(SelectedProject == null)                
                    EditDetailVis = Visibility.Hidden;                
            }
        }

        DataRowView selectedproject;
        public DataRowView SelectedProject
        {
            get { return selectedproject; }
            set {
                if (value != null)
                    EditDetailVis = Visibility.Visible;
                else
                    EditDetailVis = Visibility.Hidden;
                SetField(ref selectedproject, value); }
        }

        //bool newbusinessselected =false;
        //public bool NewBusinessSelected
        //{
        //    get { return newbusinessselected; }
        //    set {
        //        SetField(ref newbusinessselected, value);
        //        if (SelectedNewBusProject == null)                
        //            EditDetailVis = Visibility.Hidden;                
        //    }
        //}

        //DataRowView selectednewproject;
        //public DataRowView SelectedNewBusProject
        //{
        //    get { return selectednewproject; }
        //    set
        //    {
        //        if (value != null)
        //            EditDetailVis = Visibility.Visible;
        //        else
        //            EditDetailVis = Visibility.Hidden;
        //        SetField(ref selectednewproject, value);
        //    }
        //}

        Visibility editprojectvis = Visibility.Hidden;
        public Visibility EditDetailVis
        {
            get { return editprojectvis; }
            set { SetField(ref editprojectvis, value); }
        }

        private DateTime GetStartMonth()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        public DateTime? StartMonthProjects
        {
            get { return startmonthprojects; }
            set { SetField(ref startmonthprojects, value); }
        }

        public DateTime? FirstMonthComments
        {
            get { return firstmonthcomments; }
            set { SetField(ref firstmonthcomments, value); }
        }

        public DateTime? LastMonthComments
        {
            get { return lastmonthcomments; }
            set { SetField(ref lastmonthcomments, value); }
        }

        #endregion

        #region Commands

        private void ExecuteOpenProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {                  
                    object[] values = new object[2];
                    values = parameter as object[];
                    int id = (int)values[0];
                    if (id > 0)
                    { 
                        IMessageBoxService msgbox = new MessageBoxService();                       
                        //if return value is true then Refresh list
                        if (msgbox.OpenProjectDlg((Window)values[1], id))
                        {
                            FilterData();
                            //if (SalesFunnelSelected)
                            //{
                                SetPopupFilters(excludedcols, SalesFunnel);
                                ApplyPopupFilter();
                            //}
                            //else
                            //{
                            //    SetNewBusinessPopupFilters(excludednewbusinesscols, NewBusiness);
                            //    ApplyNewBusinessPopupFilter();
                            //}
                        }                        
                        msgbox = null;
                    }
                }
            }
            catch { }
        }


        ICommand openprojectdetails;
        public ICommand OpenProjectDetails
        {
            get
            {
                if (openprojectdetails == null)
                    openprojectdetails = new DelegateCommand(CanExecuteOpenProjectDetails, ExecuteOpenProjectDetails);
                return openprojectdetails;
            }
        }

        public bool CanExecuteOpenProjectDetails(object parameter)
        {
            return EditDetailVis==Visibility.Visible;
        }
        
        private void ExecuteOpenProjectDetails(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    int id = 0;
                    //if(SalesFunnelSelected)
                        id = (int)SelectedProject["ProjectID"];
                    //else
                    //    id = (int)SelectedNewBusProject["ProjectID"];

                    if (id > 0)
                    {
                        IMessageBoxService msgbox = new MessageBoxService();
                        //if return value is true then Refresh list
                        if (msgbox.OpenProjectDlg((Window)parameter, id))
                        {
                            FilterData();
                            //if (SalesFunnelSelected)
                            //{
                                SetPopupFilters(excludedcols, SalesFunnel);
                                ApplyPopupFilter();
                            //}
                            //else
                            //{
                            //    SetNewBusinessPopupFilters(excludednewbusinesscols, NewBusiness);
                            //    ApplyNewBusinessPopupFilter();
                            //}

                        }                            
                        msgbox = null;
                    }
                }
            }
            catch { }
        }
        
        public bool CanExecuteOpenComments(object parameter)
        {
            return canopenprojectcomments;
        }

        ICommand opencomments;
        public ICommand OpenComments
        {
            get
            {
                if (opencomments == null)
                    opencomments = new DelegateCommand(CanExecuteOpenComments, ExecuteOpenComments);
                return opencomments;
            }
        }

        private readonly bool canopenprojectcomments = true;
        private void ExecuteOpenComments(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    int id = 0;
                    //if (SalesFunnelSelected)
                        id = (int)SelectedProject["ProjectID"];
                    //else
                    //    id = (int)SelectedNewBusProject["ProjectID"];
                    if (id > 0)
                    {
                        IMessageBoxService msgbox = new MessageBoxService();
                        //if return value is true then Refresh list
                        if (msgbox.OpenProjectCommentsDlg((Window)parameter, id))
                        {
                            FilterData();
                            //if (SalesFunnelSelected)
                            //{
                                SetPopupFilters(excludedcols, SalesFunnel);
                                ApplyPopupFilter();
                            //}
                            //else
                            //{
                            //    SetNewBusinessPopupFilters(excludednewbusinesscols, NewBusiness);
                            //    ApplyNewBusinessPopupFilter();
                            //}
                        }
                        msgbox = null;
                    }
                }
            }
            catch
            {
            }
        }

        ICommand exportpb;
        public ICommand ExportPlaybook
        {
            get
            {
                if (exportpb == null)
                    exportpb = new DelegateCommand(CanExecute, ExecuteExportPB);
                return exportpb;
            }
        }

        private void ExecuteExportPB(object parameter)
        {
            try
            {
                DataTable dt = SalesFunnel.Copy();
                foreach (DataColumn dc in SalesFunnel.Columns)                   
                    if ((int)dc.ExtendedProperties["FieldType"] == (int)ReportFieldType.General)
                        dt.Columns.Remove(dc.ColumnName);

                //DataTable dtnb = NewBusiness.Copy();
                //foreach (DataColumn dc in NewBusiness.Columns)                   
                //    if ((int)dc.ExtendedProperties["FieldType"] == (int)ReportFieldType.General)
                //        dtnb.Columns.Remove(dc.ColumnName);

                ExcelLib xl = new ExcelLib();
                //xl.MakePlaybookReport((Window)parameter, dt, dtnb, Config.ColourisePlaybookReport);
                xl.MakePlaybookReport((Window)parameter, dt, Config.ColourisePlaybookReport);
                xl = null;
                dt.Dispose();
                dt = null;
                //dtnb.Dispose();
                //dtnb = null;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report", "Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        ICommand exportall;
        public ICommand ExportAll
        {
            get
            {
                if (exportall == null)
                    exportall = new DelegateCommand(CanExecute, ExecuteExportAll);
                return exportall;
            }
        }

        private void ExecuteExportAll(object parameter)
        {
            try
            {                             
                ExcelLib xl = new ExcelLib();
                //xl.MakeMasterReport((Window)parameter, SalesFunnel, NewBusiness, true);
                xl.MakeMasterReport((Window)parameter, SalesFunnel, true);
                xl = null;                
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report", "Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        private void ExecuteClearFilterPopup(object parameter)
        {
            try
            {
                //if (SalesFunnelSelected)
                //{
                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                    if (success)
                    {
                        foreach (FilterPopupDataModel fm in s.FilterData)
                            fm.IsChecked = false;

                        s.IsApplied = true;
                        ApplyPopupFilter();
                    }
                //}
                //else
                //{
                //    FilterPopupModel s = new FilterPopupModel();
                //    bool success = NewBusinessDictFilterPopup.TryGetValue(parameter.ToString(), out s);
                //    if (success)
                //    {
                //        foreach (FilterPopupDataModel fm in s.FilterData)
                //            fm.IsChecked = false;

                //        s.IsApplied = true;
                //        ApplyNewBusinessPopupFilter();
                //    }
                //}
            }
            catch
            {
            }
        }

        private void ExecuteResetFilterPopup(object parameter)
        {
            try
            {
                //if (SalesFunnelSelected)
                //{
                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                    if (success)
                    {
                        foreach (FilterPopupDataModel fm in s.FilterData)
                            fm.IsChecked = true;

                        s.IsApplied = false;
                        ApplyPopupFilter();
                    }
                //}
                //else
                //{
                //    FilterPopupModel s = new FilterPopupModel();
                //    bool success = NewBusinessDictFilterPopup.TryGetValue(parameter.ToString(), out s);
                //    if (success)
                //    {
                //        foreach (FilterPopupDataModel fm in s.FilterData)
                //            fm.IsChecked = true;

                //        s.IsApplied = false;
                //        ApplyNewBusinessPopupFilter();
                //    }
                //}
            }
            catch
            {
            }
        }

        private void ExecuteApplyFilterPopup(object parameter)
        {
            try
            {
                //if (SalesFunnelSelected)
                //{
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
                    }
                //}
                //else
                //{
                //    FilterPopupModel s = new FilterPopupModel();
                //    bool success = NewBusinessDictFilterPopup.TryGetValue(parameter.ToString(), out s);
                //    if (success)
                //    {
                //        s.IsApplied = false;
                //        foreach (FilterPopupDataModel f in s.FilterData)
                //        {
                //            if (!f.IsChecked)
                //            {
                //                s.IsApplied = true;
                //                break;
                //            }
                //        }
                //        ApplyNewBusinessPopupFilter();
                //    }
                //}
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

            //foreach (KeyValuePair<string, FilterPopupModel> fd2 in NewBusinessDictFilterPopup)
            //{
            //    FilterPopupModel fg2 = fd2.Value;
            //    fg2.IsApplied = false;
            //    foreach (FilterPopupDataModel fdata2 in fg2.FilterData)
            //        fdata2.IsChecked = true;
            //}
            //ApplyNewBusinessPopupFilter();

        }

        private void ApplyPopupFilter()
        {
            try
            {
                //if (PopupFilterDictContains(Constants.MasterProjectsPopupList, DictFilterPopup))
                //{
                //    Dictionary<string, List<string>> locdictFilterPopup = new Dictionary<string, List<string>>();
                //    foreach(string s in Constants.MasterProjectsPopupList)                    
                //        locdictFilterPopup.Add(s, DictFilterPopup[s].FilterData.Where(y => y.IsChecked == true).Select(x => x.Description).ToList<string>());
                                       
                //    var c1 = tempsalesfunnel.AsEnumerable()
                //    .Where(row => locdictFilterPopup["SalesDivision"].Contains(row["SalesDivision"].ToString())
                //        && locdictFilterPopup["IndustrySegment"].Contains(row["IndustrySegment"].ToString())
                //        && locdictFilterPopup["KPM"].Contains(row["KPM"].ToString())
                //        && locdictFilterPopup["Customer"].Contains(row["Customer"].ToString())
                //        && locdictFilterPopup["UserName"].Contains(row["UserName"].ToString())
                //        && locdictFilterPopup["DifferentiatedTechnology"].Contains(row["DifferentiatedTechnology"].ToString())
                //        && locdictFilterPopup["Application"].Contains(row["Application"].ToString())
                //        && locdictFilterPopup["ProjectStatus"].Contains(row["ProjectStatus"].ToString())
                //        && locdictFilterPopup["ProjectType"].Contains(row["ProjectType"].ToString())
                //        && locdictFilterPopup["SMCode"].Contains(row["SMCode"].ToString())
                //        && locdictFilterPopup["SalesFunnelStage"].Contains(row["SalesFunnelStage"].ToString())
                //    );
                                      
                //    if (c1.Count() > 0)
                //    {                                                                                                         
                //        DataTable tblFiltered = c1.CopyToDataTable();                        
                //        ReFormatColumns(ref tempsalesfunnel, ref tblFiltered);
                //        SalesFunnel = tblFiltered;
                //    }
                //    else
                //        SalesFunnel = tempsalesfunnel.Clone();
                //}
                //else
                //    SalesFunnel = tempsalesfunnel;

                SalesFunnel = DynamicFilter.FilterDataTable(tempsalesfunnel, Constants.MasterProjectsPopupList, DictFilterPopup);
            }
            catch
            {
            }            
        }

        //Dictionary<string, FilterPopupModel> newbusinessdictFilterPopup = new Dictionary<string, FilterPopupModel>();
        //public Dictionary<string, FilterPopupModel> NewBusinessDictFilterPopup
        //{
        //    get { return newbusinessdictFilterPopup; }
        //    set { SetField(ref newbusinessdictFilterPopup, value); }
        //}

        //public void SetNewBusinessPopupFilters(string[] excludedcols, DataTable dt)
        //{
        //    try
        //    {
        //        foreach (string colname in excludedcols)
        //        {
        //            if (!NewBusinessDictFilterPopup.ContainsKey(colname))
        //                NewBusinessDictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = dt.Columns[colname].Caption, IsApplied = false });

        //            FilterPopupModel s = new FilterPopupModel();
        //            bool success = NewBusinessDictFilterPopup.TryGetValue(colname, out s);

        //            foreach (DataRow dr in dt.Rows)
        //                if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0)
        //                    s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });
        //            s.IsApplied = false;
        //        }
        //    }
        //    catch { }
        //}


        //private void ApplyNewBusinessPopupFilter()
        //{
        //    try
        //    {
        //        //if (PopupFilterDictContains(Constants.NewBusinessProjectsPopupList, NewBusinessDictFilterPopup))
        //        //{
        //        //    Dictionary<string, List<string>> locdictFilterPopup = new Dictionary<string, List<string>>();
        //        //    foreach (string s in Constants.NewBusinessProjectsPopupList)
        //        //        locdictFilterPopup.Add(s, NewBusinessDictFilterPopup[s].FilterData.Where(y => y.IsChecked == true).Select(x => x.Description).ToList<string>());

        //        //    var c1 = tempnewbusiness.AsEnumerable()
        //        //    .Where(row => locdictFilterPopup["SalesDivision"].Contains(row["SalesDivision"].ToString())
        //        //        && locdictFilterPopup["IndustrySegment"].Contains(row["IndustrySegment"].ToString())
        //        //    && locdictFilterPopup["KPM"].Contains(row["KPM"].ToString())
        //        //    && locdictFilterPopup["Customer"].Contains(row["Customer"].ToString())
        //        //    && locdictFilterPopup["UserName"].Contains(row["UserName"].ToString())
        //        //    && locdictFilterPopup["DifferentiatedTechnology"].Contains(row["DifferentiatedTechnology"].ToString())
        //        //    && locdictFilterPopup["Application"].Contains(row["Application"].ToString())
        //        //    && locdictFilterPopup["ProjectType"].Contains(row["ProjectType"].ToString())
        //        //    && locdictFilterPopup["SMCode"].Contains(row["SMCode"].ToString())
        //        //    );

        //        //    if (c1.Count() > 0)
        //        //    {
        //        //        DataTable tblFiltered = c1.CopyToDataTable();
        //        //        ReFormatColumns(ref tempnewbusiness, ref tblFiltered);
        //        //        NewBusiness = tblFiltered;
        //        //    }
        //        //    else
        //        //        NewBusiness = tempnewbusiness.Clone();
        //        //}
        //        //else
        //        //    NewBusiness = tempnewbusiness;

        //        NewBusiness = DynamicFilter.FilterDataTable(tempnewbusiness, Constants.NewBusinessProjectsPopupList, NewBusinessDictFilterPopup);
        //    }
        //    catch
        //    {
        //    }
        //}
        

        //------------------------------------------------------------------------------------------------------------

        private void ExecuteApplyFilter(object parameter)
        {
            FilterData();
            SetPopupFilters(excludedcols, SalesFunnel);
            ApplyPopupFilter();

            //SetNewBusinessPopupFilters(excludednewbusinesscols, NewBusiness);
            //ApplyNewBusinessPopupFilter();
        }

        private void InitializePopupFilters()
        {
            try
            {
                foreach (string colname in Constants.MasterProjectsPopupList)
                {
                    if (!DictFilterPopup.ContainsKey(colname))
                        DictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = SalesFunnel.Columns[colname].Caption, IsApplied = false });

                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(colname, out s);

                    if (colname == "SalesDivision")
                    {
                        foreach (ModelBaseVM ev in BusinessUnits)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = (ev.Name == "PT") });
                        s.IsApplied = true;
                    }
                    else
                    if (colname == "IndustrySegment")
                    {
                        FullyObservableCollection<IndustrySegmentModel> mktsegs = GetIndustrySegments();
                        foreach (IndustrySegmentModel ev in mktsegs)
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
                    if (colname == "DifferentiatedTechnology")
                    {
                        s.FilterData.Add(new FilterPopupDataModel() { Description = "Yes", IsChecked = true });
                        s.FilterData.Add(new FilterPopupDataModel() { Description = "No", IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "Application")
                    {
                        FullyObservableCollection<ApplicationModel> appcats = GetApplications();

                        foreach (ApplicationModel ev in appcats)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "ProjectStatus")
                    {
                        FullyObservableCollection<ModelBaseVM> salesstatuses = GetSalesStatuses();

                        foreach (ModelBaseVM ev in salesstatuses)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = Config.DefaultSalesStatuses.Contains(ev.ID.ToString()) ? true : false });
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
                    if (colname == "SMCode")
                    {
                        FullyObservableCollection<SMCodeModel> smcodes = GetSMCodes();

                        foreach (SMCodeModel ev in smcodes)
                            if (s.FilterData.Count(x => x.Description == ev.Name) == 0)
                                s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "SalesFunnelStage")
                    {
                        foreach (ActivityStatusCodesModel ev in ActivityStatusCodes)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.PlaybookDescription, IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "NewBusinessCategory")
                    {
                        FullyObservableCollection<ModelBaseVM> newbusinesscategories = GetNewBusinessCategories();
                        foreach (ModelBaseVM ev in newbusinesscategories)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
                        s.IsApplied = false;
                    }
                    //else
                    //if (colname == "Priority")
                    //{
                    //    FullyObservableCollection<ModelBaseVM> priorities = GetPriorities();

                    //    foreach (ModelBaseVM ev in priorities)
                    //        s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
                    //    s.IsApplied = false;
                    //}
                    else
                    {
                        if (colname == "Customer" || colname == "UserName")
                        {
                            foreach (DataRow dr in SalesFunnel.Rows)
                                if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
                                    s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });
                            s.IsApplied = false;
                        }
                    }
                }
            }
            catch {

            }
        }

        //private void InitializeNewBusinessPopupFilters()
        //{
        //    try
        //    {
        //        foreach (string colname in Constants.NewBusinessProjectsPopupList)
        //        {
        //            if (!NewBusinessDictFilterPopup.ContainsKey(colname))
        //                NewBusinessDictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = NewBusiness.Columns[colname].Caption, IsApplied = false });

        //            FilterPopupModel s = new FilterPopupModel();
        //            bool success = NewBusinessDictFilterPopup.TryGetValue(colname, out s);

        //            if (colname == "SalesDivision")
        //            {
        //                foreach (ModelBaseVM ev in BusinessUnits)
        //                    s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = (ev.Name == "PT") });
        //                s.IsApplied = true;
        //            }
        //            else
        //            if (colname == "IndustrySegment")
        //            {
        //                FullyObservableCollection<IndustrySegmentModel> mktsegs = GetIndustrySegments();
        //                foreach (IndustrySegmentModel ev in mktsegs)
        //                    s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
        //                s.IsApplied = false;
        //            }
        //            else
        //            if (colname == "KPM")
        //            {
        //                s.FilterData.Add(new FilterPopupDataModel() { Description = "Yes", IsChecked = true });
        //                s.FilterData.Add(new FilterPopupDataModel() { Description = "No", IsChecked = true });
        //                s.IsApplied = false;
        //            }
        //            else
        //            if (colname == "DifferentiatedTechnology")
        //            {
        //                s.FilterData.Add(new FilterPopupDataModel() { Description = "Yes", IsChecked = true });
        //                s.FilterData.Add(new FilterPopupDataModel() { Description = "No", IsChecked = true });
        //                s.IsApplied = false;
        //            }
        //            else
        //            if (colname == "Application")
        //            {
        //                FullyObservableCollection<ApplicationModel> appcats = GetApplications();

        //                foreach (ApplicationModel ev in appcats)
        //                    s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
        //                s.IsApplied = false;
        //            }
        //            else
        //            if (colname == "ProjectType")
        //            {
        //                foreach (ProjectTypeModel ev in ProjectTypes)
        //                    s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
        //                s.IsApplied = false;
        //            }
        //            else
        //            if (colname == "SMCode")
        //            {
        //                FullyObservableCollection<SMCodeModel> smcodes = GetSMCodes();

        //                foreach (SMCodeModel ev in smcodes)
        //                    if (s.FilterData.Count(x => x.Description == ev.Name) == 0)
        //                        s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
        //                s.IsApplied = false;
        //            }
        //            else
        //            {
        //                if (colname == "Customer" || colname == "UserName")
        //                {
        //                    foreach (DataRow dr in NewBusiness.Rows)
        //                        if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
        //                            s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });
        //                    s.IsApplied = false;
        //                }
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //}


        private void FilterData()
        {
            tempsalesfunnel = GetAllSalesFunnelReport(CountriesSrchString, (DateTime)StartMonthProjects, (DateTime)FirstMonthComments, (DateTime)LastMonthComments);
            //tempnewbusiness = GetNewBusinessReport(CountriesSrchString, (DateTime)StartMonthProjects);

            SalesFunnel = tempsalesfunnel;
            //NewBusiness = tempnewbusiness;

        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources  
                //if (newbusiness != null)
                //{
                //    newbusiness.Dispose();
                //    newbusiness = null;
                //}
                if (salesfunnel != null)
                {
                    salesfunnel.Dispose();
                    salesfunnel = null;
                }
            }
        }

        #endregion
    }
       

}
