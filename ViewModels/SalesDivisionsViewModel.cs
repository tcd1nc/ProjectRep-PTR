using System.Linq;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class SalesDivisionsViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<Models.SalesDivisionModel> _salesdivisions;
        Models.SalesDivisionModel _salesdivision;
        bool _isdirty = false;
        public SalesDivisionsViewModel()
        {
            _salesdivisions = new FullyObservableCollection<Models.SalesDivisionModel>();
            _salesdivisions = GetSalesDivisions();

            SalesDivisions.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;
            //populate from database

            SaveAndClose = new RelayCommand(ExecuteSaveAndClose, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            _salesdivision = new Models.SalesDivisionModel();
           
        }

        private void SalesDivisions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
             _isdirty = true;
            if (e.PropertyName == "Name")
            {
                DuplicateName = IsDuplicateName();
            }
        }


        public FullyObservableCollection<Models.SalesDivisionModel> SalesDivisions
        {
            get { return _salesdivisions; }
            set { SetField(ref _salesdivisions, value); }
        }

        public Models.SalesDivisionModel SalesDivision
        {
            get { return _salesdivision; }
            set {
                if (value != null)
                    SetField(ref _salesdivision, value); }
        }

        Models.SalesDivisionModel _selectedsalesdivision;
        public Models.SalesDivisionModel SelectedSalesDivision
        {
            get { return _selectedsalesdivision; }
            set { SetField(ref _selectedsalesdivision, value); }
        }

       private bool IsDuplicateName()
        {
           bool _isduplicate = false;

           var query = _salesdivisions.GroupBy(x => x.GOM.Name.ToUpper())
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
            _canexecuteadd = false;
            SalesDivisions.Add(new Models.SalesDivisionModel() { GOM = new Models.GenericObjModel() });
            SalesDivision = SalesDivisions[SalesDivisions.Count - 1];
            SalesDivisions.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;
            ScrollToSelectedItem = SalesDivisions.Count - 1;
        }

       
        //save
        
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
            foreach (Models.SalesDivisionModel ms in SalesDivisions)
            {
                if (!string.IsNullOrEmpty(ms.GOM.Name))
                {
                    if (ms.GOM.ID == 0)
                        AddSalesDivision(ms);
                    else
                        UpdateSalesDivision(ms);
                }
            }
            CloseWindow();
        }

        #endregion

    }
}
