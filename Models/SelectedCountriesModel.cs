
namespace PTR.Models
{
    public class zSelectedCountriesModel : ViewModelBase
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

        bool isselected;
        public bool IsSelected
        {
            get { return isselected; }
            set { SetField(ref isselected, value); }
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
