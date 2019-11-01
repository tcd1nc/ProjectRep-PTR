
namespace PTR.Models
{
    public class ActivityStatusCodesModel :ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }
               
        string colour;
        public string Colour {
            get { return colour; }
            set { SetField(ref colour, value); }
        }

        bool reqtrialstatus;
        public bool ReqTrialStatus
        {
            get { return reqtrialstatus; }
            set { SetField(ref reqtrialstatus, value); }
        }

        string pbdescription;
        public string PlaybookDescription
        {
            get { return pbdescription; }
            set { SetField(ref pbdescription, value); }
        }

    }
}
