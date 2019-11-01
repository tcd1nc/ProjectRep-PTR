using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class AccessPermissionsViewModel :ViewModelBase
    {
        FullyObservableCollection<Models.OperatingCompanyModel> _opcos;
        FullyObservableCollection<Models.CountryModel> _countries;
        Collection<EnumValue> _accesspermissions;
        FullyObservableCollection<Models.AccessPermissionModel> _associateaccesspermissions;
        bool canSaveExecute = true;
        bool canAddNewExecute = true;
        bool canCancelExecute = true;
        bool canDeleteExecute = true;
        int _thisassociateid = 0;

        public AccessPermissionsViewModel(int _associateid)
        {
            init();
            _thisassociateid = _associateid;
            _associateaccesspermissions = GetAssociatePermissions(_associateid);
            _associateaccesspermissions.ItemPropertyChanged += _associateaccesspermissions_ItemPropertyChanged;
        }

        private void _associateaccesspermissions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if(e.PropertyName == "CountryID")
                CheckForDuplicateCountry();
        }

        private void init()
        {
            _opcos = GetOperatingCompanies();
            _countries = GetCountries();
            _accesspermissions = EnumerationLists.UserPermissionsList;
            
            SavePermissionsCommand = new RelayCommand(SavePermissions, param => this.canSaveExecute);
            AddNewPermissionsCommand = new RelayCommand(AddNewPermissions, param => this.canAddNewExecute);
            CancelPermissionsCommand = new RelayCommand(CancelPermissions, param => this.canCancelExecute);
            DeletePermissionCommand = new RelayCommand(DeletePermission, param => this.canDeleteExecute);

        }


        public FullyObservableCollection<Models.OperatingCompanyModel> OperatingCompanies
        {
            get { return _opcos; }
            set { SetField(ref _opcos, value); }
        }

        Models.OperatingCompanyModel _selectedopco;
        public Models.OperatingCompanyModel SelectedOperatingCompany
        {
            get { return _selectedopco; }
            set { SetField(ref _selectedopco, value);

                if (value != null)
                    filterAssociateAccess(value.GOM.ID);
            }
        }

        public FullyObservableCollection<Models.CountryModel> Countries
        {
            get { return _countries; }
            set { SetField(ref _countries, value); }
        }

        public Collection<EnumValue> AccessPermissions
        {
            get { return _accesspermissions; }
            set { SetField(ref _accesspermissions, value); }
        }


        FullyObservableCollection<Models.AccessPermissionModel> _filteredaccesspermissions;
        public FullyObservableCollection<Models.AccessPermissionModel> FilteredAccessPermissions
        {
            get { return _filteredaccesspermissions; }
            set { SetField(ref _filteredaccesspermissions, value); }
        }

        Models.AccessPermissionModel _selectedaccesspermission;
        public Models.AccessPermissionModel SelectedAccessPermission
        {
            get { return _selectedaccesspermission; }
            set { SetField(ref _selectedaccesspermission, value); }
        }
        
        private void filterAssociateAccess(int _opcoid)
        {
            FilteredAccessPermissions.Clear();
            FullyObservableCollection<Models.AccessPermissionModel> _tempfilteredaccesspermissions = new FullyObservableCollection<Models.AccessPermissionModel>();
            foreach (Models.AccessPermissionModel ap in _associateaccesspermissions)
            {
                if (ap.OperatingCompanyID == _opcoid)
                    _tempfilteredaccesspermissions.Add(ap);
            }
            FilteredAccessPermissions = _tempfilteredaccesspermissions;
        }

        bool _duplicatepermission;
        public bool DuplicatePermission
        {
            get { return _duplicatepermission; }
            set { SetField(ref _duplicatepermission, value);
                canSaveExecute = value;
            }
        }

        private void CheckForDuplicateCountry()
        {
           var query = _associateaccesspermissions.GroupBy(x => x.CountryID)
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();

            if (query.Count > 0)
                DuplicatePermission = true;
            else
                DuplicatePermission = false;        
        }


        #region Commands

        ICommand _savepermissions;
        public ICommand SavePermissionsCommand
        {
            get { return _savepermissions; }
            set { _savepermissions = value; }
        }

        private void SavePermissions(object obj)
        {
            foreach(Models.AccessPermissionModel ap in _associateaccesspermissions)
            {
                if(ap.ID == 0) //addnew
                {
                    if(ap.CountryID>0 && ap.AccessPermissionTypeID > 0)
                    {

                    }

                }
                else //update
                {


                }
            }

            CloseWindow();
        }

        ICommand _addnewpermission;
        public ICommand AddNewPermissionsCommand
        {
            get { return _addnewpermission; }
            set { _addnewpermission = value; }
        }

        private void AddNewPermissions(object obj)
        {
            if (SelectedOperatingCompany != null)
            {
                _associateaccesspermissions.Add(new Models.AccessPermissionModel() {ID = 0, AssociateID = _thisassociateid, CountryID = 0, AccessPermissionTypeID = 0, OperatingCompanyID = SelectedOperatingCompany.ID });
                _associateaccesspermissions.ItemPropertyChanged += _associateaccesspermissions_ItemPropertyChanged;
                filterAssociateAccess(SelectedOperatingCompany.GOM.ID);

                canAddNewExecute = false;
               
            }
        }

        ICommand _cancelpermission;
        public ICommand CancelPermissionsCommand
        {
            get { return _cancelpermission; }
            set { _cancelpermission = value; }
        }

        private void CancelPermissions(object obj)
        {
            if (!canAddNewExecute)
                canAddNewExecute = true;
            else
                CloseWindow();

        }

        ICommand _deletepermission;
        public ICommand DeletePermissionCommand
        {
            get { return _deletepermission; }
            set { _deletepermission = value; }
        }

        private void DeletePermission(object obj)
        {
            if (SelectedAccessPermission != null)
            {
                FilteredAccessPermissions.Remove(SelectedAccessPermission);
                _associateaccesspermissions.Remove(SelectedAccessPermission);
            }

        }

        #endregion


    }
}
