
namespace PTR.Models
{
    public class ActivityStatusCodesModel : ModelBaseVM
    {                       
        string colour;
        public string Colour {
            get { return colour; }
            set { SetField(ref colour, value); }
        }
               
        string pbdescription;
        public string PlaybookDescription
        {
            get { return pbdescription; }
            set { SetField(ref pbdescription, value); }
        }

    }
}
