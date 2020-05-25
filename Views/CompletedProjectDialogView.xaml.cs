using System;
using System.Windows;
using System.Windows.Input;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for CompletedProjectDialogView.xaml
    /// </summary>
    public partial class CompletedProjectDialogView : Window
    {
        public CompletedProjectDialogView(decimal estimatedannualsales, DateTime? expecteddatefirstsales, DateTime statusmonth)
        {
            InitializeComponent();
            this.DataContext = new ViewModels.CompletedProjectDialogViewModel(estimatedannualsales, expecteddatefirstsales, statusmonth);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;            
        }

    }
}
