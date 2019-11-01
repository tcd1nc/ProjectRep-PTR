using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ProjectReportView.xaml
    /// </summary>
    public partial class EvaluationPlansView : Window
    {
        public EvaluationPlansView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.EvaluationPlansViewModel();
        }
    }
}
