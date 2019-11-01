namespace PTR.Models
{
    public class SalesRegionModel : ViewModelBase
    {
        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int countryid;
        public int CountryID
        {
            get { return countryid; }
            set { SetField(ref countryid, value); }
        }
        
        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        bool deleted;
        public bool Deleted
        {
            get { return deleted; }
            set { SetField(ref deleted, value); }
        }

        FullyObservableCollection<TreeViewNodeModel> customers;
        public FullyObservableCollection<TreeViewNodeModel> Customers
        {
            get { return customers; }
            set { SetField(ref customers, value); }
        }

        bool selected;
        public bool IsSelected
        {
            get { return selected; }
            set { SetField(ref selected, value); }
        }

        bool enabled;
        public bool IsEnabled
        {
            get { return enabled; }
            set { SetField(ref enabled, value); }
        }

    }
}
