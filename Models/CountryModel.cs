namespace PTR.Models
{
    public class CountryModel : ModelBaseVM
    {
        int operatingcompanyid;
        public int OperatingCompanyID
        {
            get { return operatingcompanyid; }
            set { SetField(ref operatingcompanyid, value); }
        }

        string culturecode;
        public string CultureCode
        {
            get { return culturecode; }
            set { SetField(ref culturecode, value); }
        }

        bool useusd;
        public bool UseUSD
        {
            get { return useusd; }
            set { SetField(ref useusd, value); }
        }

        FullyObservableCollection<TreeViewNodeModel> salesregions;
        public FullyObservableCollection<TreeViewNodeModel> SalesRegions
        {
            get { return salesregions; }
            set { SetField(ref salesregions, value); }
        }

    }

}