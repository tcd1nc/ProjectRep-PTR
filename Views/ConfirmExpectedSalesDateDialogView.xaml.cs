using System;
using System.Windows;
using System.Windows.Input;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ConfirmExpectedSalesDateDialogView.xaml
    /// </summary>
    public partial class ConfirmExpectedSalesDateDialogView : Window
    {
        public ConfirmExpectedSalesDateDialogView(DateTime?  values)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.ConfirmExpectedSalesDateDialogViewModel(values);           
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;
            
        }

    }
}
