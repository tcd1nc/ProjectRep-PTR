using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class NewBusinessCategoriesViewModel : ViewModelBase
    {
        bool isdirty = false;
        FullyObservableCollection<ModelBaseVM> newbizcats = new FullyObservableCollection<ModelBaseVM>();

        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        public NewBusinessCategoriesViewModel()
        {        
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetNewBusinessCategories();
            canexecutesave = false;
        
        }

        public FullyObservableCollection<ModelBaseVM> NewBusinessCategories
        {
            get { return newbizcats; }
            set { SetField(ref newbizcats, value); }
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

        private void GetNewBusinessCategories()
        {
            NewBusinessCategories = DatabaseQueries.GetNewBusinessCategories();
            NewBusinessCategories.ItemPropertyChanged += NewBusinessCategories_ItemPropertyChanged;
        }

        private void NewBusinessCategories_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = NewBusinessCategories.Where(x => x.IsChecked).Count() > 0;
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
            var query = NewBusinessCategories.GroupBy(x => x.Name.Trim().ToUpper())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsNameMissing()
        {
            int nummissing = NewBusinessCategories.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
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
            NewBusinessCategories.Add(new ModelBaseVM() {Name = string.Empty, IsEnabled = true, IsChecked = false });
            ScrollToIndex = NewBusinessCategories.Count() - 1;
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

        bool canexecutedelete = false;
        private bool CanExecuteDelete(object obj)
        {
            if (IsSelected)
                return true;
            return canexecutedelete;
        }

        Collection<ModelBaseVM> deleteditems = new Collection<ModelBaseVM>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting New Business Category";
            string confirmtxt = "Do you want to delete the selected item";
            if (NewBusinessCategories.Where(x => x.IsChecked).Count() > 1)
            {
                title = "Deleting New Business Categories";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (ModelBaseVM si in NewBusinessCategories)
                {
                    if (si.IsChecked)
                    {                                                   
                        if(si.ID > 0)
                            DeleteNewBusinessCategory(si.ID);
                        deleteditems.Add(si);
                    }
                }
               
                foreach (ModelBaseVM pm in deleteditems)
                {
                    NewBusinessCategories.Remove(pm);
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
                foreach (ModelBaseVM item in NewBusinessCategories)
                {
                    if (!string.IsNullOrEmpty(item.Name))
                    {
                        if (item.ID == 0)
                            item.ID = AddNewBusinessCategory(item);
                        else
                            UpdateNewBusinessCategory(item);
                    }
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
