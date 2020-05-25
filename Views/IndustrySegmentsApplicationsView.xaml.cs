using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for IndustrySegmentsApplicationsView.xaml
    /// </summary>
    public partial class IndustrySegmentsApplicationsView : Window
    {
        public IndustrySegmentsApplicationsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.IndustrySegmentsApplicationsViewModel();
        }
    }
}
