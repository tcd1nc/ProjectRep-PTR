using System.Windows.Input;
using static PTR.DatabaseQueries;
using System.Windows;
using System.Data;

namespace PTR.ViewModels
{
    public class ProjectCommentsViewModel : FilterModule
    {
        private const string title = "Project Comments";
        DataRowView selectedproject;
        
        private readonly Window windowref;

        public ProjectCommentsViewModel(Window winref, int projectid)
        {
            selectedproject = GetBasicProjectDetails(projectid, StaticCollections.CurrentUser.ID);

            if (projectid == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (Project ID: " + projectid.ToString() + ")";

            windowref = winref;

        }
        

        #region View Properties

        public DataRowView SelectedProject
        {
            get { return selectedproject; }
            set { SetField(ref selectedproject, value); }
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

        bool isdirtydata = false;
        public bool IsDirtyData
        {
            get { return isdirtydata; }
            set { SetField(ref isdirtydata, value); }
        }

        bool cansave = false;
        public bool CanSave
        {
            get { return cansave; }
            set { SetField(ref cansave, value); }
        }


        #endregion

        #region Commands
      
        private bool CanExecuteSave(object obj)
        {        
            return true;
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
            ExecuteUpdateActivities();

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

        //ICommand windowclosing;
        //bool canclosewindow = true;
        //private bool CanCloseWindow(object obj)
        //{
        //    return canclosewindow;
        //}

        //public ICommand WindowClosing
        //{
        //    get
        //    {
        //        if (windowclosing == null)
        //            windowclosing = new DelegateCommand(CanCloseWindow, ExCloseWindow);
        //        return windowclosing;
        //    }
        //}
        //private void ExCloseWindow(object parameter)
        //{
        //    //ExecuteUpdateActivities();
        //}



        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (IsDirtyData)
            {                
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    ExecuteUpdateActivities();
                    SaveCommentsFlag = true;
                }
                return true;                               
            }
            else
                return true;
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExecuteClosing);
                return windowclosing;
            }
        }

        private void ExecuteClosing(object parameter)
        {           
        }

        ICommand windowcancelclosing;     
        private bool CanCancelCloseWindow(object obj)
        {
            return true;
        }

        public ICommand CancelClosingCommand
        {
            get
            {
                if (windowcancelclosing == null)
                {
                    windowcancelclosing = new DelegateCommand(CanCancelCloseWindow, CancelWindowClosing);
                }
                return windowcancelclosing;
            }
        }

        private void CancelWindowClosing(object parameter)
        {

        }


    }
}