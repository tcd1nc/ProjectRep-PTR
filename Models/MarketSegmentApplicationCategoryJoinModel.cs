
namespace PTR.Models
{
    public class MarketSegmentApplicationCategoryJoinModel : ModelBaseVM
    {
        int marketsegmentid;
        public int MarketSegmentID        {
            get { return marketsegmentid; }
            set { SetField(ref marketsegmentid, value); }
        }

        int applicationcategoryid;
        public int ApplicationCategoryID
        {
            get { return applicationcategoryid; }
            set { SetField(ref applicationcategoryid, value); }
        }

        int marketsegmentindustryid;
        public int MarketSegmentIndustryID
        {
            get { return marketsegmentindustryid; }
            set { SetField(ref marketsegmentindustryid, value); }
        }

    }
}
