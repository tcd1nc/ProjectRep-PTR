using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for IndustrySegmentsView.xaml
    /// </summary>
    public partial class IndustrySegmentsView : Window
    {
        public IndustrySegmentsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.IndustrySegmentsViewModel();
        }
    }
}
