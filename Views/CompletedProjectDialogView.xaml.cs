using System.Windows;
using System.Windows.Input;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for CompletedProjectDialogView.xaml
    /// </summary>
    public partial class CompletedProjectDialogView : Window
    {
        public CompletedProjectDialogView(object[] values)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CompletedProjectDialogViewModel(values);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;
            
        }

    }
}
