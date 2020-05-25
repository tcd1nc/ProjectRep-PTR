
namespace PTR.Models
{
    public class ApplicationModel : ModelBaseVM
    {
        int industryid;
        public int IndustryID {
            get { return industryid; }
            set { SetField(ref industryid, value); }
        }
                       
    }
}
