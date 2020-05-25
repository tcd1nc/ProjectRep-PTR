using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ApplicationsView.xaml
    /// </summary>
    public partial class ApplicationsView : Window
    {
        public ApplicationsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ApplicationsViewModel();
        }
    }
}
