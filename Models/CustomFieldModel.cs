
namespace PTR.Models
{
    public class CustomFieldModel : ViewModelBase
    {

        int id = 0;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        string name = string.Empty;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        string label = string.Empty;
        public string Label
        {
            get { return label; }
            set { SetField(ref label, value); }
        }

        bool required = false;
        public bool Required
        {
            get { return required; }
            set { SetField(ref required, value); }
        }

        string xvalue = string.Empty;
        public string StrValue
        {
            get { return xvalue; }
            set { SetField(ref xvalue, value); }
        }

        int customfieldtypeid = 0;
        public int CustomFieldTypeID
        {
            get { return customfieldtypeid; }
            set { SetField(ref customfieldtypeid, value); }
        }

        int customfieldnameid = 0;
        public int CustomFieldNameID
        {
            get { return customfieldnameid; }
            set { SetField(ref customfieldnameid, value); }
        }
    }
}
