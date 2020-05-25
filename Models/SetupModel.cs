
using System;

namespace PTR.Models
{
    public class SetupModel : ViewModelBase
    {
        string emailformat;
        string domain;
        int maxProjectNameLength;
        bool ePRequired;
        int defaultTrialStatusID;
        int statusIDforTrials;
        bool validateProducts;
        bool colouriseplaybookreport;
        string defaultsalesstatuses;
        char productdelimiter;
        DateTime defaultmasterliststartmonth;
        bool disablepreviousmonths;
        public string Emailformat { get { return emailformat;} set { SetField(ref emailformat, value); }}
        public string Domain { get { return domain; } set { SetField(ref domain, value); } }
        public int MaxProjectNameLength { get { return maxProjectNameLength; } set { SetField(ref maxProjectNameLength, value); } }
        public bool EPRequired { get { return ePRequired; } set { SetField(ref ePRequired, value); } }
        public int DefaultTrialStatusID { get { return defaultTrialStatusID; } set { SetField(ref defaultTrialStatusID, value); } }
        public int StatusIDforTrials { get { return statusIDforTrials; } set { SetField(ref statusIDforTrials, value); } }
        public bool ValidateProducts { get { return validateProducts; } set { SetField(ref validateProducts, value); } }
        public bool ColourisePlaybookReport { get { return colouriseplaybookreport; } set { SetField(ref colouriseplaybookreport, value); } }
        public string DefaultSalesStatuses { get { return defaultsalesstatuses; } set { SetField(ref defaultsalesstatuses, value); } }
        public char ProductDelimiter { get { return productdelimiter; } set { SetField(ref productdelimiter, value); } }
        public DateTime DefaultMasterListStartMonth { get { return defaultmasterliststartmonth; } set { SetField(ref defaultmasterliststartmonth, value); } }
        public bool DisablePreviousMonths { get { return disablepreviousmonths; } set { SetField(ref disablepreviousmonths, value); } }

    }
}
