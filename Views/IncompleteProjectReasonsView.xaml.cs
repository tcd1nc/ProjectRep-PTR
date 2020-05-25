using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for IncompleteProjectReasonsView.xaml
    /// </summary>
    public partial class IncompleteProjectReasonsView : Window
    {
        public IncompleteProjectReasonsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.IncompleteProjectReasonsViewModel();
        }
    }
}
