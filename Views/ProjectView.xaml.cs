using System.Windows;


namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ProjectView.xaml
    /// </summary>
    public partial class ProjectView : Window
    {
       
        public ProjectView()
        {
            InitializeComponent();
            //new project
            
            this.DataContext = new ViewModels.ProjectViewModel();
           
        }

        public ProjectView(int projectID)
        {
            InitializeComponent();
           
            //existing project
            this.DataContext = new ViewModels.ProjectViewModel(projectID);
           
        }
               
    }
}
