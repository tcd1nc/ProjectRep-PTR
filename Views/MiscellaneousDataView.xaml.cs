using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for MiscellaneousDataView.xaml
    /// </summary>
    public partial class MiscellaneousDataView : Window
    {
        public MiscellaneousDataView()
        {
            InitializeComponent();
            DataContext = new ViewModels.MiscellaneousDataViewModel();
        }
    }
}
