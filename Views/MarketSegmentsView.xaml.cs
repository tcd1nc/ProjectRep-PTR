using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for MarketSegmentsView.xaml
    /// </summary>
    public partial class MarketSegmentsView : Window
    {
        public MarketSegmentsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.MarketSegmentsViewModel();
        }
    }
}
