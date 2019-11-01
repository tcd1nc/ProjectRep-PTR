using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTR.Models
{
    public class UserCustomerAccessModel : ViewModelBase
    {
        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int userid;
        public int UserID
        {
            get { return userid; }
            set { SetField(ref userid, value); }
        }

        int customerid;
        public int CustomerID
        {
            get { return customerid; }
            set { SetField(ref customerid, value); }
        }

        int accessid;
        public int AccessID
        {
            get { return accessid; }
            set { SetField(ref accessid, value); }
        }

        int countryid;
        public int CountryID
        {
            get { return countryid; }
            set { SetField(ref countryid, value); }
        }

    }
}
