namespace PTR.Models
{
    public class SelectedItemModel :ViewModelBase
    {
        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int itemid;
        public int ItemID
        {
            get { return itemid; }
            set { SetField(ref itemid, value); }
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

        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }
    }
}
