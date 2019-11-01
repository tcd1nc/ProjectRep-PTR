using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class CustomersViewModel2 : ObjectCRUDViewModel
    {
        bool isdirty = false;
        private int ID = 0;
        bool isloaded = false;
        bool SenderIsTreeView = false;

        FullyObservableCollection<CustomerModel> customers;
        FullyObservableCollection<CountryModel> countries;
        FullyObservableCollection<SalesRegionModel> salesregions;
        TV.CustomerTreeViewViewModel TV;

        public CustomersViewModel2()
        {
            ExCloseWindow = ExecuteClosing;
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            CustomerListEnabled = true;
            GetCountries();

            GetCustomers();
            IsEnabled = false;
            TV = new TV.CustomerTreeViewViewModel();

            UpdateCustomerTree();
            ClearCustomer();
            if (Customers != null)
                SelectedCustomer = Customers[0];

        }

        #region Properties

        bool isenabled = true;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
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

        CustomerModel selectedcustomer;
        public CustomerModel SelectedCustomer
        {
            get { return selectedcustomer; }
            set
            {                  
                SetField(ref selectedcustomer, value);
                if (value != null)
                {
                    SaveCustomer();
                    LoadSelectedCustomer(value);
                    SelectedCustomerIndex = GetCustomerIndex(ID);
                }
            }
        }

        int selectedcustomerindex;
        public int SelectedCustomerIndex
        {
            get { return selectedcustomerindex; }
            set { SetField(ref selectedcustomerindex, value); }
        }

        int countryid;
        public int CountryID
        {
            get { return countryid; }
            set
            {
                SetField(ref countryid, value);
                FilterSalesRegions(value);
                if (isloaded)
                {
                    CheckAllValidation();                   
                }
            }
        }

        int salesregionid;
        public int SalesRegionID
        {
            get { return salesregionid; }
            set { SetField(ref salesregionid, value);
                if (isloaded)
                {
                    CheckAllValidation();                  
                }
            }
        }
               
        string name;
        public string Name
        {
            get { return name; }
            set
            {
                SetField(ref name, value);
                if (isloaded)
                {
                    CheckAllValidation();                   
                }
            }
        }

        string customernumber;
        public string Number
        {
            get { return customernumber; }
            set { SetField(ref customernumber, value);
                if (isloaded)
                {
                    CheckAllValidation();                    
                }
            }
        }

        string location;
        public string Location
        {
            get { return location; }
            set { SetField(ref location, value);
                if (isloaded)
                {
                    CheckAllValidation();
                }
            }
        }

        bool deleted;
        public bool Deleted
        {
            get { return deleted; }
            set { SetField(ref deleted, value);
                if (isloaded)
                {
                    CheckAllValidation();                    
                }
            }
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
            foreach(CountryModel cm in StaticCollections.Countries)
            {
                if (!cm.GOM.Deleted)
                {
                    tempcountries.Add(cm);
                }
            }
            Countries = tempcountries;
        }

        private void GetCustomers()
        {            
            CustomerModel blank = new CustomerModel()
            {
                GOM = new GenericObjModel()
                {
                    ID = -1,
                    Name = "None Selected"
                },
                Location = string.Empty,
                Number = string.Empty,
                CountryID = -1,
                SalesRegionID = -1
            };
            Customers = DatabaseQueries.GetCustomers();
            Customers.Insert(0, blank);                       
        }

        private void FilterSalesRegions(int countryid)
        {
            FullyObservableCollection<SalesRegionModel> tempsalesregions = new FullyObservableCollection<SalesRegionModel>();
            SalesRegions?.Clear();
            foreach (SalesRegionModel sd in StaticCollections.SalesRegions)
            {
                if (sd.CountryID == countryid && !sd.Deleted)
                    tempsalesregions.Add(sd);
            }
            SalesRegions = tempsalesregions;
        }

        private void LoadSelectedCustomer(CustomerModel customer)
        {
            isloaded = false;
           
            IsDeletableCustomer(customer.GOM.ID);
            if (customer.GOM.ID == -1)
                IsEnabled = false;
            else
                IsEnabled = true;
          
            if (!SenderIsTreeView)
            {
                UpdateCustomerTree();                  
                SelectCustomer(customer.GOM.ID);                
            }
            ShowDataMessage = true;  //necessary to flush the property as it gets stuck!!!
            ShowDataMessage = false;
            DataMessageLabel = string.Empty;


            CountryID = customer.CountryID;
            SalesRegionID = 0;
            SalesRegionID = customer.SalesRegionID;

            ID = customer.GOM.ID;
            Name = customer.GOM.Name;
            Deleted = customer.GOM.Deleted;
            Location = customer.Location;
            Number = customer.Number;

            SenderIsTreeView = false;

            isdirty = false;
            canexecutesave = false;
            cancancelnewcustomer = false;
            isloaded = true;

        }

        private void SelectCustomer(int customerid)
        {
            if (Nodes != null )
            {
                foreach (TreeViewNodeModel opconode in Nodes)
                {
                    foreach (TreeViewNodeModel countrynode in opconode.Children)
                    {
                        foreach (TreeViewNodeModel salesregion in countrynode.Children)
                        {
                            foreach (TreeViewNodeModel customer in salesregion.Children)
                            {
                                if(customer.ID == customerid)
                                {
                                    customer.IsSelected = true;
                                    break;
                                }
                            }
                        }
                    }                    
                }               
            }
        }

        #endregion

        #region Validation

        private bool CheckAllValidation()
        {
            isdirty = true;
            if (!Validate("Name") || !Validate("Country") || !Validate("Sales Region"))
            {
                ShowDataMessage = true;
                canexecutesave = false;
                return false;
            }
            else
            {
                ShowDataMessage = false;
                DataMessageLabel = string.Empty;
                canexecutesave = true;
                return true;
            }            
        }

        private bool Validate(string test)
        {
            bool isvalid = true;
            DataMessageLabel = string.Empty;
            switch (test)
            {
                case "Name":
                    if (IsMissingName())
                    {
                        isvalid = false;
                        DataMessageLabel = "Missing Name";
                    }
                    else
                    if (IsDuplicateName())
                    {
                        isvalid = false;
                        DataMessageLabel = "Duplicate Name";
                    }
                    break;

                case "Country":
                    if (IsMissingCountry())
                    {
                        isvalid = false;
                        DataMessageLabel = "Missing Country";
                    }
                    break;

                case "Sales Region":
                    if (IsMissingSalesRegion())
                    {
                        isvalid = false;
                        DataMessageLabel = "Missing Sales Region";
                    }
                    break;

            };
            return isvalid;
        }

        private bool IsDuplicateName()
        {
            int duplicate = Customers.Where(x => x.GOM.Name.Trim().ToUpper() == Name.Trim().ToUpper() && (SalesRegionID == x.SalesRegionID) && (ID != x.GOM.ID || ID == 0)).Count();
            return (duplicate > 0);
        }

        private bool IsMissingName()
        {
            return (string.IsNullOrEmpty(Name.Trim()));
        }

        private bool IsMissingCountry()
        {
            return (CountryID <= 0);
        }

        private bool IsMissingSalesRegion()
        {
            return (SalesRegionID <= 0);
        }

        bool invalidfield = false;
        public bool ShowDataMessage
        {
            get { return invalidfield; }
            set { SetField(ref invalidfield, value); }
        }

        string dataerror;
        public string DataMessageLabel
        {
            get { return dataerror; }
            set { SetField(ref dataerror, value); }
        }

        bool listenabled;
        public bool CustomerListEnabled
        {
            get { return listenabled; }
            set { SetField(ref listenabled, value); }
        }

        bool isaddmode = false;
        public bool IsAddMode
        {
            get { return isaddmode; }
            set { SetField(ref isaddmode, value); }
        }

        #endregion

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return true;
        }
               
        private void ExecuteAddNew(object parameter)
        {
            if (isdirty)
            {
                SaveCustomer();
            }

            IsAddMode = true;

            IsDeletedEnabled = false;
            canexecuteadd = false;
            cancancelnewcustomer = true;
            CustomerListEnabled = false;
            UpdateCustomerTree();
            ShowDataMessage = false;
            DataMessageLabel = string.Empty;

            Name = string.Empty;
            Location = string.Empty;
            Number = string.Empty;

            IsEnabled = true;
            ID = 0;
            isloaded = true;

        }

        private void ExecuteCancel(object parameter)
        {
            StaticCollections.Customers = DatabaseQueries.GetCustomers();
            CloseWindow();
        }

        private bool CanExecuteSave(object obj)
        {
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {            
            SaveCustomer();
        }

        private bool SaveCustomer()
        {
            invalidfield = false;
            if (isdirty && canexecutesave)
            {
                CustomerModel Customer = new CustomerModel
                {
                    GOM = new GenericObjModel()
                    {
                        ID = ID,
                        Name = Name.Trim(),
                        Deleted = Deleted
                    },
                    Location = Location,
                    Number = Number,
                    CountryID = CountryID,
                    SalesRegionID = SalesRegionID
                };

                if (ID == 0)
                    ID = AddCustomer(Customer);   //get return identity from sql
                else                
                    UpdateCustomer(Customer);
                               
                GetCustomers();
                               
                canexecutesave = false;
                canexecuteadd = true;
                isdirty = false;
                cancancelnewcustomer = false;
                CustomerListEnabled = true;
                
                ShowDataMessage = true;
                DataMessageLabel = "Saved";
                if (IsAddMode)
                {
                    isloaded = false;
                    Name = string.Empty;
                    Location = string.Empty;
                    Number = string.Empty;
                    CountryID = 0;
                    SalesRegionID = 0;
                    UpdateCustomerTree();
                    isloaded = true;
                    IsAddMode = false;
                }

            }
            CustomerListEnabled = true;

            return true;
        }

        private int GetCustomerIndex(int customerid)
        {
            int index = 0;
            foreach(CustomerModel cm in Customers)
            {
                if (cm.GOM.ID == customerid)
                    return index;
                index++;
            }
            return 0;
        }

        private void ClearCustomer()
        {
            ID = 0;
            canexecutesave = false;
            canexecuteadd = true;
            isdirty = false;
            cancancelnewcustomer = false;
            CustomerListEnabled = true;
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
            SelectedCustomer = Customers[0];
            IsAddMode = false;
        }

        private bool CanExecuteDelete(object obj)
        {
            if (!isdeletedenabled)
                return false;

            return canexecutedelete;
        }

        private void IsDeletableCustomer(int customerid)
        {
            IsDeletedEnabled = !(GetCountCustomerProjects(customerid) > 0);
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
                    if( ((TreeViewNodeModel)parameter).NodeTypeID == 1)
                    {                       
                        foreach(CustomerModel c in Customers)
                        {
                            if(c.GOM.ID == ((TreeViewNodeModel)parameter).ID)
                            {
                                SaveCustomer();
                                SenderIsTreeView = true;                                
                                SelectedCustomer = c;                                
                                break;
                            }
                        }                     
                    }
                }
            }
            catch { }
        }
               
        private void ExecuteClosing(object parameter)
        {
            if (canexecutesave)
            {
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveCustomer();                                     
                }
            } 
            //refresh Customers list 
            StaticCollections.Customers = DatabaseQueries.GetCustomers();
        }

        #endregion

    }
}
