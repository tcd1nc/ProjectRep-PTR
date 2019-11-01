using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;

namespace PTR.ViewModels
{
    public class CustomersViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<CustomerModel> customers;
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<SalesRegionModel> salesregions;
      
        CustomerModel customer;
        bool candeletecustomer;

        public CustomersViewModel()
        {
            customers = new FullyObservableCollection<CustomerModel>();
            StaticCollections.Customers = GetCustomers();
            foreach(CustomerModel cm in StaticCollections.Customers)
            {
                if (!cm.GOM.Deleted)
                {
                    CustomerModel newc = new CustomerModel()
                    {
                        GOM = new GenericObjModel() { ID = cm.GOM.ID, Name = cm.GOM.Name, Description = cm.GOM.Description },
                        CorporateID = cm.CorporateID,
                        CountryID = cm.CountryID,
                        Location = cm.Location,
                        Number = cm.Number,
                        SalesRegionID = cm.SalesRegionID
                    };
                    newc.GOM.PropertyChanged += _customer_PropertyChanged;
                    newc.PropertyChanged += _customer_PropertyChanged;
                    customers.Add(newc);
                }
            }
            
            countries = StaticCollections.Countries;
           

            customer = new CustomerModel
            {
                GOM = new GenericObjModel() { ID = 0, Description = string.Empty, Name = string.Empty },
                Location = string.Empty,
                Number = string.Empty                 
            };


            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            Delete = new RelayCommand(ExecuteDelete, CanExecuteDelete);
            CustomerListEnabled = true;

            HasCustomers = (customers.Count > 0);            
        }
    
        #region Event handlers

        private void _customer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Name")
                DuplicateName = IsDuplicateName();
            CheckFieldValidation();
        }
        private void Customers_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            CheckFieldValidation();
        }

        #endregion

        #region Properties

        public CustomerModel Customer
        {
            get { return customer; }
            set
            {
                if (value != null)
                {
                    SetField(ref customer, value);
                    candeletecustomer = IsDeletableCustomer(value.GOM.ID);                    
                }
            }
        }

        public FullyObservableCollection<CustomerModel> Customers
        {
            get { return customers; }
            set { SetField(ref customers, value); }
        }

        public FullyObservableCollection<CountryModel> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
        }

        CountryModel selectedcountry;
        public CountryModel SelectedCountry
        {
            get { return selectedcountry; }
            set {
                FilterSalesRegions(value.GOM.ID);
                SetField(ref selectedcountry, value);                    
            }
        }


        public FullyObservableCollection<SalesRegionModel> SalesRegions
        {
            get { return salesregions; }
            set { SetField(ref salesregions, value); }
        }

        bool hascustomers = true;
        public bool HasCustomers
        {
            get { return hascustomers; }
            set { SetField(ref hascustomers, value); }
        }

        #endregion

        private void FilterSalesRegions(int countryid)
        {            
            FullyObservableCollection<SalesRegionModel> tempsalesregions = new FullyObservableCollection<SalesRegionModel>();
            SalesRegions?.Clear();
            //tempsalesregions.Add(new SalesRegionModel() {ID=-1, CountryID = countryid, Name = "All" });
            foreach (SalesRegionModel sd in StaticCollections.SalesRegions)
            {
                if (sd.CountryID == countryid)
                    tempsalesregions.Add(sd);
            }
            SalesRegions = tempsalesregions;
        }



        #region Validation

        private bool IsDuplicateName()
        {
            var query = customers.GroupBy(x => x.GOM.Name.ToUpper())
            .Where(g => g.Count() > 1)
            .Select(y => y.Key)
            .ToList();
            return (query.Count > 0);                        
        }
        
        private void CheckFieldValidation()
        {
            bool CountryRequired = !(Customer.CountryID > 0);
            bool CustomerNameRequired = (string.IsNullOrEmpty(Customer.GOM.Name));
            InvalidField = (DuplicateName || CountryRequired || CustomerNameRequired);

            if (CustomerNameRequired)
                DataMissingLabel = "Customer Name Missing";
            else
                if (DuplicateName)
                DataMissingLabel = "Duplicate Customer Name";
            else
                if (CountryRequired)
                    DataMissingLabel = "Country Missing";
 
            canexecuteadd = !InvalidField;
            canexecutesave = !InvalidField;
            CustomerListEnabled = !InvalidField;
        }

        bool invalidfield = false;
        public bool InvalidField
        {
            get { return invalidfield; }
            set { SetField(ref invalidfield, value); }
        }
        
        string datamissing = string.Empty;
        public string DataMissingLabel
        {
            get { return datamissing; }
            set { SetField(ref datamissing, value); }
        }

        bool listenabled;
        public bool CustomerListEnabled
        {
            get { return listenabled; }
            set { SetField(ref listenabled, value); }
        }       

        #endregion

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return true;
        }

        private void ExecuteAddNew(object parameter)
        {
            CustomerModel newc = new CustomerModel() {
                GOM = new GenericObjModel(),
                CorporateID = 0,
                CountryID = 0,
                Location = string.Empty,
                Number = string.Empty
            };
            newc.GOM.PropertyChanged += _customer_PropertyChanged;
            newc.PropertyChanged += _customer_PropertyChanged;
            
            Customers.Add(newc);
            Customer = Customers[Customers.Count - 1];
                     
            ScrollToSelectedItem = Customers.Count - 1;

            canexecuteadd = false;
            HasCustomers = true;
        }
        
        private void ExecuteCancel(object parameter)
        {
            StaticCollections.Customers = GetCustomers();
            CloseWindow();
        }

        private bool CanExecuteSave(object obj)
        {
            return canexecutesave;
        }
       
        private void ExecuteSave(object parameter)
        {            
            canexecuteadd = true;
            foreach (CustomerModel ms in Customers)
            {
                if (ms.GOM.ID == 0)
                    AddCustomer(ms);
                else
                    UpdateCustomer(ms);
            }            
        }

        private bool CanExecuteDelete(object obj)
        {
            if (!candeletecustomer)
                return false;
            if (!HasCustomers)
                return false;
            return canexecutedelete;
        }
        
        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            if(msg.ShowMessage("Do you want to delete this customer?", "Deleting Customer",GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                DeleteCustomer(Customer.GOM.ID);
                Customers.Remove(Customer);
                Customers.ItemPropertyChanged += Customers_ItemPropertyChanged;
            }

            if (customers.Count == 0)
            {
                HasCustomers = false;
                Customer.GOM.ID = 0;
                Customer.CorporateID = 0;
                Customer.GOM.Name = string.Empty;
                Customer.Number = string.Empty;
                Customer.Location = string.Empty;                
                canexecutedelete = false;
            }
            msg = null;
        }

        private bool IsDeletableCustomer(int id)
        {           
            if (id > 0)
            {
                if (GetCountCustomerProjects(id) > 0)                
                    return false;                
                else
                    return true;
            }
            return false;
        }

        #endregion

    }
}
