using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class CustomersViewModel : ViewModelBase
    {
        bool canexecutesave = true;
        bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;

        FullyObservableCollection<CustomerModel> customers;
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<SalesRegionModel> salesregions;
        TV.CustomerTreeViewViewModel TV;

        FullyObservableCollection<SalesRegionModel> locsalesregions;

        public CustomersViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);

            IsEnabled = false;
            GetCountries();
            GetSalesRegions();
            TV = new TV.CustomerTreeViewViewModel();
            GetCustomers();

            Customer = new CustomerModel();
            
            ClearCustomer();
            ResetCustomer();
            Customer.PropertyChanged += Customer_PropertyChanged;
        }
               
        #region Properties

        bool isenabled = true;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        bool tvisenabled = true;
        public bool TVIsEnabled
        {
            get { return tvisenabled; }
            set { SetField(ref tvisenabled, value); }
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

        public FullyObservableCollection<SalesRegionModel> SalesRegions
        {
            get { return salesregions; }
            set { SetField(ref salesregions, value); }
        }

        //new
        CustomerModel customer;
        public CustomerModel Customer
        {
            get { return customer; }
            set { SetField(ref customer, value); }
        }
                      
        bool isdeletedenabled;
        public bool IsDeletedEnabled
        {
            get { return isdeletedenabled; }
            set { SetField(ref isdeletedenabled, value); }
        }

        FullyObservableCollection<TreeViewNodeModel> nodes;
        public FullyObservableCollection<TreeViewNodeModel> Nodes
        {
            get { return nodes; }
            set { SetField(ref nodes, value); }
        }

        bool showdatamessage = false;
        public bool ShowDataMessage
        {
            get { return showdatamessage; }
            set { SetField(ref showdatamessage, value); }
        }

        string dataerror;
        public string DataMessageLabel
        {
            get { return dataerror; }
            set { SetField(ref dataerror, value); }
        }

        bool isaddmode = false;
        public bool IsAddMode
        {
            get { return isaddmode; }
            set { SetField(ref isaddmode, value); }
        }

        bool invaliddata = false;
        public bool InvalidData
        {
            get { return invaliddata; }
            set { SetField(ref invaliddata, value); }
        }

        #endregion

        #region Private functions

        private void UpdateCustomerTree()
        {
            TV.LoadCustomerTree();
            Nodes = TV.Nodes;
        }
               
        private void GetCountries()
        {
            FullyObservableCollection<CountryModel> tempcountries = new FullyObservableCollection<CountryModel>();
            FullyObservableCollection<CountryModel> loccountries = DatabaseQueries.GetCountries();

            foreach (CountryModel cm in loccountries)
            {
                if (!cm.Deleted)                
                    tempcountries.Add(cm);                
            }
            Countries = tempcountries;
        }

        private void GetCustomers()
        {            
            Customers = DatabaseQueries.GetCustomers();
            UpdateCustomerTree();
        }

        private void GetSalesRegions()
        {
            locsalesregions = DatabaseQueries.GetSalesRegions();
        }

        private void FilterSalesRegions(int countryid, int salesregionid)
        {
            FullyObservableCollection<SalesRegionModel> tempsalesregions = new FullyObservableCollection<SalesRegionModel>();
            int tempid = 0;           
            foreach (SalesRegionModel sd in locsalesregions)
            {
                if (sd.CountryID == countryid && !sd.Deleted)
                {
                    tempsalesregions.Add(sd);
                    if (sd.ID == salesregionid)
                        tempid = sd.ID;
                }
            }
            Customer.SalesRegionID = 0;

            SalesRegions = tempsalesregions;

            Customer.SalesRegionID = tempid;            
        }

        private void LoadSelectedCustomer(int id)
        {
            bool found = false;
            foreach(CustomerModel c in Customers)
            {
                if(c.ID == id)
                {
                    Customer.PropertyChanged-= Customer_PropertyChanged;
                    Customer.ID = c.ID;
                    Customer.Name = c.Name;
                    Customer.Location = c.Location;
                    Customer.SalesRegionID = c.SalesRegionID;
                    Customer.CountryID = c.CountryID;
                    Customer.Number = c.Number;
                    Customer.Deleted = c.Deleted;
                    Customer.IsEnabled = c.IsEnabled;
                    Customer.PropertyChanged += Customer_PropertyChanged;
                    found = true;
                    break;
                }
            }
            if (found)
            {
                FilterSalesRegions(Customer.CountryID, Customer.SalesRegionID);
                IsEnabled = true;
                IsDeletedEnabled = Customer.IsEnabled;

                ShowDataMessage = false;
                DataMessageLabel = string.Empty;

                isdirty = false;
                cancancelnewcustomer = false;
            }
        }

        #endregion

        #region Validation

        private void CheckValidation()
        {
            bool NameRequired = IsMissingName();
            bool DuplicateName = IsDuplicateName();
            bool CountryRequired= IsMissingCountry();
            bool SalesRegionRequired = IsMissingSalesRegion();

            InvalidData = NameRequired || DuplicateName || CountryRequired || SalesRegionRequired;
            
            if (NameRequired)            
                DataMessageLabel = "Missing Name";            
            else
            if (DuplicateName)            
                DataMessageLabel = "Duplicate Name";            
            else
            if (CountryRequired)            
               DataMessageLabel = "Missing Country";            
            else
            if (SalesRegionRequired)                                    
                DataMessageLabel = "Missing Sales Region";

            ShowDataMessage = InvalidData;

        }
               
        private bool IsDuplicateName()
        {
            int duplicate = Customers.Where(x => x.Name.Trim().ToUpper() == Customer.Name.Trim().ToUpper() && (Customer.SalesRegionID == x.SalesRegionID) 
            && (Customer.ID != x.ID || Customer.ID == 0)).Count();
            return (duplicate > 0);
        }

        private bool IsMissingName()
        {         
            return (string.IsNullOrEmpty(Customer.Name.Trim()));
        }

        private bool IsMissingCountry()
        {           
            return (Customer.CountryID == 0);
        }

        private bool IsMissingSalesRegion()
        {     
            return (Customer.SalesRegionID == 0);
        }

        
        #endregion

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return canexecuteadd;
        }
               
        private void ExecuteAddNew(object parameter)
        {
            SaveCustomer();            
 
            IsAddMode = true;

            ClearCustomer();

            ShowDataMessage = false;
            DataMessageLabel = string.Empty;
            cancancelnewcustomer = true;
            IsEnabled = true;
            isdirty = false;
        }

        private void Customer_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "CountryID")
                FilterSalesRegions(Customer.CountryID, Customer.SalesRegionID);
           
            if (IsAddMode || (!IsAddMode && Customer.ID > 0))
            {
                isdirty = true;
                CheckValidation();
            }
        }

        private bool CanExecuteSave(object obj)
        {
            if (InvalidData)
                return false;
            if (!isdirty)
                return false;
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {            
            SaveCustomer();
        }

        private void SaveCustomer()
        {
            ShowDataMessage = false;
            if (isdirty)
            {
                if (Customer.ID == 0)
                    Customer.ID = AddCustomer(Customer);
                else                
                    UpdateCustomer(Customer);
               
                DataMessageLabel = "Saved";
                ShowDataMessage = true;
                ResetCustomer();
                ClearCustomer();
                
                GetCustomers();
            }
            IsAddMode = false;
        }

        private void ClearCustomer()
        {
            Customer.ID = 0;
            Customer.Name = string.Empty;
            Customer.Location = string.Empty;            
            Customer.SalesRegionID = 0;
            Customer.CountryID = 0;
            Customer.Number = string.Empty;
            Customer.Deleted = false;
            Customer.IsEnabled = true;
        }

        private void ResetCustomer()
        {            
            isdirty = false;
            canexecuteadd = true;
            cancancelnewcustomer = false;
            IsAddMode = false;
            IsEnabled = false;
        }

        ICommand cancelnewcustomer;
        public ICommand CancelNewCustomer
        {
            get
            {
                if (cancelnewcustomer == null)
                    cancelnewcustomer = new DelegateCommand(CanCancelNewCustomer, ExecuteCancelNewCustomer);
                return cancelnewcustomer;
            }
        }

        bool cancancelnewcustomer = false;
        private bool CanCancelNewCustomer(object obj)
        {
            return cancancelnewcustomer;
        }

        private void ExecuteCancelNewCustomer(object parameter)
        {
            ClearCustomer();
            ResetCustomer();
        }

        readonly bool canexecutedelete = true;
        private bool CanExecuteDelete(object obj)
        {
            if (!isdeletedenabled)
                return false;

            return canexecutedelete;
        }

        ICommand selectcustomernode;
        public ICommand SelectCustomerNode
        {
            get
            {
                if (selectcustomernode == null)
                    selectcustomernode = new DelegateCommand(CanExecute, ExecuteSelectCustomerNode);
                return selectcustomernode;
            }
        }

        private void ExecuteSelectCustomerNode(object parameter)
        {
            try
            {
                if (parameter.GetType().Equals(typeof(TreeViewNodeModel)))
                {
                    if (((TreeViewNodeModel)parameter).NodeTypeID == 1)
                    {
                        int selectedcustomerid = ((TreeViewNodeModel)parameter).ID;                        
                        SaveCustomer();
                        //get customer
                        LoadSelectedCustomer(selectedcustomerid);                                                                                       
                    }
                }
            }
            catch { }
        }


        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }


        private void ExecuteClosing(object parameter)
        {           
        }

        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {
                if (!InvalidData)
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveCustomer();
                        return true;
                    }
                    else                    
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
