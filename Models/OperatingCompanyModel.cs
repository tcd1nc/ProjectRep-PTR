
namespace PTR.Models
{
    public class OperatingCompanyModel : ViewModelBase
    {
    //    GenericObjModel gom;
    //    public GenericObjModel GOM
    //    {
    //        get { return gom; }
    //        set { SetField(ref gom, value); }
    //    }
    //
    int id;
    public int ID
    {
        get { return id; }
        set { SetField(ref id, value); }
    }
   

    string name;
    public string Name
    {
        get { return name; }
        set { SetField(ref name, value); }
    }
}

}
