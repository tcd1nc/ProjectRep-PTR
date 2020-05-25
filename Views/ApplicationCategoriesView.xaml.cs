using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ApplicationCategoriesView.xaml
    /// </summary>
    public partial class ApplicationCategoriesView : Window
    {
        public ApplicationCategoriesView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ApplicationCategoriesViewModel();
        }
    }
}
