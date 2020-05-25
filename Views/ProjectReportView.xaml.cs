using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using PTR.Models;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ProjectReportView.xaml
    /// </summary>
    public partial class ProjectReportView : Window
    {
        DataTable dt;
        Dictionary<string, FilterPopupModel> dictFilterPopup;
        public ProjectReportView()
        {
            InitializeComponent();
            try
            {
                this.DataContext = new ViewModels.ProjectReportViewModel();
                ((ViewModels.ProjectReportViewModel)DataContext).PropertyChanged += ProjectReportView_PropertyChanged;
                dt = (DataContext as ViewModels.ProjectReportViewModel).Projects;
                dictFilterPopup = (DataContext as ViewModels.ProjectReportViewModel).DictFilterPopup;
            }
            catch
            {
            }
        }

        private void ProjectReportView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                dt = (DataContext as ViewModels.ProjectReportViewModel).Projects;
                dictFilterPopup = (DataContext as ViewModels.ProjectReportViewModel).DictFilterPopup;
            }
            catch
            {
            }
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            StaticCollections.FormatWithStatusColumn(ref dt, this, ref e, ref dictFilterPopup, Constants.ProjectListReportPopupList);
        }
    }
}
