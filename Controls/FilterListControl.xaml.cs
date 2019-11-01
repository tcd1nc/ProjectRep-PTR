using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;


namespace PTR
{
    /// <summary>
    /// Interaction logic for FilterListControl.xaml
    /// </summary>
    public partial class FilterListControl : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string info)
        {
            //if (PropertyChanged != null)
           // {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(info));
           // }
        }

        public FilterListControl()
        {
            InitializeComponent();         
        }

        #region Dependency Property Declarations

     //   public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(ObservableCollection<FilterListItem>), typeof(FilterListControl), new FrameworkPropertyMetadata(new PropertyChangedCallback(ListChanged)));
        public static readonly DependencyProperty ListItemsProperty = DependencyProperty.Register("ListItems", typeof(FullyObservableCollection<FilterListItem>), 
            typeof(FilterListControl), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public static readonly DependencyProperty TTipProperty = DependencyProperty.Register("TTip", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register("Heading", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(string), typeof(FilterListControl));
        public static readonly DependencyProperty ToggleButtonVisibilityProperty = DependencyProperty.Register("ToggleButtonVisibility", typeof(Visibility), typeof(FilterListControl));
        public static readonly DependencyProperty ShowItemsProperty = DependencyProperty.Register("ShowItems", typeof(bool), typeof(FilterListControl));
       
        //commands
        //showallitems
        public static readonly DependencyProperty SelectAllButtonCommandProperty = DependencyProperty.Register("SelectAllButtonCommand", typeof(ICommand), typeof(FilterListControl));
        public static readonly DependencyProperty SelectAllButtonParameterProperty = DependencyProperty.Register("SelectAllButtonParameter", typeof(object), typeof(FilterListControl));
        public static readonly DependencyProperty ExpandButtonCommandProperty = DependencyProperty.Register("ExpandButtonCommand", typeof(ICommand), typeof(FilterListControl));

        public static readonly DependencyProperty ShowAllProperty = DependencyProperty.Register("ShowAll", typeof(bool), 
            typeof(FilterListControl), new FrameworkPropertyMetadata(true,ShowAllCallback));
        public static readonly DependencyProperty SelectAllProperty = DependencyProperty.Register("SelectAll", typeof(bool),
            typeof(FilterListControl), new FrameworkPropertyMetadata(false, SelectAllCallback));
        public static readonly DependencyProperty VisibleCountProperty = DependencyProperty.Register("VisibleCount", typeof(int),
            typeof(FilterListControl));

        #endregion  Dependency Property Declarations

        #region Commands

        public ICommand ExpandButtonCommand
        {
            get { return (ICommand)GetValue(ExpandButtonCommandProperty); }
            set { SetValue(ExpandButtonCommandProperty, value); }
        }
        public ICommand SelectAllButtonCommand
        {
            get { return (ICommand)GetValue(SelectAllButtonCommandProperty); }
            set { SetValue(SelectAllButtonCommandProperty, value); }
        }

        public object SelectAllButtonParameter
        {
            get { return (object)GetValue(SelectAllButtonParameterProperty); }
            set { SetValue(SelectAllButtonParameterProperty, value); }
        }

        #endregion Commands

        public int VisibleCount
        {
            get { return (int)GetValue(VisibleCountProperty); }
            set { SetValue(VisibleCountProperty, value); }
        }

        public bool ShowAll
        {
            get { return (bool)GetValue(ShowAllProperty); }
            set { SetValue(ShowAllProperty, value); }
        }

        private static void ShowAllCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null && (source as FilterListControl).ListItems!=null)
            {
                if((bool)args.NewValue == true)
                    foreach (FilterListItem fi in (source as FilterListControl).ListItems)                
                        fi.VisibleState = Visibility.Visible;                
                else
                    for (var i = (source as FilterListControl).ListItems.Count - 1; i >= 0; i--)
                    {
                        if (i < (source as FilterListControl).VisibleCount)                        
                            (source as FilterListControl).ListItems[i].VisibleState = Visibility.Visible;                        
                        else                        
                            (source as FilterListControl).ListItems[i].VisibleState = Visibility.Collapsed;                        
                    }
            }
        }

        public bool SelectAll
        {
            get { return (bool)GetValue(SelectAllProperty); }
            set { SetValue(SelectAllProperty, value); }
        }

        private static void SelectAllCallback(DependencyObject source, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != null && (source as FilterListControl).ListItems != null)
            {
                if ((bool)args.NewValue == true)
                    (source as FilterListControl).SelectItems();
                else
                    (source as FilterListControl).UnSelectItems();
            }
        }

        private void UnSelectItems()         
        {
            foreach (FilterListItem fi in ListItems)            
                fi.IsSelected = false;
            
        }
        private void SelectItems()    
        {
            foreach (FilterListItem fi in ListItems)            
                fi.IsSelected = true;            
        }


        public ObservableCollection<FilterListItem> ListItems
        {
            get { return (ObservableCollection<FilterListItem>)GetValue(ListItemsProperty); }
            set { SetValue(ListItemsProperty, value); }
        }

        public string SelectedItems
        {
            get { return (string)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        public Visibility ToggleButtonVisibility
        {
            get { return (Visibility)GetValue(ToggleButtonVisibilityProperty); }
            set { SetValue(ToggleButtonVisibilityProperty, value); }
        }

        public bool ShowItems
        {
            get { return (bool)GetValue(ShowItemsProperty); }
            set { SetValue(ShowItemsProperty, value); }
        }

        public string Heading
        {
            get { return (string)GetValue(HeadingProperty); }
            set { SetValue(HeadingProperty, value); }
        }

        public string TTip
        {
            get { return (string)GetValue(TTipProperty); }
            set { SetValue(TTipProperty, value); }
        }
    }
}
