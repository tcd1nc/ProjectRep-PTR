using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for SMCodesView.xaml
    /// </summary>
    public partial class SMCodesView : Window
    {
        public SMCodesView()
        {
            InitializeComponent();
            DataContext = new ViewModels.SMCodesViewModel();
        }
    }
}
