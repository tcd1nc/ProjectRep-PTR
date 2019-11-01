using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for CustomersView2.xaml
    /// </summary>
    public partial class EditCustomerView2 : Window
    {
              
        public EditCustomerView2()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CustomersViewModel2();
        }

        

    }
}
