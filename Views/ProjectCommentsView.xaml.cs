using System.Windows;
using System.Windows.Input;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectCommentsView : Window
    {
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)            
                e.Handled = true;            
        }

        private readonly Window thiswin;

        public ProjectCommentsView(int projectID)
        {
            InitializeComponent();
            //existing project

            thiswin = this;
            this.DataContext = new ViewModels.ProjectCommentsViewModel(thiswin, projectID);  
        }       

    }
}
