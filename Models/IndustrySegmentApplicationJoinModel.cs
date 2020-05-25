
namespace PTR.Models
{
    public class IndustrySegmentApplicationJoinModel : ModelBaseVM
    {
        int industrysegmentid;
        public int IndustrySegmentID        {
            get { return industrysegmentid; }
            set { SetField(ref industrysegmentid, value); }
        }

        int Applicationid;
        public int ApplicationID
        {
            get { return Applicationid; }
            set { SetField(ref Applicationid, value); }
        }

        int industrysegmentindustryid;
        public int IndustrySegmentIndustryID
        {
            get { return industrysegmentindustryid; }
            set { SetField(ref industrysegmentindustryid, value); }
        }

    }
}
