using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for EditCustomerView.xaml
    /// </summary>
    public partial class EditCustomerView : Window
    {
        public EditCustomerView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CustomersViewModel();
        }
    }
}
