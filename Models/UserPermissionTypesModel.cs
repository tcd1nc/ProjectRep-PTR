
namespace PTR.Models
{
    public class zUserPermissionTypesModel :ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

        bool ischecked = false;
        public bool IsSelected
        {
            get { return ischecked; }
            set { SetField(ref ischecked, value); }
        }

        bool isenabled = false;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }
    }
}
