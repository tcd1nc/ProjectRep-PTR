using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace PTR.ViewModels
{
    public class BUViewModel : ViewModelBase
    {
        FullyObservableCollection<ModelBaseVM> bus;
        ModelBaseVM bu;
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }


        bool isdirty = false;
        public BUViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            BUs = GetBusinessUnits();
            BUs.ItemPropertyChanged += BUs_ItemPropertyChanged;
                     
            bu = new ModelBaseVM();           
        }

        #region Event Handlers

        private void BUs_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = BUs.Where(x => x.IsChecked).Count() > 0;
        }

        #endregion

        #region Properties

        public FullyObservableCollection<ModelBaseVM> BUs
        {
            get { return bus; }
            set { SetField(ref bus, value); }
        }

        public ModelBaseVM BU
        {
            get { return bu; }
            set {
                if (value != null)
                    SetField(ref bu, value); }
        }

        ModelBaseVM _selectedsalesdivision;
        public ModelBaseVM SelectedSalesDivision
        {
            get { return _selectedsalesdivision; }
            set { SetField(ref _selectedsalesdivision, value); }
        }

        bool duplicatename;
        public bool DuplicateName
        {
            get { return duplicatename; }
            set { SetField(ref duplicatename, value); }
        }

        bool isselected = false;
        public bool IsSelected
        {
            get { return isselected; }
            set { SetField(ref isselected, value); }
        }

        #endregion

        #region Validation

        private void CheckValidation()
        {
            BUNameRequired = BUNameMissing();
            DuplicateName = IsDuplicateName();
          
            InvalidField = (DuplicateName || BUNameRequired);

            if (BUNameRequired)
                DataMissingLabel = "Business Unit Name Missing";
            else
                if (DuplicateName)
                DataMissingLabel = "Duplicate Business Unit Name";
            
            canexecuteadd = !InvalidField;
            canexecutesave = !InvalidField;
        }

        bool BUNameRequired;
       

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

        private bool IsDuplicateName()
        {
           bool _isduplicate = false;

           var query = bus.GroupBy(x => x.Name.ToUpper())
          .Where(g => g.Count() > 1)
          .Select(y => y.Key)
          .ToList();
            if (query.Count > 0)
                return true;

            return _isduplicate;
        }


        private bool BUNameMissing()
        {
            int nummissing = BUs.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (nummissing > 0);
        }

        #endregion


        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return canexecuteadd;
        }

        private void ExecuteAddNew(object obj)
        {
            BUs.Add(new ModelBaseVM()
            {
                ID = 0,
                Name = string.Empty
            });
            canexecutesave = false;
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        //save
        private bool CanExecuteSave(object obj)
        {
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveAll();
            CloseWindow();
        }              
                
        private void SaveAll()
        {
            foreach (ModelBaseVM ms in BUs)
            {
                if (!string.IsNullOrEmpty(ms.Name))
                {
                    if (ms.ID == 0)
                       ms.ID = AddBU(ms);
                    else
                       UpdateBU(ms);
                }
            }
            canexecutesave = false;
            isdirty = false;
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

        Collection<ModelBaseVM> deleteditems = new Collection<ModelBaseVM>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Business Unit";
            string confirmtxt = "Do you want to delete the selected item";
            if (BUs.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (ModelBaseVM si in BUs)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteBusinessUnit(si.ID);
                        deleteditems.Add(si);
                    }
                }
                foreach (ModelBaseVM pm in deleteditems)
                {
                    BUs.Remove(pm);
                }
                deleteditems.Clear();
                CheckValidation();
            }
            msg = null;
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
