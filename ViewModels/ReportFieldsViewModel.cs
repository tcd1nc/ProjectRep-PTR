using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class ReportFieldsViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }
        bool isdirty = false;
        FullyObservableCollection<ReportFields> reportfields = new FullyObservableCollection<ReportFields>();
        FullyObservableCollection<ModelBaseVM> reportnames = new FullyObservableCollection<ModelBaseVM>();
        FullyObservableCollection<ModelBaseVM> alignmenttypes = new FullyObservableCollection<ModelBaseVM>();

        public FullyObservableCollection<ReportFields> ReportFields { get { return reportfields; } set { SetField(ref reportfields, value); }}
        public FullyObservableCollection<ModelBaseVM> ReportNames { get { return reportnames; }set { SetField(ref reportnames, value); } }
        public FullyObservableCollection<ModelBaseVM> AlignmentTypes { get { return alignmenttypes; } set { SetField(ref alignmenttypes, value); } }
        public Collection<ReportFieldDataTypeModel> FieldTypes { get; set; } = new Collection<ReportFieldDataTypeModel>();
        public Collection<ReportFieldsDataTypesModel> DataTypes { get; set; }

        public ReportFieldsViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            
            GetReportNames();

            if (ReportNames.Count > 0)            
                SelectedReportName = ReportNames[0];
            
            GetAlignmentTypes();
            GetDataTypes();
            GetFieldTypes();
            canexecutesave = false;
        }

        ModelBaseVM selectedreportname;
        public ModelBaseVM SelectedReportName
        {
            get { return selectedreportname; }
            set { SetField(ref selectedreportname, value);
                if (value != null)
                    GetReportFields(value.Name);
            }
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

        private void GetReportFields(string reportname)
        {
            ReportFields = DatabaseQueries.GetReportFields(reportname);
            ReportFields.ItemPropertyChanged += ReportFields_ItemPropertyChanged;
        }

        private void ReportFields_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
                ReportFields[e.CollectionIndex].IsSelected = true;
            }
            IsSelected = ReportFields.Where(x => x.IsChecked).Count() > 0;
        }

        private void GetAlignmentTypes()
        {
            AlignmentTypes = DatabaseQueries.GetReportFieldsAlignmentTypes();
        }

        private void GetDataTypes()
        {
            DataTypes = GetReportFieldsDataTypes();
        }

        private void GetFieldTypes()
        {
            FieldTypes = GetReportFieldTypes();
        }

        private void GetReportNames()
        {
            ReportNames = DatabaseQueries.GetReportNames();
        }

        private void CheckValidation()
        {
            bool DuplicateCaption = IsDuplicateCaption();
            bool DuplicateColumnName = IsDuplicateColumnName();
            bool CaptionMissing = IsCaptionMissing();
            bool FieldNameMissing = IsFieldNameMissing();
            bool DataTypeMissing = IsDataTypeMissing();
            bool AlignmentMissing = IsAlignmentMissing();
            bool FieldTypeMissing = IsFieldTypeMissing();
            InvalidField = (DuplicateCaption || DuplicateColumnName || CaptionMissing || FieldNameMissing || DataTypeMissing || FieldTypeMissing || AlignmentMissing);
                        
            if (CaptionMissing)
                DataMissingLabel = "Header Missing";
            else
            if (DuplicateCaption)
                DataMissingLabel = "Duplicate Header";
            else
            if (FieldNameMissing)
                DataMissingLabel = "Missing Column Name";
            else
            if (DuplicateColumnName)
                DataMissingLabel = "Duplicate Column Name";
            else
            if (DataTypeMissing)
                DataMissingLabel = "Data Type Missing";
            else
             if (FieldTypeMissing)
                DataMissingLabel = "Field Type Missing";
            else
            if (AlignmentMissing)
                DataMissingLabel = "Alignment Missing";
          
        }

        private bool IsDuplicateCaption()
        {
            var query = ReportFields.GroupBy(x => x.Caption.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsDuplicateColumnName()
        {
            var query = ReportFields.GroupBy(x => x.FieldName.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsCaptionMissing()
        {
            int nummissing = ReportFields.Where(x => string.IsNullOrEmpty(x.Caption)).Count();
            return (nummissing > 0);
        }

        private bool IsFieldNameMissing()
        {
            int nummissing = ReportFields.Where(x => string.IsNullOrEmpty(x.FieldName)).Count();
            return (nummissing > 0);
        }

        private bool IsDataTypeMissing()
        {
            int nummissing = ReportFields.Where(x => x.DataTypeID == 0).Count();
            return (nummissing > 0);
        }

        private bool IsFieldTypeMissing()
        {
            int nummissing = ReportFields.Where(x => x.FieldType < -90).Count();
            return (nummissing > 0);
        }

        private bool IsAlignmentMissing()
        {
            int nummissing = ReportFields.Where(x => string.IsNullOrEmpty(x.Alignment)).Count();
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
            ReportFields.Add(new ReportFields()
            {
                ID = 0,
                Name = SelectedReportName.Name,
                Caption = string.Empty,
                Alignment = string.Empty,
                DataTypeID = 0,
                FieldName = string.Empty,
                System = false,
                FieldType = -999,                              
                IsChecked = false,
                IsEnabled = true               
            });

            ScrollToIndex = ReportFields.Count()-1;
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

        Collection<ReportFields> deleteditems = new Collection<ReportFields>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Report Field";
            string confirmtxt = "Do you want to delete the selected item";
            if (ReportFields.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }

            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (ReportFields si in ReportFields)
                {
                    if (si.IsSelected)
                    {
                        if (si.ID > 0)
                            DeleteReportField(si.ID);
                        deleteditems.Add(si);
                        si.IsSelected = false;
                    }                    
                }

                foreach (ReportFields pm in deleteditems)
                {
                    ReportFields.Remove(pm);
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
                foreach (ReportFields am in ReportFields)
                {
                    if (am.IsSelected == true)
                    {
                        if (am.ID == 0)
                            am.ID = AddReportField(am);
                        else
                            UpdateReportField(am);
                        am.IsSelected = false;
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
