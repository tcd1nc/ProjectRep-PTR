namespace PTR.Models
{
    public class IndustrySegmentModel : ModelBaseVM
    {       
        int industryid;
        public int IndustryID
        {
            get { return industryid; }
            set { SetField(ref industryid, value); }
        }

    }
}
