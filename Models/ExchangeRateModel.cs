using System;

namespace PTR.Models
{
    public class ExchangeRateModel :ViewModelBase
    {
        int id;
        DateTime? exratemonth;
        decimal exrate;
        int countryid;

        public int ID {
            get { return id; }
            set { SetField(ref id, value); }
        }

        public DateTime? ExRateMonth {
            get { return exratemonth; }
            set { SetField(ref exratemonth, value); }
        }

        public decimal ExRate {
            get { return exrate; }
            set { SetField(ref exrate, value); }
        }

        public int CountryID
        {
            get { return countryid; }
            set { SetField(ref countryid, value); }
        }
    }
}
