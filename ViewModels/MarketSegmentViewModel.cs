using System.Linq;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class MarketSegmentViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<Models.MarketSegmentModel> _marketsegments;
        Models.MarketSegmentModel _marketsegment;
        FullyObservableCollection<Models.SalesDivisionModel> _salesdivisions;

        bool _isdirty = false;

        public MarketSegmentViewModel()
        {
            _marketsegments = new FullyObservableCollection<Models.MarketSegmentModel>();
            _marketsegments = GetMarketSegments();

            MarketSegments.ItemPropertyChanged += MarketSegments_ItemPropertyChanged;

            SaveAndClose = new RelayCommand(ExecuteSaveAndClose, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);


            //populate from database
            _salesdivisions = GetSalesDivisions();
            _marketsegment = new Models.MarketSegmentModel();
        }

        private void MarketSegments_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            _isdirty = true;
            if(e.PropertyName == "MarketSegment")
            {
                DuplicateName = IsDuplicateName();
            }
        }

        public FullyObservableCollection<Models.MarketSegmentModel> MarketSegments
        {
            get { return _marketsegments; }
            set { SetField(ref _marketsegments, value); }
        }

        public Models.MarketSegmentModel MarketSegment
        {
            get { return _marketsegment; }
            set {
                if (value != null)
                    SetField(ref _marketsegment, value); }
        }

        public FullyObservableCollection<Models.SalesDivisionModel> SalesDivisions
        {
            get { return _salesdivisions; }
            set { SetField(ref _salesdivisions, value); }
        }

        Models.MarketSegmentModel _selectedmarketsegment;
        public Models.MarketSegmentModel SelectedMarketSegment
        {
            get { return _selectedmarketsegment; }
            set { SetField(ref _selectedmarketsegment, value); }
        }
        
        private bool IsDuplicateName()
        {
            bool _isduplicate = false;
            var query = _marketsegments.GroupBy(x => x.GOM.Name.ToUpper())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            if (query.Count > 0)
                return true;

            return _isduplicate;
        }

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            if (DuplicateName)
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            MarketSegments.Add(new Models.MarketSegmentModel() {GOM = new Models.GenericObjModel(), IndustryID=0});
            MarketSegments.ItemPropertyChanged += MarketSegments_ItemPropertyChanged;
            ScrollToSelectedItem = MarketSegments.Count - 1;
        }

        
        private void ExecuteCancel(object parameter)
        {            
            CloseWindow();
        }

        private bool CanExecuteSave(object obj)
        {
            if (DuplicateName)
                return false;    

            if (!_isdirty)
                return false;

            return _canexecutesave;
        }        
        
        private void ExecuteSaveAndClose(object parameter)
        {
            foreach(Models.MarketSegmentModel ms in MarketSegments)
            {
                if (!string.IsNullOrEmpty(ms.GOM.Name))
                {
                    if (ms.GOM.ID == 0)                                            
                        AddNewMarketSegment(ms);                                            
                    else                
                        UpdateMarketSegment(ms);                
                }
            }                                   
            CloseWindow();
        }
              
        #endregion

    }
}
