
namespace PTR.Models
{
    public class AMPImplementationLevelsModel : ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

    }
}
