using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static PTR.DatabaseQueries;
using static PTR.StaticCollections;

namespace PTR.ViewModels
{
    public class UsersViewModel : ObjectCRUDViewModel
    {
        FullyObservableCollection<Models.UserModel> _associates;
        FullyObservableCollection<Models.CountryModel> _countries;
        FullyObservableCollection<Models.SalesDivisionModel> _salesdivisions;
        Models.UserModel _associate;
        bool _isdirty;
       
        public UsersViewModel()
        {
            GetCountryList();
            _countries = CountryList;
            _countries.Add(new Models.CountryModel() { GOM = new Models.GenericObjModel() });
            _salesdivisions = GetSalesDivisionsMasterList();                  
                       
            SaveAndClose = new RelayCommand(ExecuteSaveAndClose, CanExecuteSave);           
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);

            _associate = new Models.UserModel();   
            if (_countries.Count > 0)
                SelectedCountry=_countries[0];
                             
            if (Associates.Count>0)
                ScrollToSelectedItem = 0;            
        }

        #region Dirty Records Functions

        Collection<int> _dirtyrows = new Collection<int>();
        private void AddDirty(int _id)
        {
            if (!_dirtyrows.Contains(_id))
                _dirtyrows.Add(_id);
        }

        #endregion

        #region Private functions

        private void BuildAssociatesColl()
        {
            Models.SelectedCountriesModel _newcountry;
            Models.UserPermissionTypesModel usrperms;
            Models.SelectedSalesDivisionModel salesdiv;            

            foreach (Models.UserModel am in Associates)
            {
                
                am.IsEnabled = !(_currentuser.GOM.ID == am.GOM.ID);

                am.SalesDivisionsColl = new FullyObservableCollection<Models.SelectedSalesDivisionModel>();
                List<string> _associatessaledivs = am.SalesDivisions.Split(',').ToList();
                foreach (Models.SalesDivisionModel sd in _salesdivisions)
                {
                    salesdiv = new Models.SelectedSalesDivisionModel();
                    salesdiv.ID = sd.GOM.ID;
                    if (am.Administrator)
                        salesdiv.IsSelected = true;
                    else
                        salesdiv.IsSelected = _associatessaledivs.Contains(sd.ID.ToString());
                    salesdiv.Name = sd.GOM.Name;
                    am.SalesDivisionsColl.Add(salesdiv);
                }

                am.SalesDivisionsColl.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;

                am.CountriesColl = new FullyObservableCollection<Models.SelectedCountriesModel>();
                List<string> _associatescountries = am.Countries.Split(',').ToList();
                foreach (Models.CountryModel cm in _countries)
                {
                    if (cm.GOM.ID > 0)
                    {
                        _newcountry = new Models.SelectedCountriesModel();
                        _newcountry.Name = cm.GOM.Name;
                        _newcountry.ID = cm.GOM.ID;
                        _newcountry.IsSelected = _associatescountries.Contains(cm.ID.ToString());
                        am.CountriesColl.Add(_newcountry);
                    }
                }                
                
                am.CountriesColl.ItemPropertyChanged += Countries_ItemPropertyChanged;

                am.PermissionsColl = new FullyObservableCollection<Models.UserPermissionTypesModel>();
                foreach (EnumValue ev in EnumerationLists.UserPermissionTypeList)
                {
                    usrperms = new Models.UserPermissionTypesModel();
                    if (am.Administrator)
                        usrperms.IsSelected = true;
                    else
                        usrperms.IsSelected = (am.UserPermissions & (UserPermissionsType)ev.ID) == (UserPermissionsType)ev.ID;
                    usrperms.GOM.Name = ev.Description;
                    usrperms.GOM.ID = ev.ID;
                    am.PermissionsColl.Add(usrperms);
                }
                am.PermissionsColl.ItemPropertyChanged += Permissions_ItemPropertyChanged;               
            }
            Associates.ItemPropertyChanged += Associates_ItemPropertyChanged;
        }

        #endregion

        #region Event Handlers

