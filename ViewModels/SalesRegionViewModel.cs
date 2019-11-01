using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;

namespace PTR.ViewModels
{
    public class SalesRegionViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<SalesRegionModel> salesregions;
        bool isloaded = false;
        bool isdirty = false;

        public SalesRegionViewModel()
        {
            ExCloseWindow = ExecuteClosing;
            countries = StaticCollections.Countries;
            if (Countries.Count > 0)
            {
                Country = Countries[0];                
                FilterSalesRegions(Countries[0].GOM.ID);
            }

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);          
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            
            canexecutedelete = false;
            isloaded = true;
        }

        #region Private functions

        private void FilterSalesRegions(int countryid)
        {
            if(SalesRegions !=null)
                SalesRegions.ItemPropertyChanged -= SalesRegions_ItemPropertyChanged;
            FullyObservableCollection<SalesRegionModel> tempsalesregions = GetCountrySalesRegions(countryid);            
            FullyObservableCollection<SalesRegionModel> newsalesregions = new FullyObservableCollection<SalesRegionModel>();
            foreach (SalesRegionModel sd in tempsalesregions)
            {
                SalesRegionModel sm = new SalesRegionModel
                {
                    ID = sd.ID,
                    CountryID = sd.CountryID,
                    Name = sd.Name,
                    IsEnabled = IsDeletableSalesRegion(sd.ID)
                };
                newsalesregions.Add(sm);
            }
            SalesRegions = newsalesregions;
            SalesRegions.ItemPropertyChanged += SalesRegions_ItemPropertyChanged;
            canexecutesave = false;
        }

        #endregion

        #region Event handlers

        private void SalesRegions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
           CheckFieldValidation();
           canexecutedelete = IsItemsSelected();
        }

        #endregion

        #region Properties

        public FullyObservableCollection<SalesRegionModel> SalesRegions
        {
            get { return salesregions; }
            set { SetField(ref salesregions, value); }
        }
        
        SalesRegionModel selectedregion;
        public SalesRegionModel SelectedSalesRegion
        {
            get { return selectedregion; }
            set { SetField(ref selectedregion, value); }
        }
                
        CountryModel selectedCountry;
        public CountryModel Country
        {
            get { return selectedCountry; }
            set {

                if (selectedCountry != null)
                {
                    if (!InvalidField && isloaded)                    
                        SaveSalesRegions();
                    
                    InvalidField = false;
                    canexecuteadd = true;
                }
                                      
                if (value != null)
                {                   
                    FilterSalesRegions(value.GOM.ID);              
                }
                SetField(ref selectedCountry, value);
            }
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
        }

        #endregion

        #region Validation
        
        private void CheckFieldValidation()
        {
            bool DuplicateSalesRegion = IsDuplicateSalesRegion();
            bool MissingSalesRegionName = IsMissingSalesRegion();
            isdirty = true;
            InvalidField = (DuplicateSalesRegion || MissingSalesRegionName);
               
            if (DuplicateSalesRegion)
                DataMissingLabel = "Duplicate Sales Region";
            else
                if(MissingSalesRegionName)
                    DataMissingLabel = "Missing Sales Region Name";

            canexecuteadd = !InvalidField;
            canexecutesave = !InvalidField;            
        }

        private bool IsDuplicateSalesRegion()
        {
            var query = salesregions.GroupBy(x => x.Name.Trim().ToUpper())
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
            return (query.Count > 0);                
        }

        private bool IsMissingSalesRegion()
        {
            int missing = SalesRegions.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (missing > 0);
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

        #endregion

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {           
            SalesRegions.Add(new SalesRegionModel()
            {
                ID = 0,
                Name = string.Empty,
                CountryID = Country.GOM.ID
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
            SaveSalesRegions();            
        }

        private void SaveSalesRegions()
        {
            if (isdirty)
            {
                foreach (SalesRegionModel em in SalesRegions)
                {
                    if (em.ID == 0)
                        em.ID = AddSalesRegion(em);
                    else
                        UpdateSalesRegion(em);
                }
                FilterSalesRegions(Country.GOM.ID);            
                isdirty = false;
            }
        }
       
        private bool CanExecuteDelete(object obj)
        {            
            return canexecutedelete;
        }

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if (msg.ShowMessage("Do you want to delete this sales region?", "Deleting Sales Region", GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach(SalesRegionModel sd in SalesRegions)
                {
                    if (sd.IsSelected)                    
                        DeleteSalesRegion(sd.ID);                                            
                }
                FilterSalesRegions(Country.GOM.ID);
            }
            msg = null;                     
            canexecutedelete = (salesregions.Count > 0);
            canexecutesave = false;
        }

        private bool IsDeletableSalesRegion(int id)
        {
            return (GetCountSalesRegionCustomers(id) == 0);                              
        }

        private bool IsItemsSelected()
        {
            foreach (SalesRegionModel sd in SalesRegions)
            {
                if (sd.IsSelected)
                    return true;
            }
            return false;
        }
             
        private void ExecuteClosing(object parameter)
        {
            SaveSalesRegions();
            if (SalesRegions != null)
                SalesRegions.ItemPropertyChanged -= SalesRegions_ItemPropertyChanged;
            //refresh sales regions list
            StaticCollections.SalesRegions = DatabaseQueries.GetSalesRegions();
        }


        #endregion

    }
}
