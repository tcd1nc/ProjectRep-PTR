using static PTR.DatabaseQueries;
using PTR.Models;
using System.Data;
using System.Windows.Input;
using System;

namespace PTR.ViewModels
{
    public class CustomReportViewModel : ViewModelBase
    {
        public ICommand GenerateReportCommand { get; set; }
        public ICommand MakeCustomReportCommand { get; set; }
        bool genreport = false;
        bool makeexcelrpt = false;

        public CustomReportViewModel()
        {            
            CustomReports = GetCustomReports();
            GenerateReportCommand = new RelayCommand(GenerateReport, param => this.genreport);
            MakeCustomReportCommand = new RelayCommand(MakeCustomReport, param => this.makeexcelrpt);
        }

        #region Properties

        FullyObservableCollection<CustomReportModel> customreports;
        public FullyObservableCollection<CustomReportModel> CustomReports
        {
            get { return customreports; }
            set { SetField(ref customreports, value); }
        }

        CustomReportModel selectedreport;
        public CustomReportModel SelectedReport
        {
            get { return selectedreport; }
            set { SetField(ref selectedreport, value);
                try
                {
                    Parameters = GetCustomReportParameters(value.ID);
                    ReportData?.Clear();
                    ReportData = null;
                    genreport = (value != null);
                    makeexcelrpt = false;                  
                }
                catch
                {
                }
            }
        }

        FullyObservableCollection<CustomReportParametersModel> parameters;
        public FullyObservableCollection<CustomReportParametersModel> Parameters
        {
            get { return parameters; }
            set { SetField(ref parameters, value); }
        }

        DataSet ds;
        public DataSet ReportData
        {
            get { return ds; }
            set { SetField(ref ds, value); }
        }

        #endregion

        #region Commands

        private void GenerateReport(object obj)
        {
            try
            {
                ReportData = GetCustomReportData(SelectedReport, Parameters);
                makeexcelrpt = (ReportData != null);
            }
            catch
            {
            }
        }

        private void MakeCustomReport(object obj)
        {
            if (ReportData != null && SelectedReport != null)
            {
                ExcelLib xl = new ExcelLib();
                xl.MakeCustomReport((System.Windows.Window)obj, ReportData, SelectedReport);
                xl = null;                
            }            
        }

        #endregion


    }
}
