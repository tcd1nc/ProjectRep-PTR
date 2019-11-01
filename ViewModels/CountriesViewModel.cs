using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;

namespace PTR.ViewModels
{
    public class CountriesViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<GenericObjModel> availablecodes = new FullyObservableCollection<GenericObjModel>();
        CountryModel country;
        FullyObservableCollection<GenericObjModel> operatingcompanies;
      
        bool isdirty = false;
        public CountriesViewModel()
        {
            ExCloseWindow = ExecuteClosing;
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            operatingcompanies = GetOperatingCompanies();
            country = new CountryModel();
            GetCountries();
            GetAvailableCodes();
            canexecutesave = false;
        }

        #region Event handlers

        private void Countries_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            CheckValidation();
        }

        private void _countries_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CheckValidation();
        }

        #endregion

        #region Properties

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
        }

        public FullyObservableCollection<GenericObjModel> AvailableCodes
        {
            get { return availablecodes; }
            set { SetField(ref availablecodes, value); }
        }

        private void GetCountries()
        {
            StaticCollections.Countries = DatabaseQueries.GetCountries();
            countries = new FullyObservableCollection<CountryModel>();
            foreach (CountryModel cm in StaticCollections.Countries)
            {
                if (!cm.GOM.Deleted)
                {
                    CountryModel newc = new CountryModel()
                    {
                        GOM = new GenericObjModel()
                        {
                            ID = cm.GOM.ID,
                            Name = cm.GOM.Name,
                            Description = cm.GOM.Description
                        },
                        UseUSD = cm.UseUSD,
                        CultureCode = cm.CultureCode,
                        OperatingCompanyID = cm.OperatingCompanyID
                    };
                    newc.GOM.PropertyChanged += _countries_PropertyChanged;
                    countries.Add(newc);
                }
            }
            Countries.ItemPropertyChanged += Countries_ItemPropertyChanged;
        }

        private void GetAvailableCodes()
        {
            var cultures = (from c in CultureInfo.GetCultures(CultureTypes.SpecificCultures)
                            where !c.IsNeutralCulture
                            select c).OrderBy(x => (new RegionInfo(new CultureInfo(x.Name, false).LCID)).DisplayName).Distinct();

            Collection<GenericObjModel> unsortedcodes = new Collection<GenericObjModel>();
            foreach (CultureInfo ci in cultures)
            {
                unsortedcodes.Add(new GenericObjModel() { Description = ci.Name });
            }

            var col = unsortedcodes.OrderBy(x => x.Description);

            foreach (GenericObjModel gm in col)
                availablecodes.Add(gm);            
        }

        public CountryModel Country
        {
            get { return country; }
            set { SetField(ref country, value); }
        }

        public FullyObservableCollection<GenericObjModel> OperatingCompanies
        {
            get { return operatingcompanies; }
            set { SetField(ref operatingcompanies, value); }
        }
                
        #endregion

        #region Validation

        private bool IsDuplicateName()
        {           
            var query = countries.GroupBy(x => x.GOM.Name.Trim().ToUpper())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsOPCOMissing()
        {
            int nummissing = Countries.Where(x => x.OperatingCompanyID == 0).Count();
            return (nummissing > 0);
        }

        private bool CountryNameMissing()
        {
            int nummissing = Countries.Where(x => string.IsNullOrEmpty(x.GOM.Name.Trim())).Count();
            return (nummissing > 0);
        }

        private bool CultureCodeMissing()
        {
            int nummissing = Countries.Where(x => string.IsNullOrEmpty(x.CultureCode.Trim())).Count();
            return (nummissing > 0);
        }

        private void CheckValidation()
        {
            isdirty = true;
            CountryNameRequired = CountryNameMissing();            
            DuplicateName = IsDuplicateName();
            OPCORequired = IsOPCOMissing();
            CultureCodeRequired = CultureCodeMissing();

            InvalidField = (DuplicateName || CountryNameRequired || OPCORequired || CultureCodeRequired);

            if (CountryNameRequired)
                    DataMissingLabel = "Country Name Missing";
                else 
                if (DuplicateName)
                    DataMissingLabel = "Duplicate Country Name";
                else                
                    if (OPCORequired)
                        DataMissingLabel = "Operating Company Missing";
                    else
                        if (CultureCodeRequired)
                            DataMissingLabel = "Culture Code Missing";
                 
            canexecuteadd = !InvalidField;
            canexecutesave = !InvalidField;
        }
        
        bool CountryNameRequired;
        bool OPCORequired;
        bool CultureCodeRequired;

        bool invalidfield;
        public bool InvalidField
        {
            get { return invalidfield; }
            set{ SetField(ref invalidfield, value); }
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
            GenericObjModel gom = new GenericObjModel() { ID = 0, Name = string.Empty };
            gom.PropertyChanged += _countries_PropertyChanged;

            Countries.Add(new CountryModel()
            {
                GOM = gom,
                OperatingCompanyID = 0,
                CultureCode ="en-US",
                UseUSD=true
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
            SaveAll();
        }

        private void SaveAll()
        {
            if (isdirty)
            {
                foreach (CountryModel cm in Countries)
                {
                    if (cm.GOM.ID == 0)
                        cm.GOM.ID = AddCountry(cm);
                    else
                        UpdateCountry(cm);
                }
            
                canexecutesave = false;
                isdirty = false;
            }
        }
                               
        private void ExecuteClosing(object parameter)
        {
            if (Countries != null)
            {
                Countries.ItemPropertyChanged -= Countries_ItemPropertyChanged;
                foreach (CountryModel cm in Countries)
                    cm.GOM.PropertyChanged -= _countries_PropertyChanged;
            }
            if (canexecutesave)
            {
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveAll();
                }
            }
            //refresh countries list
            StaticCollections.Countries = DatabaseQueries.GetCountries();
        }


        #endregion

    }
}