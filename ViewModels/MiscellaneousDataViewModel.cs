using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class MiscellaneousDataViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }
        bool isdirty = false;
        FullyObservableCollection<MiscellaneousDataModel> miscellaneousdata = new FullyObservableCollection<MiscellaneousDataModel>();
        FullyObservableCollection<ProjectTypeModel> projecttypes = new FullyObservableCollection<ProjectTypeModel>();
        
        public MiscellaneousDataViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetMiscellaneousData();
            GetProjectTypes();
            
            canexecutesave = false;
        }

        public FullyObservableCollection<MiscellaneousDataModel> MiscellaneousData
        {
            get { return miscellaneousdata; }
            set { SetField(ref miscellaneousdata, value); }
        }

        public FullyObservableCollection<ProjectTypeModel> ProjectTypes
        {
            get { return projecttypes; }
            set { SetField(ref projecttypes, value); }
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

        private void GetMiscellaneousData()
        {
            MiscellaneousData = DatabaseQueries.GetMiscellaneousData();
            MiscellaneousData.ItemPropertyChanged += MiscellaneousData_ItemPropertyChanged;
        }

        private void MiscellaneousData_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = MiscellaneousData.Where(x => x.IsChecked).Count() > 0;
        }

        private void GetProjectTypes()
        {
            ProjectTypes = DatabaseQueries.GetProjectTypes();          
        }

        private void CheckValidation()
        {            
            bool ProjectTypeMissing = IsProjectTypeMissing();
            bool LabelMissing = IsLabelMissing();
            bool DuplicateName = IsDuplicateName();
            InvalidField = (DuplicateName || ProjectTypeMissing || LabelMissing);

            if (DuplicateName)
                DataMissingLabel = "Duplicate Label";
            else
           if (ProjectTypeMissing)
                DataMissingLabel = "Project Type Missing";
            else
            if (LabelMissing)
                DataMissingLabel = "Label Missing";
        }

        private bool IsDuplicateName()
        {
            var query = MiscellaneousData.GroupBy(x => x.FKID.ToString() + "-" + x.Name)
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsProjectTypeMissing()
        {
            int nummissing = MiscellaneousData.Where(x => x.FKID == 0).Count();
            return (nummissing > 0);
        }

        private bool IsLabelMissing()
        {
            int nummissing = MiscellaneousData.Where(x => string.IsNullOrEmpty(x.Name)).Count();
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
            MiscellaneousData.Add(new MiscellaneousDataModel()
            {
                ID = 0,
                FKID = 0,
                Name = string.Empty,
                IsChecked = false,
                IsEnabled = true
            });

            ScrollToIndex = MiscellaneousData.Count()-1;
            CheckValidation();
        }


        int scrolltolast=0;
        public int ScrollToIndex
        {
            get { return scrolltolast; }
            set { SetField(ref scrolltolast, value); }
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

        Collection<MiscellaneousDataModel> deleteditems = new Collection<MiscellaneousDataModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Miscellaneous Data";
            string confirmtxt = "Do you want to delete the selected item";
            if (MiscellaneousData.Where(x => x.IsChecked).Count() > 1)
            {
                confirmtxt = confirmtxt + "s";
            }

            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (MiscellaneousDataModel si in MiscellaneousData)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteMiscellaneousData(si.ID);
                        deleteditems.Add(si);
                    }
                }

                foreach (MiscellaneousDataModel pm in deleteditems)
                {
                    MiscellaneousData.Remove(pm);
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
                foreach (MiscellaneousDataModel am in MiscellaneousData)
                {
                    if (am.ID == 0)
                        am.ID = AddMiscellaneousData(am);
                    else
                        UpdateMiscellaneousData(am);
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
