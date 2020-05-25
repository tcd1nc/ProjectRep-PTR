
namespace PTR.Models
{
    public class ModelBaseVM : ViewModelBase
    {
        int id = 0;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        string description = string.Empty;
        public string Description
        {
            get { return description; }
            set { SetField(ref description, value); }
        }

        bool deleted = false;
        public bool Deleted
        {
            get { return deleted; }
            set { SetField(ref deleted, value); }
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

        bool isselected;
        public bool IsSelected
        {
            get { return isselected; }
            set { SetField(ref isselected, value); }
        }

    }
}
