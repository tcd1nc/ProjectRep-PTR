using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for MaintenanceView.xaml
    /// </summary>
    public partial class MaintenanceView : Window
    {
       
        public MaintenanceView()
        {
            InitializeComponent();
            //new project            
            this.DataContext = new ViewModels.MaintenanceViewModel();
           
        }
                      
    }
}
