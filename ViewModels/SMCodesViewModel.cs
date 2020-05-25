using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class SMCodesViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }
        bool isdirty = false;
        FullyObservableCollection<SMCodeModel> smcodes = new FullyObservableCollection<SMCodeModel>();

        public SMCodesViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetSalesDivisionList();
            GetSMCodes();
            canexecutesave = false;
        }

        public FullyObservableCollection<SMCodeModel> SMCodes
        {
            get { return smcodes; }
            set { SetField(ref smcodes, value); }
        }

        public FullyObservableCollection<ModelBaseVM> BusinessUnits { get; private set; }

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

        private void GetSalesDivisionList()
        {
            BusinessUnits = GetBusinessUnits();
        }
        
        private void GetSMCodes()
        {
            FullyObservableCollection<SMCodeModel> smcodes = DatabaseQueries.GetSMCodes();
            SMCodes?.Clear();
            foreach(SMCodeModel sm in smcodes)
            {
                if (sm.ID > 0)
                    SMCodes.Add(sm);
            }            
            SMCodes.ItemPropertyChanged += SMCodes_ItemPropertyChanged;
        }

        private void SMCodes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = SMCodes.Where(x => x.IsChecked).Count() > 0;
        }

        private void CheckValidation()
        {
            bool NameRequired = IsNameMissing();
            bool DuplicateName = IsDuplicateName();
            bool DescriptionMissing = IsDescriptionMissing();
            bool IndustryMissing = IsIndustryMissing();
            InvalidField = (DuplicateName || NameRequired || DescriptionMissing || IndustryMissing);

            if (NameRequired)
                DataMissingLabel = "Name Missing";
            else
            if (DuplicateName)
                DataMissingLabel = "Duplicate Name";
            else
            if (DescriptionMissing)
                DataMissingLabel = "Description Missing";
            else
            if (IndustryMissing)
                DataMissingLabel = "Industry Missing";
        }

        private bool IsDuplicateName()
        {
            var query = SMCodes.GroupBy(x => x.Name.Trim().ToUpper() + "-" + x.IndustryID.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsNameMissing()
        {
            int nummissing = SMCodes.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (nummissing > 0);
        }

        private bool IsDescriptionMissing()
        {
            int nummissing = SMCodes.Where(x => string.IsNullOrEmpty(x.Description.Trim())).Count();
            return (nummissing > 0);
        }

        private bool IsIndustryMissing()
        {
            int nummissing = SMCodes.Where(x => x.IndustryID ==0).Count();
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
            SMCodes.Add(new SMCodeModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                IndustryID = 0,
                IsChecked = false,
                IsEnabled = true
            });

            ScrollToIndex = SMCodes.Count()-1;
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

        Collection<SMCodeModel> deleteditems = new Collection<SMCodeModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Strategic Move Code";
            string confirmtxt = "Do you want to delete the selected item";
            if (SMCodes.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }

            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (SMCodeModel si in SMCodes)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteSMCode(si.ID);
                        deleteditems.Add(si);
                    }
                }

                foreach (SMCodeModel pm in deleteditems)
                {
                    SMCodes.Remove(pm);
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
                foreach (SMCodeModel am in SMCodes)
                {
                    if (am.ID == 0)
                        am.ID = AddSMCode(am);
                    else
                        UpdateSMCode(am);
                }
                isdirty = false;
            }
        }

        private void ExecuteClosing(object parameter)
        {
            //if (isdirty && !InvalidField)
            //{
            //    IMessageBoxService msg = new MessageBoxService();
            //    GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
            //    msg = null;
            //    if (result.Equals(GenericMessageBoxResult.Yes))
            //        SaveAll();
            //}
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
