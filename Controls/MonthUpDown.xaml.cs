using System;
using System.Windows;
using System.Windows.Controls;

namespace PTR.Controls
{
    /// <summary>
    /// Interaction logic for MonthUpDown.xaml
    /// </summary>
    public partial class MonthUpDown : UserControl
    {
        public MonthUpDown()
        {
            InitializeComponent();
        }

        public DateTime Maximum
        {
            get { return (DateTime)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }
        public readonly static DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(DateTime?), typeof(MonthUpDown), new UIPropertyMetadata(DateTime.MaxValue));
                
        public DateTime Minimum
        {
            get { return (DateTime)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public readonly static DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(DateTime?), typeof(MonthUpDown), new UIPropertyMetadata(DateTime.MinValue));
        
        public DateTime? Value
        {
            get { return (DateTime?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public readonly static DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(DateTime?), typeof(MonthUpDown), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ValueChanged));

        private static void ValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {

            ((MonthUpDown)target).tbmain.Text = ((DateTime)e.NewValue).ToString(((MonthUpDown)target).Format);

        }


        public readonly static DependencyProperty StepProperty = DependencyProperty.Register("Step", typeof(int), typeof(MonthUpDown), new UIPropertyMetadata(1));
        
        public int Step
        {
            get { return (int)GetValue(StepProperty); }
            set { SetValue(StepProperty, value); }
        }

        public readonly static DependencyProperty FormatProperty = DependencyProperty.Register("Format", typeof(string), typeof(MonthUpDown), new UIPropertyMetadata(string.Empty));

        public string Format
        {
            get { return (string)GetValue(FormatProperty); }
            set { SetValue(FormatProperty, value); }
        }
                      
        private void btup_Click(object sender, RoutedEventArgs e)
        {
            if (Value !=null && Value < Maximum)
            {
                Value =  ((DateTime) Value).AddMonths(Step);
                if (Value > Maximum)
                    Value = Maximum;
            }
        }

        private void btdown_Click(object sender, RoutedEventArgs e)
        {
            if (Value !=null && Value > Minimum)
            {
                Value = ((DateTime)Value).AddMonths(-Step);
                if (Value < Minimum)
                    Value = Minimum;
            }
        }

    }
}
