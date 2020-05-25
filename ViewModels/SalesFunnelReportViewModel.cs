using System;
using System.Data;
using System.Windows.Input;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class SalesFunnelReportViewModel : FilterModule
    {
        DateTime? firstmonth;
        DateTime? lastmonth;

        public SalesFunnelReportViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            ExecuteRFExportToExcel = ExecuteExportToExcel;
            firstmonth = GetPreviousYearStartMonth();
            lastmonth =  new DateTime(DateTime.Now.Year, 12, 1);
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
                      
        bool useUSD = true;
        public bool UseUSD
        {
            get { return useUSD; }
            set { SetField(ref useUSD, value); }
        }


        string tblcaption;
        public string TblCaption
        {
            get { return tblcaption; }
            set { SetField(ref tblcaption, value); }
        }

        string counttblcaption;
        public string CountTblCaption
        {
            get { return counttblcaption; }
            set { SetField(ref counttblcaption, value); }
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

        private void ExecuteExportToExcel(object parameter)
        {
            try
            { 
                ExcelLib xl = new ExcelLib();
                xl.MakeSalesPipelineReport((System.Windows.Window)parameter, datatable, projectcountdatatable);
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
            DataSet ds = GetSalesPipelineReport(CountriesSrchString, BusinessUnitSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, UseUSD, (DateTime)firstmonth, (DateTime)lastmonth, ShowKPM(), ShowAllKPM());
            Data = ds.Tables[Constants.SalesPipeline];
            ProjectCount = ds.Tables[Constants.SalesPipelineCount];            
            ProjectSummary = GetSalesPipelineReportToolTip(CountriesSrchString, BusinessUnitSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, UseUSD, (DateTime)firstmonth, (DateTime)lastmonth, ShowKPM(), ShowAllKPM());

            TblCaption = Data.TableName;
            CountTblCaption = ProjectCount.TableName;

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

                if(iscol && isrow && col > 0)
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


}
