
namespace PTR.Models
{
    public class CustomReportModel : ModelBaseVM
    {
        string spname;
        public string SPName
        {
            get { return spname; }
            set { SetField(ref spname, value); }
        }

        bool combinetables;
        public bool CombineTables
        {
            get { return combinetables; }
            set { SetField(ref combinetables, value); }
        }
    }
}
