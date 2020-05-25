
using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for NewBusinessCategoryView.xaml
    /// </summary>
    public partial class NewBusinessCategoryView : Window
    {
        public NewBusinessCategoryView()
        {
            InitializeComponent();
            DataContext = new ViewModels.NewBusinessCategoriesViewModel();
        }
    }
}
