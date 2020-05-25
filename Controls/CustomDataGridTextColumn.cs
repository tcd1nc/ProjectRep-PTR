using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PTR
{
    public class CustomDataGridTextColumn : DataGridTextColumn
    {
        public CustomDataGridTextColumn() : base()
        {
            
        }

        protected override System.Windows.FrameworkElement GenerateElement(DataGridCell cell, object dataItem)
        {
            var textBox = (TextBox)base.GenerateEditingElement(cell, dataItem);
            //textBox.GotFocus += TextBox_GotFocus;
            //// textBox.TextChanged += OnTextChanged;

            //textBox.PreviewMouseLeftButtonDown += TextBox_PreviewMouseLeftButtonDown;
            //textBox.GotKeyboardFocus += TextBox_GotKeyboardFocus;
            //textBox.MouseDoubleClick += TextBox_MouseDoubleClick;

          
            return base.GenerateElement(cell, dataItem);
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }

        private void TextBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            (sender as TextBox).SelectAll();
        }


    }
       

}

