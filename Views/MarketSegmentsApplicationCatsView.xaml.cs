using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for MarketSegmentsApplicationCatsView.xaml
    /// </summary>
    public partial class MarketSegmentsApplicationCatsView : Window
    {
        public MarketSegmentsApplicationCatsView()
        {
            InitializeComponent();
            DataContext = new ViewModels.MarketSegmentsApplicationCatsViewModel();
        }
    }
}
