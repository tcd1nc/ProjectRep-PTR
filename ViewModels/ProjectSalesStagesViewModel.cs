using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class ProjectSalesStagesViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;
        FullyObservableCollection<ActivityStatusCodesModel> activitycodes = new FullyObservableCollection<ActivityStatusCodesModel>();

        public ProjectSalesStagesViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetActivitiesCodes();
            canexecutesave = false;
            Colours = GetColours();
        }

        public Collection<string> Colours { get; private set; }

        public FullyObservableCollection<ActivityStatusCodesModel> ActivityCodes
        {
            get { return activitycodes; }
            set { SetField(ref activitycodes, value); }
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

        private void GetActivitiesCodes()
        {
            Collection<ActivityStatusCodesModel> ActivityStatusCodes = GetActivityStatusCodes();
            foreach (ActivityStatusCodesModel am in ActivityStatusCodes)
            {
                ActivityStatusCodesModel newc = new ActivityStatusCodesModel()
                {
                    ID = am.ID,
                    Name = am.Name,
                    Description = am.Description,
                    Colour = am.Colour,
                    PlaybookDescription = am.PlaybookDescription
                };
                ActivityCodes.Add(newc);
            }
            ActivityCodes.ItemPropertyChanged += ActivityCodes_ItemPropertyChanged;
        }

        private void ActivityCodes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            CheckValidation();
            isdirty = true;
        }

        private void CheckValidation()
        {            
            bool StatusRequired = IsStatusMissing();
            bool ColourRequired = IsColourMissing();
            bool DuplicateStatus = IsDuplicateStatus();
            bool DescriptionRequired = DescriptionMissing();

            InvalidField = (DuplicateStatus || StatusRequired || ColourRequired || DescriptionRequired);

            if (StatusRequired)
                DataMissingLabel = "Status Missing";
            else
            if (DuplicateStatus)
                DataMissingLabel = "Duplicate Status";
            else
            if (ColourRequired)
                DataMissingLabel = "Colour Missing";
            else
            if (DescriptionRequired)
                DataMissingLabel = "Description Missing";
        }

        private bool IsDuplicateStatus()
        {
            var query = ActivityCodes.GroupBy(x => x.Name.Trim().ToUpper())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsStatusMissing()
        {
            int nummissing = ActivityCodes.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (nummissing > 0);
        }

        private bool DescriptionMissing()
        {
            int nummissing = ActivityCodes.Where(x => string.IsNullOrEmpty(x.Description.Trim())).Count();
            return (nummissing > 0);
        }

        private bool IsColourMissing()
        {
            int nummissing = ActivityCodes.Where(x => string.IsNullOrEmpty(x.Colour.Trim())).Count();
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
            ActivityCodes.Add(new ActivityStatusCodesModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                Colour = new SolidColorBrush(Colors.AliceBlue).ToString(),
                PlaybookDescription = string.Empty
            });
            CheckValidation();
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
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
                foreach (ActivityStatusCodesModel am in ActivityCodes)
                {
                    //if (am.ID == 0)
                    //    am.ID = AddActivityStatusCode(am);
                    //else
                        UpdateActivityStatusCode(am);
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
