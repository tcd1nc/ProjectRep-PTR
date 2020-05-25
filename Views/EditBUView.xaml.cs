
using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for EditBUView.xaml
    /// </summary>
    public partial class EditBUView : Window
    {
        public EditBUView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.BUViewModel();
        }
    }
}
