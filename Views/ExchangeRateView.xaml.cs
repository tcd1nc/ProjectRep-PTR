using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ExchangeRateView.xaml
    /// </summary>
    public partial class ExchangeRateView : Window
    {
        public ExchangeRateView()
        {
            InitializeComponent();
            DataContext = new ViewModels.ExchangeRateViewModel();
        }

       
    }
}
