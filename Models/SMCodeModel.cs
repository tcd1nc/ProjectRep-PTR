namespace PTR.Models
{
    public class SMCodeModel : ViewModelBase
    {
        int salesdivisionid;
        
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

        public int SalesDivisionID
        {
            get { return salesdivisionid; }
            set { SetField(ref salesdivisionid, value); }
        }
    }
}
