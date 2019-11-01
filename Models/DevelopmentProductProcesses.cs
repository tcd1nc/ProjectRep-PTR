
namespace PTR.Models
{
    public class DevelopmentProductProcesses :ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

    }
}
