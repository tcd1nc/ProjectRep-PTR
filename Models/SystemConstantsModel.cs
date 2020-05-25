
namespace PTR.Models
{
    public class SystemConstantsModel : ViewModelBase
    {
        int id = 0;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        string constantname = string.Empty;
        public string ConstantName
        {
            get { return constantname; }
            set { SetField(ref constantname, value); }
        }

        string constantvalue = string.Empty;
        public string ConstantValue
        {
            get { return constantvalue; }
            set { SetField(ref constantvalue, value); }
        }
    }
}
