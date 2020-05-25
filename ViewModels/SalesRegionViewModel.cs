using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace PTR.ViewModels
{
    public class SalesRegionViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        bool canexecutedelete = false;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<SalesRegionModel> salesregions;

        bool isdirty = false;
        
        public SalesRegionViewModel()
        {
            countries = GetCountries();
            if (Countries.Count > 0)
            {
                Country = Countries[0];                
                FilterSalesRegions(Countries[0].ID);
            }

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            canexecutesave = false;
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
                    IsChecked = false,
                    IsEnabled = sd.IsEnabled
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
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = SalesRegions.Where(x => x.IsChecked).Count() > 0;
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
                    if (!InvalidField)                    
                        SaveAll();
                    
                    InvalidField = false;
                    canexecuteadd = true;
                }
                                      
                if (value != null)
                {                   
                    FilterSalesRegions(value.ID);              
                }
                SetField(ref selectedCountry, value);
            }
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
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
            bool DuplicateSalesRegion = IsDuplicateSalesRegion();
            bool MissingSalesRegionName = IsMissingSalesRegion();
            InvalidField = (DuplicateSalesRegion || MissingSalesRegionName);
               
            if (DuplicateSalesRegion)
                DataMissingLabel = "Duplicate Sales Region";
            else
                if(MissingSalesRegionName)
                    DataMissingLabel = "Missing Sales Region Name";          
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
            if(InvalidField)
                return false;
            return canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {           
            SalesRegions.Add(new SalesRegionModel()
            {
                ID = 0,
                Name = string.Empty,
                CountryID = Country.ID,
                IsEnabled = true,
                IsChecked = false
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
                foreach (SalesRegionModel em in SalesRegions)
                {
                    if (em.ID == 0)
                        em.ID = AddSalesRegion(em);
                    else
                        UpdateSalesRegion(em);                    
                }       
                isdirty = false;               
            }
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
            return canexecutedelete;
        }

        Collection<SalesRegionModel> deleteditems = new Collection<SalesRegionModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Sales Region";
            string confirmtxt = "Do you want to delete the selected item";
            if (SalesRegions.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach(SalesRegionModel sd in SalesRegions)
                {
                    if (sd.IsChecked)
                    {
                        if (sd.ID > 0)
                            DeleteSalesRegion(sd.ID);
                        deleteditems.Add(sd);
                    }
                }              
                foreach (SalesRegionModel pm in deleteditems)
                {
                    SalesRegions.Remove(pm);
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
