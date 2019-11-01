
namespace PTR.Models
{
    public class zTrialStatusModel : ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }
    }
}
