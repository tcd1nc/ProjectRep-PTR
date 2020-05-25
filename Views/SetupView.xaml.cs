using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for SetupView.xaml
    /// </summary>
    public partial class SetupView : Window
    {
        public SetupView()
        {
            InitializeComponent();
            DataContext = new ViewModels.SetupViewModel();
        }
    }
}
