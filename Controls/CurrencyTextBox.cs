using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PTR
{
    public class CurrencyTextBox : TextBox
    {
        //create instance using TextBox control as base
        public CurrencyTextBox() : base()
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

        public static readonly DependencyProperty NumberTypeProperty = DependencyProperty.RegisterAttached("NumberType", typeof(string), typeof(CurrencyTextBox),
             new UIPropertyMetadata(string.Empty));

        public string NumberType
        {
            get { return (string)GetValue(NumberTypeProperty); }
            set { SetValue(NumberTypeProperty, value); }
        }

        public static readonly DependencyProperty BackgroundErrorColorProperty = DependencyProperty.RegisterAttached("BackgroundErrorColor", typeof(SolidColorBrush), typeof(CurrencyTextBox),
             new UIPropertyMetadata(new SolidColorBrush(Colors.Red)));

        public SolidColorBrush BackgroundErrorColor
        {
            get { return (SolidColorBrush)GetValue(BackgroundErrorColorProperty); }
            set { SetValue(BackgroundErrorColorProperty, value); }
        }


        public static readonly DependencyProperty NumericalErrorProperty = DependencyProperty.Register("NumericalError", typeof(bool), typeof(CurrencyTextBox),
            new UIPropertyMetadata(false));

        public bool NumericalError
        {
            get { return (bool)GetValue(NumericalErrorProperty); }
            set { SetValue(NumericalErrorProperty, value); }
        }

        public static readonly DependencyProperty CultureProperty = DependencyProperty.Register("Culture", typeof(string), typeof(CurrencyTextBox),
           new UIPropertyMetadata("en-US"));

        public string Culture
        {
            get { return (string)GetValue(CultureProperty); }
            set { SetValue(CultureProperty, value); }
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
            char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
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
                bool blnInt = int.TryParse(value.ToString(), NumberStyles.Currency, cultinfo, out int enteredint);

                if (blnInt)
                {
                    if (enteredint <= Maximum && enteredint >= Minimum)
                        return true;
                    else
                        return false;
                }


                return blnInt;
            }
            else
                return false;
        }

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.RegisterAttached("Maximum", typeof(int), typeof(CurrencyTextBox),
              new UIPropertyMetadata(int.MaxValue));

        public int Maximum
        {
            get { return (int)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.RegisterAttached("Minimum", typeof(int), typeof(CurrencyTextBox),
              new UIPropertyMetadata(0));

        public int Minimum
        {
            get { return (int)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

    }
}

