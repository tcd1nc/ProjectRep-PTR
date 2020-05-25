
namespace PTR.Models
{
    public class CustomerModel : ModelBaseVM
    {              
        string location;
        public string Location {
            get { return location; }
            set { SetField(ref location, value); }
        }

        int countryid;
        public int CountryID
        {
            get { return countryid; }
            set { SetField(ref countryid, value); }
        }

        string number;
        public string Number {
            get { return number; }
            set { SetField(ref number, value); }
        }

        int corpid;
        public int CorporateID
        {
            get { return corpid; }
            set { SetField(ref corpid, value); }
        }
                
        string culturecode;
        public string CultureCode
        {
            get { return culturecode; }
            set { SetField(ref culturecode, value); }
        }
               
        int salesregionid;
        public int SalesRegionID
        {
            get { return salesregionid; }
            set { SetField(ref salesregionid, value); }
        }

        string salesregionname;
        public string SalesRegionName
        {
            get { return salesregionname; }
            set { SetField(ref salesregionname, value); }
        }

        string country;
        public string CountryName
        {
            get { return country; }
            set { SetField(ref country, value); }
        }
    }
}
