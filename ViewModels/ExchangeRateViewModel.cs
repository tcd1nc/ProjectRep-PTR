using System;
using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;

namespace PTR.ViewModels
{
    public class ExchangeRateViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<ExchangeRateModel> exchangerates;
        bool isdirty = false;

        public ExchangeRateViewModel()
        {
            ExCloseWindow = ExecuteClosing;
            countries = StaticCollections.Countries;
            if (Countries.Count > 0)
            {
                Country = Countries[0];
                GetExchangeRates(Countries[0].GOM.ID);
            }
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);            
        }

        #region Event handlers

        private void ExchangeRates_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            CheckFieldValidation();
        }

        #endregion

        #region Properties

        public FullyObservableCollection<ExchangeRateModel> ExchangeRates
        {
            get { return exchangerates; }
            set { SetField(ref exchangerates, value); }
        }
        
        ExchangeRateModel selectedMonth;
        public ExchangeRateModel SelectedMonth
        {
            get { return selectedMonth; }
            set { SetField(ref selectedMonth, value);
                  CheckFieldValidation();
            }
        }
                
        CountryModel selectedCountry;
        public CountryModel Country
        {
            get { return selectedCountry; }
            set {

                if (selectedCountry != null)
                {
                    if (canexecutesave)                    
                        SaveExchangeRates();
                    
                    InvalidField = false;
                    canexecuteadd = true;
                }
                                      
                if (value != null)
                {
                    GetExchangeRates(value.GOM.ID);                  
                }
                SetField(ref selectedCountry, value);
            }
        }

        private void GetExchangeRates(int countryid)
        {
            if (ExchangeRates != null)
            {
                ExchangeRates.ItemPropertyChanged -= ExchangeRates_ItemPropertyChanged;
            }
            ExchangeRates = DatabaseQueries.GetExchangeRates(countryid);
            ExchangeRates.ItemPropertyChanged += ExchangeRates_ItemPropertyChanged;
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
        }

        #endregion

        #region Validation

        bool MonthRequired;
        bool ExchangeRateRequired;
        bool DuplicateMonth;

        private bool MonthMissing()
        {
            int nummissing = ExchangeRates.Where(x => x.ExRateMonth == null).Count();
            return (nummissing > 0);
        }

        private bool ExchangeRateMissing()
        {
            int nummissing = ExchangeRates.Where(x => x.ExRate == 0).Count();
            return (nummissing > 0);
        }

        private void CheckFieldValidation()
        {          
            MonthRequired = MonthMissing();
            ExchangeRateRequired = ExchangeRateMissing();
            DuplicateMonth = IsDuplicateMonth();
            isdirty = true;

            InvalidField = (DuplicateMonth || MonthRequired || ExchangeRateRequired);

            if (MonthRequired)
                DataMissingLabel = "Month Missing";
            else
                if (DuplicateMonth)
                DataMissingLabel = "Duplicate Month";
            else
                if (ExchangeRateRequired)
                DataMissingLabel = "Exchange Rate Missing";

            canexecuteadd = !InvalidField;
            canexecutesave = !InvalidField;            
        }

        private bool IsDuplicateMonth()
        {
            var query = exchangerates.GroupBy(x => x.ExRateMonth)
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
            return (query.Count > 0);                
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
            canexecuteadd = false;
            ExchangeRates.Add(new ExchangeRateModel()
            {
                ExRate = 1,
                ExRateMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1),
                ID = 0,
                CountryID = Country.GOM.ID
            });
            CheckFieldValidation();
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
            SaveExchangeRates();
               
        }

        private void SaveExchangeRates()
        {
            if (isdirty)
            {
                foreach (ExchangeRateModel em in ExchangeRates)
                    if (em.ID == 0)
                        em.ID = AddExchangeRate(em);
                    else
                        UpdateExchangeRate(em);
            
                canexecutesave = false;
                isdirty = false;
            }
        }
              
        private void ExecuteClosing(object parameter)
        {
            if (ExchangeRates != null)
            {
                ExchangeRates.ItemPropertyChanged -= ExchangeRates_ItemPropertyChanged;
            }
            if (canexecutesave)
            {
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveExchangeRates();
                }
            }
        }
        #endregion

    }
}
