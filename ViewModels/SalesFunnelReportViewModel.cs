using System;
using System.Data;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class SalesFunnelReportViewModel : FilterModule
    {
        DateTime? firstmonth;
        DateTime? lastmonth;

        Models.ProjectMonthRange projectdaterange;

        public SalesFunnelReportViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            projectdaterange = DatabaseQueries.GetFirstLastMonths();
            firstmonth = GetPreviousYearStartMonth();
            lastmonth = projectdaterange.LastDate;
            FilterData();           
        }

        #region Properties

        DataTable datatable;
        public DataTable Data
        {
            get { return datatable; }
            set { SetField(ref datatable, value); }
        }

        DataTable projectcountdatatable;
        public DataTable ProjectCount
        {
            get { return projectcountdatatable; }
            set { SetField(ref projectcountdatatable, value); }
        }

        DataTable projectsummarydatatable;
        public DataTable ProjectSummary
        {
            get { return projectsummarydatatable; }
            set { SetField(ref projectsummarydatatable, value); }
        }
        
        bool showtooltip;
        public bool ShowTooltip
        {
            get { return showtooltip; }
            set { SetField(ref showtooltip, value); }
        }

        string summarystring;
        public string SummaryString
        {
            get { return summarystring; }
            set { SetField(ref summarystring, value); }
        }

        string currentmonth;
        public string CurrentMonth
        {
            get { return currentmonth; }
            set { SetField(ref currentmonth, value); }
        }

        string currenstatus;
        public string CurrentStatus
        {
            get { return currenstatus; }
            set { SetField(ref currenstatus, value); }
        }
                
        public DateTime? FirstMonth
        {
            get { return firstmonth; }
            set { SetField(ref firstmonth, value); }
        }

        public DateTime? LastMonth
        {
            get { return lastmonth; }
            set { SetField(ref lastmonth, value); }
        }
                      
        bool useUSD = false;
        public bool UseUSD
        {
            get { return useUSD; }
            set { SetField(ref useUSD, value); }
        }

        #endregion

        #region Private functions for Properties

        private void GetSummaryString(int row, int col)
        {
            if (row >= 0 && col > -1)
            {
                CurrentMonth = ProjectSummary.Columns[col].Caption.ToString();            
                CurrentStatus = ProjectSummary.Rows[row].ItemArray[0].ToString();
                SummaryString = ProjectSummary.Rows[row].ItemArray[col].ToString();
                if (SummaryString.Length > 0)
                    ShowTooltip = true;
                else
                    ShowTooltip = false;
            }
            else
                ShowTooltip = false;
        }

        private DateTime GetPreviousYearStartMonth()
        {
            return new DateTime(DateTime.Now.Year - 1, 1, 1);
        }
                
        #endregion

        #region Filters

        ICommand exporttoexcel;
        public ICommand ExportToExcel
        {
            get
            {
                if (exporttoexcel == null)
                    exporttoexcel = new DelegateCommand(CanExecute, ExecuteExportToExcel);
                return exporttoexcel;
            }
        }

        private void ExecuteExportToExcel(object parameter)
        {
            try
            { 
                ExcelLib xl = new ExcelLib();
                xl.MakeSalesFunnelReport(datatable, projectcountdatatable);
                xl = null;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report","Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        private void ExecuteApplyFilter(object parameter)
        {
              FilterData();
        }
        
        private void FilterData()
        {            
            DataSet ds = DatabaseQueries.GetFilteredProjectsEstSales(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, UseUSD, (DateTime)firstmonth, (DateTime)lastmonth, ShowKPM(), ShowAllKPM(), GetCDPCCPList());
                                    
            Data = ds.Tables["TotalData"];
            ProjectCount = ds.Tables["ProjectCountData"];            
            ProjectSummary = DatabaseQueries.GetSummaryByStatusMonth(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, UseUSD, (DateTime)firstmonth, (DateTime)lastmonth, ShowKPM(), ShowAllKPM(), GetCDPCCPList());

        }

        ICommand mousemove;
        public ICommand MouseMove
        {
            get
            {
                if (mousemove == null)
                    mousemove = new DelegateCommand(CanExecute, ExecuteMouseMove);
                return mousemove;
            }
        }

        private void ExecuteMouseMove(object parameter)
        {
            try
            {
                char[] commaseparator = new char[] { ',' };

                string p = string.Empty;
                p = (string)parameter;
                string[] c = p.Split(commaseparator,StringSplitOptions.None);
                int row = -1;
                bool isrow = int.TryParse(c[0],out row);

                int col = -1;
                bool iscol = int.TryParse(c[1], out col);

                if(iscol && isrow)
                {
                    GetSummaryString(row, col);
                }
            }
            catch
            {
            }
        }

        #endregion

    }


    public class ProjectSummary
    {
        public string ProjectName
        {
            get;set;
        }
        public decimal TargetedSales  { get; set; }
    }


}
