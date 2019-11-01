using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTR.ViewModels
{
    public class UserPermissionsViewModel :ViewModelBase
    {

        FullyObservableCollection<Models.CountryModel> _countries = new FullyObservableCollection<Models.CountryModel>();
        Collection<EnumValue> _standarduserpermissions;
        public UserPermissionsViewModel(Collection<Models.UserPermissionsModel> usrpermissions)
        {

            Countries = DatabaseQueries.GetCountries();
            UserPermissions = usrpermissions;

            _standarduserpermissions = EnumerationLists.UserPermissionTypeList;
            CreateTree();
            //create tree

        }
        
        public FullyObservableCollection<Models.CountryModel> Countries
        {
            get { return _countries; }
            set { SetField(ref _countries, value); }
        }
        
        Collection<Models.UserPermissionsModel> _userpermissions;
        private Collection<Models.UserPermissionsModel> UserPermissions
        {
            get { return _userpermissions; } 
            set { SetField(ref _userpermissions, value); }
        }

        FullyObservableCollection<Models.UserPermissionsBranchModel> _userpermissionstree;
        public FullyObservableCollection<Models.UserPermissionsBranchModel> UserPermissionsTree
        {
            get { return _userpermissionstree; }
            set { SetField(ref _userpermissionstree, value); }
        }
        

        private void CreateTree()
        {
            _userpermissionstree = new FullyObservableCollection<Models.UserPermissionsBranchModel>();
            foreach(Models.CountryModel cm in Countries)
            {
                Models.UserPermissionsBranchModel upbm = new Models.UserPermissionsBranchModel();
                upbm.Country = cm;
                //upbm.IsChecked = true;

                //var q = UserPermissions.Where(x => x.CountryID == cm.ID);
                //List<Models.UserPermissionsModel> q1 = q as List<Models.UserPermissionsModel>;

                upbm.PermissionTypes = new FullyObservableCollection<Models.UserPermissionTypesModel>();
                foreach(EnumValue ev in _standarduserpermissions)
                {
                    upbm.PermissionTypes.Add(new Models.UserPermissionTypesModel() { ID = ev.ID, UserPermissionType = ev.Description, IsChecked=false });
                }
                _userpermissionstree.Add(upbm);
            }            
        }

        private void CreateUserPermissions()
        {

        }

    }

  

}
