using System;
using System.Data;
using System.Windows;
using System.Windows.Input;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class PlaybookViewModel2 : FilterModule, IDisposable
    {
        DataTable salesfunnel = new DataTable();
        DataTable newbusiness = new DataTable();
        DateTime? startmonthprojects;
        DateTime? firstmonthcomments;
        DateTime? lastmonthcomments;
               
        public PlaybookViewModel2()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            ExecuteFMOpenProject = ExecuteOpenProject;

            firstmonthcomments = new DateTime(DateTime.Now.Year-1, 12, 1);
            lastmonthcomments = new DateTime(DateTime.Now.Year, 12, 1);
            startmonthprojects = GetStartMonth();

            GetData();
        }

        #region Properties

        DataTable tempsalesfunnel;
        DataTable tempnewbusiness;

        public DataTable SalesFunnel
        {
            get { return salesfunnel; }
            set { SetField(ref salesfunnel, value); }
        }

        public DataTable NewBusiness
        {
            get { return newbusiness; }
            set { SetField(ref newbusiness, value); }
        }

        DataRowView selectedproject;
        public DataRowView SelectedProject
        {
            get { return selectedproject; }
            set {
                if (value != null)
                {
                    ProjectLabel = (int)value["ProjectID"];
                    EditDetailVis = Visibility.Visible;
                }
                else
                    EditDetailVis = Visibility.Hidden;
                SetField(ref selectedproject, value); }
        }


        DataRowView selectednewproject;
        public DataRowView SelectedNewProject
        {
            get { return selectednewproject; }
            set
            {
                if (value != null)
                {
                    ProjectLabel = (int)value["ProjectID"];
                    EditDetailVis = Visibility.Visible;
                }
                else
                    EditDetailVis = Visibility.Hidden;
                SetField(ref selectednewproject, value);
            }
        }


        int projectlabel;
        public int ProjectLabel
        {
            get { return projectlabel; }
            set { SetField(ref projectlabel, value); }
        }
        
        Visibility editprojectvis = Visibility.Hidden;
        public Visibility EditDetailVis
        {
            get { return editprojectvis; }
            set { SetField(ref editprojectvis, value); }
        }

        bool getmisccolumns = true;
        public bool GetMiscColumns
        {
            get { return getmisccolumns; }
            set { SetField(ref getmisccolumns, value);
                  FilterData();
            }
        }

        //bool kpm = true;
        //public bool KPM
        //{
        //    get { return kpm; }
        //    set { SetField(ref kpm, value); }
        //}

        //bool nonkpm = true;
        //public bool NonKPM
        //{
        //    get { return nonkpm; }
        //    set { SetField(ref nonkpm, value); }
        //}

        //bool cdp = true;
        //public bool CDP
        //{
        //    get { return cdp; }
        //    set { SetField(ref cdp, value); }
        //}

        //bool ccp = true;
        //public bool CCP
        //{
        //    get { return ccp; }
        //    set { SetField(ref ccp, value); }
        //}

        //bool allcdpccp = true;
        //public bool AllCDPCCP
        //{
        //    get { return allcdpccp; }
        //    set { SetField(ref allcdpccp, value); }
        //}



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
                    int id = ProjectLabel;
                    IMessageBoxService msgbox = new MessageBoxService();
                    bool result = msgbox.OpenProjectDlg((Window)parameter,id);
                    //if return value is true then Refresh list
                    if (result == true)
                        GetData();

                    msgbox = null;
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
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = ProjectLabel;
                    bool result = msgbox.OpenProjectCommentsDlg((Window)parameter, id);
                    //if return value is true then Refresh list
                    if (result == true)
                        GetData();

                    msgbox = null;
                }
            }
            catch { }
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
                DataTable dt = tempsalesfunnel.Copy();
                foreach (DataColumn dc in tempsalesfunnel.Columns)
                    if ((int)dc.ExtendedProperties["FieldType"] > 0 && (int)dc.ExtendedProperties["FieldType"] < 99)
                        dt.Columns.Remove(dc.ColumnName);

                ExcelLib xl = new ExcelLib();
                xl.MakeMasterProjectReport(dt);

                xl = null;
                dt.Dispose();
                dt = null;

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
                xl.MakeMasterReport(tempsalesfunnel, tempnewbusiness);
                xl = null;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report", "Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        private void ExecuteApplyFilter(object parameter)
        {
            GetData();           
        }

        private void GetData()
        {                    
            tempsalesfunnel = GetAllSalesFunnelReport(CountriesSrchString, SelectedDivisionID, ProjectTypesSrchString, (DateTime)StartMonthProjects, (DateTime)FirstMonthComments, (DateTime)LastMonthComments, GetMiscColumns, ShowKPM(), ShowAllKPM(), GetCDPCCPList());           
            tempnewbusiness = GetNewBusinessReport(CountriesSrchString, SelectedDivisionID, ProjectTypesSrchString, (DateTime)StartMonthProjects, (DateTime)FirstMonthComments, (DateTime)LastMonthComments, ShowKPM(), ShowAllKPM(), GetCDPCCPList());
            FilterData();
        }

                       
        private void FilterData()
        {
            DataTable dt = tempsalesfunnel.Copy();                        
            if (GetMiscColumns == false)
                foreach (DataColumn dc in tempsalesfunnel.Columns)
                    if ((int)dc.ExtendedProperties["FieldType"] == 1)
                        dt.Columns.Remove(dc.ColumnName);

            SalesFunnel = dt;
            
            DataTable dt2 = tempnewbusiness.Copy();
            if (GetMiscColumns == false)
                foreach (DataColumn dc in tempnewbusiness.Columns)
                if ((int)dc.ExtendedProperties["FieldType"] == 1)
                    dt2.Columns.Remove(dc.ColumnName);

            NewBusiness = dt2;
            //OnPropertyChanged("SalesFunnel");
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
                if (newbusiness != null)
                {
                    newbusiness.Dispose();
                    newbusiness = null;
                }
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
