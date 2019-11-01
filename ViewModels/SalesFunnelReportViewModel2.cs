using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class SalesFunnelReportViewModel2 : FilterModule
    {
        Collection<ProjectSummary> _projectssummary = new Collection<ProjectSummary>();

        DateTime? _firstmonth;
        DateTime? _lastmonth;

        Models.ProjectMonthRange _projectdaterange;

        public SalesFunnelReportViewModel2()
        {
            _projectdaterange = DatabaseQueries.GetFirstLastMonths();
            _firstmonth = GetPreviousYearStartMonth();
            _lastmonth = _projectdaterange.LastDate;
            FilterData();           
        }

        #region Properties

        DataTable _datatable;
        public DataTable Data
        {
            get { return _datatable; }
            set { SetField(ref _datatable, value); }
        }

        DataTable _projectcountdatatable;
        public DataTable ProjectCount
        {
            get { return _projectcountdatatable; }
            set { SetField(ref _projectcountdatatable, value); }
        }

        DataTable _projectsummarydatatable;
        public DataTable ProjectSummary
        {
            get { return _projectsummarydatatable; }
            set { SetField(ref _projectsummarydatatable, value); }
        }
        
        bool _showtooltip;
        public bool ShowTooltip
        {
            get { return _showtooltip; }
            set { SetField(ref _showtooltip, value); }
        }

        string _summarystring;
        public string SummaryString
        {
            get { return _summarystring; }
            set { SetField(ref _summarystring, value); }
        }

        string _currentmonth;
        public string CurrentMonth
        {
            get { return _currentmonth; }
            set { SetField(ref _currentmonth, value); }
        }

        string _currenstatus;
        public string CurrentStatus
        {
            get { return _currenstatus; }
            set { SetField(ref _currenstatus, value); }
        }
                
        public DateTime? FirstMonth
        {
            get { return _firstmonth; }
            set { SetField(ref _firstmonth, value); }
        }

        public DateTime? LastMonth
        {
            get { return _lastmonth; }
            set { SetField(ref _lastmonth, value); }
        }
                      
        bool _useUSD = false;
        public bool UseUSD
        {
            get { return _useUSD; }
            set { SetField(ref _useUSD, value); }
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

        ICommand _exporttoexcel;
        public ICommand ExportToExcel
        {
            get
            {
                if (_exporttoexcel == null)
                    _exporttoexcel = new DelegateCommand(CanExecute, ExecuteExportToExcel);
                return _exporttoexcel;
            }
        }

        private void ExecuteExportToExcel(object parameter)
        {
            try
            { 
                CreateExcelFile c = new CreateExcelFile();
                c.CreateExcelSummaryReport(_datatable, _projectcountdatatable);
            }
            catch
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("There was a problem creating the Excel report","Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
        }
              
        ICommand _applyfilter;
        public ICommand ApplyFilter
        {
            get
            {
                if (_applyfilter == null)
                {
                    _applyfilter = new DelegateCommand(CanExecute, ExecuteApplyFilter);
                }
                return _applyfilter;
            }
        }
        private void ExecuteApplyFilter(object parameter)
        {
              FilterData();
        }
        
        private void FilterData()
        {            
            DataSet _ds;            
            _ds = DatabaseQueries.GetFilteredProjectsEstSales(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, UseUSD, (DateTime)_firstmonth, (DateTime)_lastmonth);
                                    
            Data = _ds.Tables["TotalData"];
            ProjectCount = _ds.Tables["ProjectCountData"];            
            ProjectSummary = DatabaseQueries.GetSummaryByStatusMonth(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, UseUSD, (DateTime)_firstmonth, (DateTime)_lastmonth);

        }

        ICommand _mousemove;
        public ICommand MouseMove
        {
            get
            {
                if (_mousemove == null)
                    _mousemove = new DelegateCommand(CanExecute, ExecuteMouseMove);
                return _mousemove;
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


    //public class ProjectSummary
    //{
    //    public string ProjectName
    //    {
    //        get;set;
    //    }
    //    public decimal TargetedSales  { get; set; }
    //}


}
