using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using PTR.Models;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for EvaluationPlansView.xaml
    /// </summary>
    public partial class EvaluationPlansView : Window
    {
        DataTable dt;
        Dictionary<string, FilterPopupModel> dictFilterPopup;
        public EvaluationPlansView()
        {
            InitializeComponent();           
            try
            {
                this.DataContext = new ViewModels.EvaluationPlansViewModel();
                ((ViewModels.EvaluationPlansViewModel)DataContext).PropertyChanged += EvaluationPlansView_PropertyChanged;
                dt = (DataContext as ViewModels.EvaluationPlansViewModel).EPS;
                dictFilterPopup = (DataContext as ViewModels.EvaluationPlansViewModel).DictFilterPopup;
            }
            catch
            {

            }
        }

        private void EvaluationPlansView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                dt = (DataContext as ViewModels.EvaluationPlansViewModel).EPS;
                dictFilterPopup = (DataContext as ViewModels.EvaluationPlansViewModel).DictFilterPopup;
            }
            catch
            {

            }
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            StaticCollections.FormatWithStatusColumn(ref dt, this, ref e,  ref dictFilterPopup, Constants.EvaluationPlansListReportPopupList);            
        }
    }
}
