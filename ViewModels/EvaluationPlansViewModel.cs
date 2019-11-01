using System.Data;
using System.Windows;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class EvaluationPlansViewModel : FilterModule
    {
        public EvaluationPlansViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            FilterData();
        }

        #region Properties

        DataTable eps;
        public DataTable EPS
        {
            get { return eps; }
            set { SetField(ref eps, value); }
        }

        DataRowView selectedep;
        public DataRowView SelectedEP
        {
            get { return selectedep; }
            set { SetField(ref selectedep, value); }
        }

        #endregion

        #region Commands

        private void ExecuteApplyFilter(object parameter)
        {
            FilterData();
        }

        private void FilterData()
        {
            EPS = DatabaseQueries.GetEvaluationPlansReport(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, ShowKPM(), ShowAllKPM(), GetCDPCCPList());
        }

        ICommand openep;
        public ICommand OpenEP
        {
            get
            {
                if (openep == null)
                    openep = new DelegateCommand(CanExecute, ExecuteOpenEP);
                return openep;
            }
        }

        private void ExecuteOpenEP(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    int evalplanid = (int)SelectedEP["ID"];
                    int projectid = (int)SelectedEP["ProjectID"];                        
                    IMessageBoxService msgbox = new MessageBoxService();
                    bool result = msgbox.EvaluationPlanDialog((Window)parameter, evalplanid, projectid);
                    //if return value is true then Refresh list
                    if (result== true)
                        FilterData();

                    msgbox = null;                    
                }
            }
            catch { }
        }

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
                DataTable dt = eps.Copy();
                foreach (DataColumn dc in eps.Columns)
                    if ((int)dc.ExtendedProperties["FieldType"] == 1)
                        dt.Columns.Remove(dc.ColumnName);

                ExcelLib xl = new ExcelLib();
                xl.MakeGenericReport(dt);
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

        #endregion
    }
}
