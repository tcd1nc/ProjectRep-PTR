using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class MarketSegmentsApplicationCatsViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }
        bool isdirty = false;
        FullyObservableCollection<MarketSegmentApplicationCategoryJoinModel> marketsegmentapplicationcategories = new FullyObservableCollection<MarketSegmentApplicationCategoryJoinModel>();

        FullyObservableCollection<MarketSegmentModel> marketsegments = new FullyObservableCollection<MarketSegmentModel>();
        FullyObservableCollection<ApplicationCategoriesModel> appcategories = new FullyObservableCollection<ApplicationCategoriesModel>();

        public MarketSegmentsApplicationCatsViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetMarketSegments();
            GetApplicationCategoriesMasterList();
            GetMarketSegmentApplicationCategories();

            canexecutesave = false;
        }

        public FullyObservableCollection<MarketSegmentApplicationCategoryJoinModel> MarketSegmentApplicationCategories
        {
            get { return marketsegmentapplicationcategories; }
            set { SetField(ref marketsegmentapplicationcategories, value); }
        }

        public FullyObservableCollection<MarketSegmentModel> MarketSegments
        {
            get { return marketsegments; }
            set { SetField(ref marketsegments, value); }
        }

        public FullyObservableCollection<ApplicationCategoriesModel> ApplicationCategories
        {
            get { return appcategories; }
            set { SetField(ref appcategories, value); }
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

        private void GetSalesDivisionList()
        {
            SalesDivisions = GetSalesDivisions();
        }

        private void GetMarketSegments()
        {
            MarketSegments = DatabaseQueries.GetMarketSegments();
        }

        private void GetApplicationCategoriesMasterList()
        {
            ApplicationCategories = GetApplicationCategories();
        }

        private void GetMarketSegmentApplicationCategories()
        {
            MarketSegmentApplicationCategories = GetMarketSegmentApplicationCategoriesJoinCRUD();
            MarketSegmentApplicationCategories.ItemPropertyChanged += MarketSegmentApplicationCategories_ItemPropertyChanged;
        }

        private void MarketSegmentApplicationCategories_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = MarketSegmentApplicationCategories.Where(x => x.IsChecked).Count() > 0;
        }

        private void CheckValidation()
        {            
            bool MarketSegmentMissing = IsMarketSegmentMissing();
            bool ApplicationCategoryMissing = IsApplicationCategoryMissing();
            InvalidField = (MarketSegmentMissing || ApplicationCategoryMissing);
            
            if (MarketSegmentMissing)
                DataMissingLabel = "Market Segment Missing";
            else
            if (ApplicationCategoryMissing)
                DataMissingLabel = "Application Category Missing";
        }

        private bool IsDuplicateName()
        {
            var query = MarketSegmentApplicationCategories.GroupBy(x => x.MarketSegmentID.ToString() + "-" + x.ApplicationCategoryID.ToString())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsMarketSegmentMissing()
        {
            int nummissing = MarketSegmentApplicationCategories.Where(x => x.MarketSegmentID == 0).Count();
            return (nummissing > 0);
        }

        private bool IsApplicationCategoryMissing()
        {
            int nummissing = MarketSegmentApplicationCategories.Where(x => x.ApplicationCategoryID == 0).Count();
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
            MarketSegmentApplicationCategories.Add(new MarketSegmentApplicationCategoryJoinModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                MarketSegmentID = 0,
                ApplicationCategoryID = 0,
                IsChecked = false,
                IsEnabled = true
            });

            ScrollToIndex = MarketSegmentApplicationCategories.Count()-1;
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

        Collection<MarketSegmentApplicationCategoryJoinModel> deleteditems = new Collection<MarketSegmentApplicationCategoryJoinModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Market Segment - Application Category";
            string confirmtxt = "Do you want to delete the selected item";
            if (MarketSegmentApplicationCategories.Where(x => x.IsChecked).Count() > 1)
            {
                title = "Deleting Market Segment - Application Categories";
                confirmtxt = confirmtxt + "s";
            }

            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (MarketSegmentApplicationCategoryJoinModel si in MarketSegmentApplicationCategories)
                {
                    if (si.IsChecked)
                    {
                        if (si.ID > 0)
                            DeleteMarketSegmentApplicationCategory(si.ID);
                        deleteditems.Add(si);
                    }
                }

                foreach (MarketSegmentApplicationCategoryJoinModel pm in deleteditems)
                {
                    MarketSegmentApplicationCategories.Remove(pm);
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
                foreach (MarketSegmentApplicationCategoryJoinModel am in MarketSegmentApplicationCategories)
                {
                    if (am.ID == 0)
                        am.ID = AddMarketSegmentApplicationCategory(am);
                    else
                        UpdateMarketSegmentApplicationCategory(am);
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
