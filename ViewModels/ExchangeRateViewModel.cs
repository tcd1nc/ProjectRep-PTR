using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class ExchangeRateViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<ExchangeRateModel> exchangerates;
        bool isdirty = false;

        public ExchangeRateViewModel()
        {
            countries = GetCountries();
            if (Countries.Count > 0)
            {
                Country = Countries[0];
                GetExchangeRates(Countries[0].ID);
            }
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);     
        }

        #region Event handlers

        private void ExchangeRates_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            ExchangeRates[e.CollectionIndex].IsDirty = true;
            isdirty = true;
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
            set { SetField(ref selectedMonth, value); }
        }
                
        CountryModel selectedCountry;
        public CountryModel Country
        {
            get { return selectedCountry; }
            set {
                if (selectedCountry != null)                                            
                    SaveExchangeRates();                                   
                                      
                if (value != null)                
                    GetExchangeRates(value.ID);                  
                
                SetField(ref selectedCountry, value);
            }
        }

        private void GetExchangeRates(int countryid)
        {
            if (ExchangeRates != null)            
                ExchangeRates.ItemPropertyChanged -= ExchangeRates_ItemPropertyChanged;
            
            ExchangeRates = DatabaseQueries.GetExchangeRates(countryid);
            ExchangeRates.ItemPropertyChanged += ExchangeRates_ItemPropertyChanged;
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
        }

        #endregion

        #region Commands
       
        //save
        private bool CanExecuteSave(object obj)
        {
            return isdirty;
        }
        
        private void ExecuteSave(object parameter)
        {
            SaveExchangeRates();
            CloseWindow();
        }

        private void SaveExchangeRates()
        {
            if (isdirty)
            {
                foreach (ExchangeRateModel em in ExchangeRates)
                    if (em.IsDirty)
                    {
                        if (em.ID > 0)
                            UpdateExchangeRate(em);                           
                        else
                            em.ID = AddExchangeRate(em);
                        em.IsDirty = false;
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
                IMessageBoxService msg = new MessageBoxService();
                var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveExchangeRates();
                    return true;
                }
                else                    
                    return true;                               
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
