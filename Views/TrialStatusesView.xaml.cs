using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for TrialStatusesView.xaml
    /// </summary>
    public partial class TrialStatusesView : Window
    {
        public TrialStatusesView()
        {
            InitializeComponent();
            DataContext = new ViewModels.TrialStatusesViewModel();
        }
    }
}
