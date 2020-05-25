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
        public ConfirmExpectedSalesDateDialogView(DateTime? estDateFirstSales, DateTime? StatusMonth)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.ConfirmExpectedSalesDateDialogViewModel(estDateFirstSales, StatusMonth);           
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;            
        }

    }
}
