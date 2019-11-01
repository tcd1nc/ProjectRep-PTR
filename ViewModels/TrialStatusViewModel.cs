using System;
using System.Data;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class TrialStatusViewModel : ViewModelBase
    {
        DataTable _failedtrials;
        DataTable _newsales;
        DataTable _scheduledtrials;
        DataTable _importantprojects;

        DateTime? _firstmonth;
        DateTime? _lastmonth;

        public TrialStatusViewModel()
        {           
            _lastmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            _lastmonth = ((DateTime)_lastmonth).AddMonths(-1);
            _firstmonth = ((DateTime)_lastmonth).AddMonths(-6);
            
            GetData();
        }

        public DataTable FailedTrials
        {
            get { return _failedtrials; }
            set { SetField(ref _failedtrials, value); }
        }

        public DataTable NewSales
        {
            get { return _newsales; }
            set { SetField(ref _newsales, value); }
        }

        public DataTable ScheduledTrials
        {
            get { return _scheduledtrials; }
            set { SetField(ref _scheduledtrials, value); }
        }

        public DataTable ImportantProjects
        {
            get { return _importantprojects; }
            set { SetField(ref _importantprojects, value); }
        }

        private DateTime GetPreviousYearStartMonth()
        {
            return new DateTime(DateTime.Now.Year - 1, 1, 1);
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

        ICommand _openproject;
        public ICommand OpenProject
        {
            get
            {
                if (_openproject == null)
                    _openproject = new DelegateCommand(CanExecute, ExecuteOpenProject);
                return _openproject;
            }
        }

        private void ExecuteOpenProject(object parameter)
        {
            if (parameter != null)
            {
                if (parameter != null)
                {
                    if (parameter.GetType().Equals(typeof(DataRowView)))
                    {
                        int ID = (int)(parameter as DataRowView).Row["ID"];
                        IMessageBoxService _msgboxcommand = new MessageBoxService();
                        bool result = _msgboxcommand.OpenProjectDlg(ID);

                        //if return value is true then Refresh list
                        if (result == true)
                        {
                            GetData();
                        }
                        _msgboxcommand = null;
                    }
                }
            }
        }

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
                c.CreatePlaybookSheets(FailedTrials, NewSales, ScheduledTrials, ImportantProjects);
            }
            catch
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("There was a problem creating the Excel report","Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            }
        }

        ICommand _clearfilter;
        public ICommand ClearFilter             //Command to call delegate to clear filter
        {
            get
            {
                if (_clearfilter == null)
                {
                    _clearfilter = new DelegateCommand(CanExecute, ExecuteClearFilter);
                }
                return _clearfilter;
            }
        }

        private void ExecuteClearFilter(object parameter)                                                           
        {
            LastMonth = ((DateTime)_lastmonth).AddMonths(-1);
            FirstMonth = ((DateTime)_lastmonth).AddMonths(-6);
        }

        ICommand _applyfilter;
        public ICommand ApplyFilter         //Command to call delegate to Apply filter
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
            GetData();
        }

        private void GetData()
        {
            FailedTrials = DatabaseQueries.GetPlaybookReport("Failed Trials",(DateTime)FirstMonth, (DateTime)LastMonth);
            ImportantProjects = DatabaseQueries.GetPlaybookReport("Important Projects",(DateTime)FirstMonth, (DateTime)LastMonth);          
            ScheduledTrials = DatabaseQueries.GetPlaybookReport("Scheduled Trials", (DateTime)FirstMonth, (DateTime)LastMonth);
            NewSales = DatabaseQueries.GetPlaybookReport("New Sales", (DateTime)FirstMonth, (DateTime)LastMonth);
        }


        

    }
}
