using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class CountriesViewModel : ViewModelBase
    {
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<ModelBaseVM> availablecodes = new FullyObservableCollection<ModelBaseVM>();
        CountryModel country;
        FullyObservableCollection<ModelBaseVM> operatingcompanies;

        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }


        bool isdirty = false;
        public CountriesViewModel()
        {
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

        public FullyObservableCollection<ModelBaseVM> AvailableCodes
        {
            get { return availablecodes; }
            set { SetField(ref availablecodes, value); }
        }

        bool duplicatename;
        public bool DuplicateName
        {
            get { return duplicatename; }
            set { SetField(ref duplicatename, value); }
        }

        private void GetCountries()
        {
            FullyObservableCollection<CountryModel> tempcountries = DatabaseQueries.GetCountries();
            countries = new FullyObservableCollection<CountryModel>();
            foreach (CountryModel cm in tempcountries)
            {
                if (!cm.Deleted)
                {
                    CountryModel newc = new CountryModel()
                    {
                        ID = cm.ID,
                        Name = cm.Name,
                        Description = cm.Description,
                        UseUSD = cm.UseUSD,
                        CultureCode = cm.CultureCode,
                        OperatingCompanyID = cm.OperatingCompanyID
                    };
                    newc.PropertyChanged += _countries_PropertyChanged;
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

            Collection<ModelBaseVM> unsortedcodes = new Collection<ModelBaseVM>();
            foreach (CultureInfo ci in cultures)            
                unsortedcodes.Add(new ModelBaseVM() { Description = ci.Name });
            
            var col = unsortedcodes.OrderBy(x => x.Description);
            foreach (ModelBaseVM gm in col)
                availablecodes.Add(gm);            
        }

        public CountryModel Country
        {
            get { return country; }
            set { SetField(ref country, value); }
        }

        public FullyObservableCollection<ModelBaseVM> OperatingCompanies
        {
            get { return operatingcompanies; }
            set { SetField(ref operatingcompanies, value); }
        }
                
        #endregion

        #region Validation

        private bool IsDuplicateName()
        {           
            var query = countries.GroupBy(x => x.Name.Trim().ToUpper())
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
            int nummissing = Countries.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
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
            Countries.Add(new CountryModel()
            {
                ID = 0,
                Name = string.Empty,
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
            CloseWindow();
        }

        private void SaveAll()
        {
            if (isdirty)
            {
                foreach (CountryModel cm in Countries)
                {
                    if (cm.ID == 0)
                        cm.ID = AddCountry(cm);
                    else
                        UpdateCountry(cm);
                }            
                canexecutesave = false;
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