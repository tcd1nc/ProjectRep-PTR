using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ProjectReportView.xaml
    /// </summary>
    public partial class ProjectReportView : Window
    {
        public ProjectReportView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.ProjectReportViewModel();
        }
    }
}
