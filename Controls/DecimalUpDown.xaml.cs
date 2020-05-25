using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace PTR.Controls
{
    /// <summary>
    /// Interaction logic for DecimalUpDown.xaml
    /// </summary>
    public partial class DecimalUpDown : UserControl
    {
        public DecimalUpDown()
        {
            InitializeComponent();
        }

        public decimal Maximum
        {
            get { return (decimal)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(decimal), typeof(DecimalUpDown), new UIPropertyMetadata(decimal.MaxValue));
                
        public decimal Minimum
        {
            get { return (decimal)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(decimal), typeof(DecimalUpDown), new UIPropertyMetadata(decimal.Zero));
        
        public decimal Value
        {
            get { return (decimal)GetValue(ValueProperty); }
            set {SetValue(ValueProperty, value); }
        }

        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(decimal), typeof(DecimalUpDown), new FrameworkPropertyMetadata(decimal.Zero, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChanged));

        private static void ValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            ((DecimalUpDown)target).Value = (decimal) e.NewValue;
            ((DecimalUpDown)target).tbmain.Text = e.NewValue.ToString();           
        }
                    
        public readonly static DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(decimal), typeof(DecimalUpDown), new UIPropertyMetadata(decimal.Zero));
        
        public decimal Step
        {
            get { return (decimal)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public readonly static DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(DecimalUpDown), new UIPropertyMetadata("0.00"));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }

        private void btup_Click(object sender, RoutedEventArgs e)
        {
            if (Value < Maximum)
            {
                Value += Step;
                if (Value > Maximum)
                    Value = Maximum;

                tbmain.Text = Value.ToString(Format);
            }
        }

        private void btdown_Click(object sender, RoutedEventArgs e)
        {
            if (Value > Minimum)
            {
                Value -= Step;
                if (Value < Minimum)
                    Value = Minimum;
                tbmain.Text = Value.ToString(Format);
            }
        }

        private void Tbmain_TextChanged(object sender, TextChangedEventArgs e)
        {
            CultureInfo cultinfo;
            string cult = "en-US";
            cultinfo = new CultureInfo(cult);
            bool blnDecimal = decimal.TryParse(tbmain.Text, NumberStyles.AllowDecimalPoint | NumberStyles.Number, cultinfo, out decimal enteredDecimal);
            Value = enteredDecimal;
                                  
        }             

        private void Tbmain_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            var tb = (TextBox)sender;
            var txt = tb.Text.Insert(tb.CaretIndex, e.Text);
            CultureInfo cultinfo;
            string cult = "en-US";

            cultinfo = new CultureInfo(cult);
            bool blnDecimal = decimal.TryParse(txt, NumberStyles.AllowDecimalPoint | NumberStyles.Number, cultinfo, out decimal enteredDecimal);
            if (blnDecimal)
                e.Handled = false;
            else
                e.Handled = true;
            
            base.OnTextInput(e);
        }
    }
}
