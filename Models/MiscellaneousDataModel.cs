
namespace PTR.Models
{
    public class MiscellaneousDataModel : ViewModelBase
    {
        int id = 0;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int fkid = 0;
        public int FKID
        {
            get { return fkid; }
            set { SetField(ref fkid, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        bool ischecked;
        public bool IsChecked
        {
            get { return ischecked; }
            set { SetField(ref ischecked, value); }
        }

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }
    }
}
