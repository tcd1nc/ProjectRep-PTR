using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PTR
{
    public class NumericTextBox : TextBox
    {
        //create instance using TextBox control as base
        public NumericTextBox() : base()
        {
            //disable copy and paste
            DataObject.AddPastingHandler(this, this.OnCancelCommand);
            DataObject.AddCopyingHandler(this, this.OnCancelCommand);
            this.ContextMenu = null;
                       
            AddHandler(PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText), true);
        }


        private void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
        {                      
            if (!this.IsKeyboardFocusWithin)
            {
                // If the text box is not yet focussed, give it the focus and
                // stop further processing of this click event.
                this.Focus();
                e.Handled = true;
            }            
        }

        private void SelectAllText(object sender, RoutedEventArgs e)
        {
            this.SelectAll();
        }


        private void OnCancelCommand(object sender, DataObjectEventArgs e)
        {
            e.CancelCommand();
        }

        public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached("NumberType", typeof(string), typeof(NumericTextBox),
             new UIPropertyMetadata(string.Empty));

        public string NumberType
        {
            get { return (string)GetValue(NumberTypeProperty); }
            set { SetValue(NumberTypeProperty, value); }
        }

        public static readonly DependencyProperty BackgroundErrorColorProperty = DependencyProperty.RegisterAttached("BackgroundErrorColor", typeof(SolidColorBrush), typeof(NumericTextBox),
             new UIPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public SolidColorBrush BackgroundErrorColor
        {
            get { return (SolidColorBrush)GetValue(BackgroundErrorColorProperty); }
            set { SetValue(BackgroundErrorColorProperty, value); }
        }
                  
       
        public static readonly DependencyProperty NumericalErrorProperty = DependencyProperty.Register("NumericalError", typeof(bool), typeof(NumericTextBox), 
            new UIPropertyMetadata(false));

        public bool NumericalError
        {
            get { return (bool)GetValue(NumericalErrorProperty); }
            set { SetValue(NumericalErrorProperty, value); }
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            if (!IsValidNumber(Text))
            {
                e.Handled = true;
                Background = BackgroundErrorColor;
                NumericalError = true;                
            }
            else
            {
                e.Handled = false;
                Background = new SolidColorBrush(Colors.White);
                NumericalError = false;
            }
            base.OnTextChanged(e);
        }              

        protected override void OnTextInput(TextCompositionEventArgs e)
        {            
            char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
            if (e.Text.IndexOfAny(chars) == -1)
                e.Handled = true;
            else
                e.Handled = false;

            base.OnTextInput(e);
        }

        private bool IsValidNumber(object value)
        {
            CultureInfo cultinfo = new CultureInfo("en-US");
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                bool blnInt = int.TryParse(value.ToString(), NumberStyles.Number, cultinfo, out int enteredint);
                return blnInt;
            }
            else
                return false;
        }

        //private bool IsValidNumber(object value)
        //{
        //    CultureInfo cultinfo = new CultureInfo("en-US");
        //    if (!string.IsNullOrEmpty(value.ToString()))
        //    {
        //        bool blnInt = decimal.TryParse(value.ToString(), NumberStyles.Number | NumberStyles.AllowDecimalPoint, cultinfo, out decimal enteredint);
        //        return blnInt;
        //    }
        //    else
        //        return false;
        //}

    }
}
