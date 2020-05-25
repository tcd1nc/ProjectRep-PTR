using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class IndustrySegmentsApplicationsViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }
        bool isdirty = false;
        FullyObservableCollection<IndustrySegmentApplicationJoinModel> industrysegmentapplications = new FullyObservableCollection<IndustrySegmentApplicationJoinModel>();

        FullyObservableCollection<IndustrySegmentModel> industrysegments = new FullyObservableCollection<IndustrySegmentModel>();
        FullyObservableCollection<ApplicationModel> appcategories = new FullyObservableCollection<ApplicationModel>();

        public IndustrySegmentsApplicationsViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetIndustrySegments();
            GetApplicationsMasterList();
            GetIndustrySegmentApplications();

            canexecutesave = false;
        }

        public FullyObservableCollection<IndustrySegmentApplicationJoinModel> IndustrySegmentApplications
        {
            get { return industrysegmentapplications; }
            set { SetField(ref industrysegmentapplications, value); }
        }

        public FullyObservableCollection<IndustrySegmentModel> IndustrySegments
        {
            get { return industrysegments; }
            set { SetField(ref industrysegments, value); }
        }

        public FullyObservableCollection<ApplicationModel> Applications
        {
            get { return appcategories; }
            set { SetField(ref appcategories, value); }
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

        private void GetIndustrySegments()
        {
            IndustrySegments = DatabaseQueries.GetIndustrySegments();
        }

        private void GetApplicationsMasterList()
        {
            Applications = GetApplications();
        }

        private void GetIndustrySegmentApplications()
        {
            IndustrySegmentApplications = GetIndustrySegmentApplicationJoinCRUD();
            IndustrySegmentApplications.ItemPropertyChanged += IndustrySegmentApplications_ItemPropertyChanged;
        }

        private void IndustrySegmentApplications_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = IndustrySegmentApplications.Where(x => x.IsChecked).Count() > 0;
        }

        private void CheckValidation()
        {            
            bool IndustrySegmentMissing = IsIndustrySegmentMissing();
            bool ApplicationMissing = IsApplicationMissing();
            InvalidField = (IndustrySegmentMissing || ApplicationMissing);
            
            if (IndustrySegmentMissing)
                DataMissingLabel = "Industry Segment Missing";
            else
            if (ApplicationMissing)
                DataMissingLabel = "Application Missing";
        }

        private bool IsDuplicateName()
        {
            var query = IndustrySegmentApplications.GroupBy(x => x.IndustrySegmentID.ToString() + "-" + x.ApplicationID.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsIndustrySegmentMissing()
        {
            int nummissing = IndustrySegmentApplications.Where(x => x.IndustrySegmentID == 0).Count();
            return (nummissing > 0);
        }

        private bool IsApplicationMissing()
        {
            int nummissing = IndustrySegmentApplications.Where(x => x.ApplicationID == 0).Count();
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
            IndustrySegmentApplications.Add(new IndustrySegmentApplicationJoinModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                IndustrySegmentID = 0,
                ApplicationID = 0,
                IsChecked = false,
                IsEnabled = true
            });

            ScrollToIndex = IndustrySegmentApplications.Count()-1;
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

        Collection<IndustrySegmentApplicationJoinModel> deleteditems = new Collection<IndustrySegmentApplicationJoinModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Industry Segment - Application";
            string confirmtxt = "Do you want to delete the selected item";
            if (IndustrySegmentApplications.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }

            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (IndustrySegmentApplicationJoinModel si in IndustrySegmentApplications)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteIndustrySegmentApplication(si.ID);
                        deleteditems.Add(si);
                    }
                }

                foreach (IndustrySegmentApplicationJoinModel pm in deleteditems)
                {
                    IndustrySegmentApplications.Remove(pm);
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
                foreach (IndustrySegmentApplicationJoinModel am in IndustrySegmentApplications)
                {
                    if (am.ID == 0)
                        am.ID = AddIndustrySegmentApplication(am);
                    else
                        UpdateIndustrySegmentApplication(am);
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