        private void SalesDivisions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //add to dirty list - ensure that row id or id of item in collection is obtained
            AddDirty(SelectedAssociateIndex);
            _isdirty = true;
        }

        private void Countries_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {       
            AddDirty(SelectedAssociateIndex);
            _isdirty = true;
        }

        private void Permissions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //add to dirty list - ensure that row id or id of item in collection is obtained
            _isdirty = true;
            AddDirty(SelectedAssociateIndex);
            int permissionid = ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[e.CollectionIndex].GOM.ID;
           
            int indexvalue = e.CollectionIndex;

            switch (permissionid){

                case (int)UserPermissionsType.ViewOwnProjects://[Description("Create/Edit Own Projects")]
                    {
                        if (((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[(int)Math.Log((int)UserPermissionsType.CRUDOwnProjects,2)].IsSelected == true)
                            ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[indexvalue].IsSelected = true;
                        break;
                    }
                case (int)UserPermissionsType.CRUDOwnProjects://[Description("Create/Edit Own Projects")]
                    {
                        if (((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[indexvalue].IsSelected == true)
                            ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[(int)Math.Log((int)UserPermissionsType.ViewOwnProjects,2)].IsSelected = true;
                        break;
                    }
                case (int)UserPermissionsType.ViewOtherUsersProjects://[Description("View Other Users Projects")]   //edit others projects => view others projects       
                    {
                        if(((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[(int)Math.Log((int)UserPermissionsType.CRUDOtherUsersProjects,2)].IsSelected == true)
                            ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[indexvalue].IsSelected = true;                              
                        break;
                    }
                case (int)UserPermissionsType.CRUDOtherUsersProjects: //[Description("Create/Edit Other Users Projects")]
                    {
                        if (((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[indexvalue].IsSelected == true)
                            ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[(int)Math.Log((int)UserPermissionsType.ViewOtherUsersProjects,2)].IsSelected = true;            
                        break;
                    }
                case (int)UserPermissionsType.CreatePlaybookReports://[Description("Create Playbook Reports")]
                    {
                        if ((((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[(int)Math.Log((int)UserPermissionsType.CRUDPlaybook,2)].IsSelected == true))
                            ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[indexvalue].IsSelected = true;            
                        break;
                    }
                case (int)UserPermissionsType.CRUDPlaybook://[Description("Create/Edit Playbook")]  
                    {
                        if ((((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[indexvalue].IsSelected == true))
                            ((FullyObservableCollection<Models.UserPermissionTypesModel>)sender)[(int)Math.Log((int)UserPermissionsType.CreatePlaybookReports,2)].IsSelected = true;
                        break;
                    }        
            }                                             
        }

        private void Associates_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //add to dirty list
            _isdirty = true;
            AddDirty(SelectedAssociateIndex);
            if (e.PropertyName == "Name")            
                DuplicateName = IsDuplicateName();            
            else
                if(e.PropertyName == "Administrator")            
                //if ischecked then select all divisions and all countries and all permissions             
                    SetAdministrator(e.CollectionIndex, Associates[e.CollectionIndex].Administrator);

        }

        #endregion

        #region Properties

        public FullyObservableCollection<Models.CountryModel> Countries
        {
            get { return _countries; }
            set { SetField(ref _countries, value); }
        }

        Models.CountryModel country;
        public Models.CountryModel SelectedCountry {
            get { return country; }
            set { SetField(ref country, value);
                if (_isdirty)
                {
                    SaveAssociates();
                    _isdirty = false;
                }
                //filter associates
                //var q = (from assoc in _associatesmaster
                //         where assoc.Countries.Split(',').Contains(SelectedCountry.ID.ToString())
                //         select assoc);
                // Associates?.Clear();              
                // Associates = new FullyObservableCollection<Models.UserModel>(q);

                 Associates = DatabaseQueries.GetAssociatesOfCountry(value.GOM.ID);
                 BuildAssociatesColl();
            }
        }

        public FullyObservableCollection<Models.UserModel> Associates
        {
            get { return _associates; }
            set { SetField(ref _associates, value); }
        }

        public Models.UserModel Associate
        {
            get { return _associate; }
            set {
                if (value != null)
                    SetField(ref _associate, value); }
        }

        Models.UserModel _selectedassociate;
        public Models.UserModel SelectedAssociate
        {
            get { return _selectedassociate; }
            set { SetField(ref _selectedassociate, value); }
        }

        int _selectedassociateindex;
        public int SelectedAssociateIndex
        {
            get { return _selectedassociateindex; }
            set { SetField(ref _selectedassociateindex, value); }
        }

        #endregion

        #region Validation

        private bool IsDuplicateName()
        {
            bool _isduplicate = false;

            var query = _associates.GroupBy(x => x.GOM.Name.ToUpper())
              .Where(g => g.Count() > 1)
              .Select(y => y.Key)
              .ToList();
            if (query.Count > 0)
                return true;

            return _isduplicate;
        }

        #endregion

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            if (DuplicateName)
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
          //  _canexecuteadd = false;

            Models.SelectedSalesDivisionModel salesdiv;
            FullyObservableCollection<Models.SelectedSalesDivisionModel> salesdivcoll = new FullyObservableCollection<Models.SelectedSalesDivisionModel>();
            foreach (Models.SalesDivisionModel sd in _salesdivisions)
            {
                salesdiv = new Models.SelectedSalesDivisionModel();
                salesdiv.ID = sd.GOM.ID;
                salesdiv.IsSelected = false;
                salesdiv.Name = sd.GOM.Name;
                salesdivcoll.Add(salesdiv);
            }
            salesdivcoll.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;

            Models.SelectedCountriesModel _newcountry;
            FullyObservableCollection<Models.SelectedCountriesModel> countriescoll = new FullyObservableCollection<Models.SelectedCountriesModel>();
            foreach (Models.CountryModel cm in _countries)
            {
                _newcountry = new Models.SelectedCountriesModel();
                _newcountry.Name = cm.GOM.Name;
                _newcountry.ID = cm.GOM.ID;
                if (cm.GOM.ID == SelectedCountry.GOM.ID)
                    _newcountry.IsSelected = true;
                else                        
                    _newcountry.IsSelected = false;
                countriescoll.Add(_newcountry);
            }
            countriescoll.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;

            Models.UserPermissionTypesModel usrperms;
            FullyObservableCollection<Models.UserPermissionTypesModel> permissionscoll = new FullyObservableCollection<Models.UserPermissionTypesModel>();
            foreach (EnumValue ev in EnumerationLists.UserPermissionTypeList)
            {
                usrperms = new Models.UserPermissionTypesModel();
                if (ev.ID == (int)UserPermissionsType.ViewOwnProjects)
                    usrperms.IsSelected = true;
                else                    
                    usrperms.IsSelected = false;
                usrperms.GOM.Name = ev.Description;
                usrperms.GOM.ID = ev.ID;
                permissionscoll.Add(usrperms);
            }
            permissionscoll.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;

            Associates.Add(new Models.UserModel() {GOM = new Models.GenericObjModel(), LoginName=string.Empty, SalesDivisions=string.Empty, SalesDivisionsColl = salesdivcoll,
               UserPermissions=0, PermissionsColl = permissionscoll, CountryID=1, GIN=string.Empty, CountriesColl = countriescoll, Administrator=false, IsEnabled=true });
            Associates.ItemPropertyChanged += Associates_ItemPropertyChanged;
            ScrollToSelectedItem = Associates.Count - 1;
            AddDirty(Associates.Count - 1);
        }
                         
        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        //save
        private bool CanExecuteSave(object obj)
        {
            if (DuplicateName)
                return false;

            if (!_isdirty)
                return false;

            return _canexecutesave;
        }
                       
        private void ExecuteSaveAndClose(object parameter)
        {
            SaveAssociates();
            CloseWindow();
        }

        private void SaveAssociates()
        {
           // foreach (Models.UserModel am in _associates)
            foreach(int i in _dirtyrows)
            {
                Models.UserModel am = _associates[i];                
                if (!string.IsNullOrEmpty(am.GOM.Name))
                {
                    //convert collections to strings

                    var q = from a in am.SalesDivisionsColl
                            where a.IsSelected == true
                            select a.ID;
                 
                    am.SalesDivisions = string.Join(",", q.ToList());

                    var q2 = from a in am.CountriesColl
                             where a.IsSelected == true
                             select a.ID;

                    am.Countries = string.Join(",", q2.ToList());
                    
                    //sum the values for UserPermissions
                    int userpermissions = (from x in am.PermissionsColl where x.IsSelected select x.ID).Sum();
                    am.UserPermissions = (UserPermissionsType) userpermissions;

                    if (am.ID == 0)
                        AddNewAssociate(am);
                    else
                        UpdateAssociate(am);
                }
            }
            _dirtyrows.Clear();
        }

        private void SetAdministrator(int collectionindex, bool presetvalue)
        {
            //add divisions if necessary
            //check divisions
            Models.SelectedSalesDivisionModel salesdiv;
            _associates[collectionindex].SalesDivisionsColl.Clear();
            foreach (Models.SalesDivisionModel sd in _salesdivisions)
            {
                salesdiv = new Models.SelectedSalesDivisionModel();
                salesdiv.ID = sd.GOM.ID;
                salesdiv.IsSelected = presetvalue;
                salesdiv.Name = sd.GOM.Name;
                _associates[collectionindex].SalesDivisionsColl.Add(salesdiv);
            }
            _associates[collectionindex].SalesDivisionsColl.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;
            
            //add countries if necessary
            //check countries            
            Models.SelectedCountriesModel _newcountry;          
            _associates[collectionindex].CountriesColl.Clear();
            foreach (Models.CountryModel cm in _countries)
            {
                if (cm.GOM.ID > 0)
                {
                    _newcountry = new Models.SelectedCountriesModel();
                    _newcountry.Name = cm.GOM.Name;
                    _newcountry.ID = cm.GOM.ID;
                    _newcountry.IsSelected = presetvalue;
                    _associates[collectionindex].CountriesColl.Add(_newcountry);
                }
            }
            _associates[collectionindex].CountriesColl.ItemPropertyChanged += SalesDivisions_ItemPropertyChanged;

            //add permissions if necessary
            //check permissions
            Models.UserPermissionTypesModel usrperms;
            _associates[collectionindex].PermissionsColl.Clear();
            foreach (EnumValue ev in EnumerationLists.UserPermissionTypeList)
            {
                usrperms = new Models.UserPermissionTypesModel();
                usrperms.IsSelected = presetvalue;
                usrperms.GOM.Name = ev.Description;
                usrperms.GOM.ID = ev.ID;
                _associates[collectionindex].PermissionsColl.Add(usrperms);
            }
            _associates[collectionindex].PermissionsColl.ItemPropertyChanged += Permissions_ItemPropertyChanged;

        }
               
        #endregion

    }
   
}