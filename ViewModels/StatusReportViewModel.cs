using System.Data;
using System.Windows;
using System.Windows.Input;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class StatusReportViewModel : FilterModule
    {              
        public StatusReportViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            ExecuteFMOpenProject = ExecuteOpenProject;
            FilterData();
        }

        #region Properties

        DataTable datatable;
        public DataTable Data
        {
            get { return datatable; }
            set { SetField(ref datatable, value); }
        }

        #endregion

        #region Commands 

        DataRowView selectedproject;
        public DataRowView SelectedProject
        {
            get { return selectedproject; }
            set { SetField(ref selectedproject, value); }
        }

        private void ExecuteOpenProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = (int)SelectedProject["ProjectID"];
                    bool result = msgbox.OpenProjectDlg((Window)parameter, id);
                    //if return value is true then Refresh list
                    if (result == true)
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
                DataTable dt = datatable.Copy();
                foreach (DataColumn dc in datatable.Columns)
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
            Data = GetStatusReportFiltered(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, ShowKPM(), ShowAllKPM(), GetCDPCCPList());            
        }
               
        #endregion

    }
}
