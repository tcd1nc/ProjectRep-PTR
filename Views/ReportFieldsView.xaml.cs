using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ReportFieldsView.xaml
    /// </summary>
    public partial class ReportFieldsView : Window
    {
        public ReportFieldsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ReportFieldsViewModel();
        }
    }
}
