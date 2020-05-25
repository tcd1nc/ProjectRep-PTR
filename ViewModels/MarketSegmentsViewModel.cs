using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class MarketSegmentsViewModel : ViewModelBase
    {
        bool isdirty = false;
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        FullyObservableCollection<MarketSegmentModel> mktsegs = new FullyObservableCollection<MarketSegmentModel>();

        public MarketSegmentsViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetSalesDivisionList();
            GetMarketSegmentsList();
            canexecutesave = false;
        }

        public FullyObservableCollection<MarketSegmentModel> MarketSegments
        {
            get { return mktsegs; }
            set { SetField(ref mktsegs, value); }
        }

        public FullyObservableCollection<ModelBaseVM> SalesDivisions { get; private set; }

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

        private void GetSalesDivisionList()
        {
            SalesDivisions = GetSalesDivisions();
        }


        private void GetMarketSegmentsList()
        {
            MarketSegments = DatabaseQueries.GetMarketSegments();
            MarketSegments.ItemPropertyChanged += ApplicationCategories_ItemPropertyChanged;
        }

        private void ApplicationCategories_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = MarketSegments.Where(x => x.IsChecked).Count() > 0;
        }

        private void CheckValidation()
        {
            
            bool NameRequired = IsNameMissing();
            bool DuplicateName = IsDuplicateName();
            bool IndustryMissing = IsIndustryMissing();
            InvalidField = (DuplicateName || NameRequired || IndustryMissing);

            if (NameRequired)
                DataMissingLabel = "Name Missing";
            else
            if (DuplicateName)
                DataMissingLabel = "Duplicate Name";
            else
            if (IndustryMissing)
                DataMissingLabel = "Industry Missing";
        }

        private bool IsDuplicateName()
        {
            var query = MarketSegments.GroupBy(x => x.Name.Trim().ToUpper() + "-" + x.IndustryID.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsNameMissing()
        {
            int nummissing = MarketSegments.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (nummissing > 0);
        }

        private bool IsIndustryMissing()
        {
            int nummissing = MarketSegments.Where(x => x.IndustryID == 0).Count();
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
            MarketSegments.Add(new MarketSegmentModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                IndustryID = 0,
                IsChecked = false,
                IsEnabled = true
            });
            ScrollToIndex = MarketSegments.Count() - 1;
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

        Collection<MarketSegmentModel> deleteditems = new Collection<MarketSegmentModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Market Segment";
            string confirmtxt = "Do you want to delete the selected item";
            if (MarketSegments.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (MarketSegmentModel si in MarketSegments)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteMarketSegment(si.ID);
                        deleteditems.Add(si);
                    }
                }
                foreach (MarketSegmentModel pm in deleteditems)
                {
                    MarketSegments.Remove(pm);
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
                foreach (MarketSegmentModel am in MarketSegments)
                {
                    if (am.ID == 0)
                        am.ID = AddMarketSegment(am);
                    else
                        UpdateMarketSegment(am);
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
