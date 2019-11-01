using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace PTR
{
    /// <summary>
    /// Interaction logic for CustomDialog.xaml
    /// </summary>
    public partial class CustomDialog : Window
    {
        /// <summary>
        /// Constructor with default parameters
        /// </summary> 
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        /// <param name="_title"></param>
        /// <param name="_message"></param>       
        /// <param name="_items"></param>
        /// <param name="_msgimage"></param>
        public CustomDialog(double width, double height, string title = null, string message = null, ItemCollection items = null, 
            System.Drawing.Icon msgimage = null)
        {
            InitializeComponent();
            this.NewTitle.Text = title;
            this.Width = width;
            this.Height = height;
            this.msgtext.Text = message;
            if(msgimage != null)
                this.msgicon.Source = Imaging.CreateBitmapSourceFromHIcon(msgimage.Handle, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                                 
            buttons.ItemsSource = items;

            this.ShowDialog();
        }
                
        /// <summary>
        /// Keyboard key trapping
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //prevent alt-F4 from closing dialog
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;
            
            //allow Return key to set return value as default button
            if (e.Key == Key.Return)
            {                   
                this.Tag = GetDefaultButton();
                DialogResult = true;
            }
        }

        /// <summary>
        /// IsSelected is the default button so if present then its Tag object is returned
        /// </summary>
        /// <returns></returns>
        private object GetDefaultButton()
        {
            foreach(ListBoxItem item in buttons.Items)
            {
                if (item.IsSelected)
                    return item.Tag;
            }
            return null;            
        }
         
        /// <summary>
        /// Set Window Tag to object in selectd button's Tag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Tag = ((Button)(e.OriginalSource)).Tag;
            DialogResult = true;
        }

        /// <summary>
        /// Allow dragging of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
        
    }
}
