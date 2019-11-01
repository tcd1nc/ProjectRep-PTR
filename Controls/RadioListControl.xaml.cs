using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace PTR
{
    /// <summary>
    /// Interaction logic for FilterListControl.xaml
    /// </summary>
    public partial class RadioListControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            //if (PropertyChanged != null)
           // {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
           // }
        }

        public RadioListControl()
        {
            InitializeComponent();         
        }

        #region Dependency Property Declarations

        public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(FullyObservableCollection<RadioListItem>), 
            typeof(RadioListControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(SetSelectedID)));
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(RadioListControl));
        public static readonly DependencyProperty SelectedItemIDProperty = DependencyProperty.Register("SelectedItemID", typeof(int), typeof(RadioListControl));
        public static readonly DependencyProperty TTipProperty = DependencyProperty.Register("TTip", typeof(string), typeof(RadioListControl));
        #endregion  Dependency Property Declarations              

        public ObservableCollection<RadioListItem> ListItems
        {
            get { return (ObservableCollection<RadioListItem>)GetValue(ListItemsProperty); }
            set { SetValue(ListItemsProperty, value); }
        }
        
        public int SelectedItemID
        {
            get { return (int)GetValue(SelectedItemIDProperty); }
            set { SetValue(SelectedItemIDProperty, value); }
        }        

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            SelectedItemID = (int)(sender as RadioButton).Tag;
        }

        private static void SetSelectedID(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {           
            foreach(RadioListItem ri in (FullyObservableCollection<RadioListItem>)(args.NewValue))
            {
                if(ri.IsSelected)
                {
                    (source as RadioListControl).SelectedItemID = ri.ID;
                    break;
                }
            }           
        }

        public string TTip
        {
            get { return (string)GetValue(TTipProperty); }
            set { SetValue(TTipProperty, value); }
        }
    }

}
