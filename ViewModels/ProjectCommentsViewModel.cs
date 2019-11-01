using System.Windows.Input;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows;

namespace PTR.ViewModels
{
    public class ProjectCommentsViewModel : FilterModule
    {
        private const string title = "Project Comments";              
        ProjectReportSummary project;
        
        private readonly Window windowref;

        public ProjectCommentsViewModel(Window winref, int projectid)
        {
            project = GetSimpleProjectDetails(projectid);

            if (projectid == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (Project ID: " + projectid.ToString() + ")";

            windowref = winref;

        }

        #region View Properties

        public ProjectReportSummary SelectedProject
        {
            get { return project; }
            set { SetField(ref project, value); }
        }
        
        bool clearactivities;
        public bool ClearActivities
        {
            get { return clearactivities; }
            set { SetField(ref clearactivities, value); }
        }

        private void ExecuteClearActivities()
        {
            ClearActivities = true;
            ClearActivities = false;
        }

        bool updateactivities;
        public bool UpdateActivities
        {
            get { return updateactivities; }
            set { SetField(ref updateactivities, value); }
        }

        private void ExecuteUpdateActivities()
        {
            UpdateActivities = true;
            UpdateActivities = false;
        }

        string windowtitle;
        public string WindowTitle
        {
            get { return windowtitle; }
            set { SetField(ref windowtitle, value); }
        }

        bool? returnobject;
        public bool? SaveCommentsFlag
        {
            get { return returnobject; }
            set { SetField(ref returnobject, value); }
        }              

        #endregion
        
        #region Commands
       
        bool canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {        
            return canexecutesave;
        }

        //save
        ICommand savecomments;
        public ICommand SaveComments
        {
            get
            {
                if (savecomments == null)
                    savecomments = new DelegateCommand(CanExecuteSave, ExecuteSaveComments);
                return savecomments;
            }
        }

        private void ExecuteSaveComments(object parameter)
        {        
           // ExecuteUpdateActivities();

            SaveCommentsFlag = true;
            CloseWindowFlag = true;
        }

        ICommand cancelandclose;
        public ICommand CancelUpdate
        {
            get
            {
                if (cancelandclose == null)
                    cancelandclose = new DelegateCommand(CanExecute, ExecuteCancelAndClose);
                return cancelandclose;
            }
        }

        private void ExecuteCancelAndClose(object parameter)
        {        
            SaveCommentsFlag = false;
            CloseWindowFlag = true;
        }

        #endregion

        //use with MyDependencyProperty
        //Window wref1;
        //public Window Wref
        //{
        //    get { return wref1; }
        //    set { wref1 = value; }
        //}

        ICommand windowclosing;
        bool canclosewindow = true;
        private bool CanCloseWindow(object obj)
        {
            return canclosewindow;
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExCloseWindow);
                return windowclosing;
            }
        }
        private void ExCloseWindow(object parameter)
        {
            ExecuteUpdateActivities();
        }

    }
}