
namespace PTR.Models
{
    public class AccessPermissionModel : ViewModelBase
    {
        int _id;
        public int ID
        {
            get { return _id; }
            set { SetField(ref _id, value); }
        }

        int _associateid;
        public int AssociateID
        {
            get { return _associateid; }
            set { SetField(ref _associateid, value); }
        }


        int _operatingcompanyid;
        public int OperatingCompanyID
        {
            get { return _operatingcompanyid; }
            set { SetField(ref _operatingcompanyid, value); }
        }

        int _countryid;
        public int CountryID
        {
            get { return _countryid; }
            set { SetField(ref _countryid, value); }
        }

        int _accesspermissiontypeid;
        public int AccessPermissionTypeID
        {
            get { return _accesspermissiontypeid; }
            set { SetField(ref _accesspermissiontypeid, value); }
        }
    }
}
