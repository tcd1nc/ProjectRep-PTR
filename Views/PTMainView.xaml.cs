
using System.Windows;

namespace PTR
{
    /// <summary>
    /// Interaction logic for PTMainView.xaml
    /// </summary>
    public partial class PTMainView : Window
    {

        public PTMainView()
        {                                           
            InitializeComponent();
            this.DataContext = new ViewModels.PTMainViewModel();
            
            this.Loaded += new RoutedEventHandler(PTMainView_Loaded);
        }
        void PTMainView_Loaded(object sender, RoutedEventArgs e)
        {
           (this.DataContext as ViewModels.PTMainViewModel).ShowReminderScreen();
        }
    }

}

