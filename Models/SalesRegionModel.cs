namespace PTR.Models
{
    public class SalesRegionModel : ModelBaseVM
    {
       
        int countryid;
        public int CountryID
        {
            get { return countryid; }
            set { SetField(ref countryid, value); }
        }
                             
        FullyObservableCollection<TreeViewNodeModel> customers;
        public FullyObservableCollection<TreeViewNodeModel> Customers
        {
            get { return customers; }
            set { SetField(ref customers, value); }
        }
               
    }
}
