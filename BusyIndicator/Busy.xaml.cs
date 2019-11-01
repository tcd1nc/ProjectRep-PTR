using System.Windows;


namespace PTR
{
    /// <summary>
    /// Interaction logic for Busy.xaml
    /// </summary>
    public partial class Busy : Window
    {
        public Busy()
        {
            InitializeComponent();
        }

        public Busy(string content)
        {
            InitializeComponent();
            indicator.BusyContent = content;
        }
    }
}
