using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class ApplicationsViewModel : ViewModelBase
    {
        bool isdirty = false;
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        FullyObservableCollection<ApplicationModel> appcats = new FullyObservableCollection<ApplicationModel>();

        public ApplicationsViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetApplications();
            canexecutesave = false;
        }

        public FullyObservableCollection<ApplicationModel> Applications
        {
            get { return appcats; }
            set { SetField(ref appcats, value); }
        }

        bool invalidfield;
        public bool InvalidField
        {
            get { return invalidfield; }
            set { SetField(ref invalidfield, value); }
        }

        string datamissing;
        public string DataMissingLabel
        {
            get { return datamissing; }
            set { SetField(ref datamissing, value); }
        }

        bool isselected = false;
        public bool IsSelected
        {
            get { return isselected; }
            set { SetField(ref isselected, value); }
        }

        int scrolltolast = 0;
        public int ScrollToIndex
        {
            get { return scrolltolast; }
            set { SetField(ref scrolltolast, value); }
        }

        private void GetApplications()
        {
            Applications = DatabaseQueries.GetApplications();           
            Applications.ItemPropertyChanged += Applications_ItemPropertyChanged;
        }

        private void Applications_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = Applications.Where(x => x.IsChecked).Count() > 0;
        }

        private void CheckValidation()
        {
            
            bool NameRequired = IsNameMissing();
            bool DuplicateName = IsDuplicateName();
            InvalidField = (DuplicateName || NameRequired);

            if (NameRequired)
                DataMissingLabel = "Name Missing";
            else
            if (DuplicateName)
                DataMissingLabel = "Duplicate Name";
        }

        private bool IsDuplicateName()
        {
            var query = Applications.GroupBy(x => x.Name.Trim().ToUpper() + "-" + x.IndustryID.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsNameMissing()
        {
            int nummissing = Applications.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (nummissing > 0);
        }

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            if (InvalidField)
                return false;
            return canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            Applications.Add(new ApplicationModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                IndustryID = 0,
                IsChecked = false,
                IsEnabled = true
            });
            ScrollToIndex = Applications.Count() - 1;
            CheckValidation();
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        ICommand delete;
        public ICommand Delete
        {
            get
            {
                if (delete == null)
                    delete = new DelegateCommand(CanExecuteDelete, ExecuteDelete);
                return delete;
            }
        }

        private bool CanExecuteDelete(object obj)
        {
            if (IsSelected)
                return true;
            return false;
        }

        Collection<ApplicationModel> deleteditems = new Collection<ApplicationModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Application Category";
            string confirmtxt = "Do you want to delete the selected item";
            if (Applications.Where(x => x.IsChecked).Count() > 1)
            {
                title = "Deleting Application Categories";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (ApplicationModel si in Applications)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteApplication(si.ID);
                        deleteditems.Add(si);
                    }
                }
                foreach (ApplicationModel pm in deleteditems)
                {
                    Applications.Remove(pm);
                }
                deleteditems.Clear();
                CheckValidation();
            }
            msg = null;
        }
               

        //save
        private bool CanExecuteSave(object obj)
        {
            if (InvalidField)
                return false;
            if (isdirty)
                return true;
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveAll();
        }

        private void SaveAll()
        {
            if (isdirty)
            {
                foreach (ApplicationModel am in Applications)
                {
                    if (am.ID == 0)
                        am.ID = AddApplication(am);
                    else
                        UpdateApplication(am);
                }
                isdirty = false;
            }
        }

        private void ExecuteClosing(object parameter)
        {
        }


        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {
                if (!InvalidField)
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveAll();
                        return true;
                    }
                    else                                            
                        return true;                    
                }
                else
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes with errors. Do you want to correct and then save these?", "Unsaved Changes with Errors", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                        return false;
                    else                                         
                        return true;                    
                }
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

        
        #endregion
    }
}
