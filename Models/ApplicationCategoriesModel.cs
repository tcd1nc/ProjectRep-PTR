
namespace PTR.Models
{
    public class ApplicationCategoriesModel :ViewModelBase
    {
        int industryid;
        public int IndustryID {
            get { return industryid; }
            set { SetField(ref industryid, value); }
        }

        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }
    }
}
