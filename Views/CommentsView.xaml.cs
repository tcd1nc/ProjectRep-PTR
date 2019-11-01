using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for CommentsView.xaml
    /// </summary>
    public partial class CommentsView : Window
    {
        public CommentsView(FullyObservableCollection<Models.CommentsModel> _comments)
        {
            InitializeComponent();
            DataContext = new ViewModels.CommentsViewModel(_comments);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }
    }
}
