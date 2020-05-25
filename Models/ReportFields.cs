
namespace PTR.Models
{
    public class ReportFields : ModelBaseVM
    {

        string fieldname = string.Empty;
        public string FieldName
        {
            get { return fieldname; }
            set { SetField(ref fieldname, value); }
        }

        string datatype = string.Empty;
        public string DataType
        {
            get { return datatype; }
            set { SetField(ref datatype, value); }
        }

        string caption = string.Empty;
        public string Caption
        {
            get { return caption; }
            set { SetField(ref caption, value); }
        }

        string alignment = string.Empty;
        public string Alignment
        {
            get { return alignment; }
            set { SetField(ref alignment, value); }
        }

        string format = string.Empty;
        public string Format
        {
            get { return format; }
            set { SetField(ref format, value); }
        }

        int fieldtype;
        public int FieldType
        {
            get { return fieldtype; }
            set { SetField(ref fieldtype, value); }
        }

        bool system;
        public bool System
        {
            get { return system; }
            set { SetField(ref system, value); }
        }

        int datatypeid;
        public int DataTypeID
        {
            get { return datatypeid; }
            set { SetField(ref datatypeid, value); }
        }

    }
}
