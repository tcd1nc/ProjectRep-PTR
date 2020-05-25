using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for EditAssociateView.xaml
    /// </summary>
    public partial class AssociatesView : Window
    {
     
        public AssociatesView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.UserViewModel();
        }

       
    }
}
