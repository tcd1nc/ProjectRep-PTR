using System.Windows;
using System.Windows.Input;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for EP
    /// </summary>
    public partial class EPView : Window
    {
       
        public EPView(int id, int projectid)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.EPViewModel(id, projectid);
        }
        

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;            
        }      

    }
}
