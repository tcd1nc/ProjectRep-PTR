namespace PTR.Models
{
    public class MarketSegmentModel :ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

        int industryid;
        public int IndustryID
        {
            get { return industryid; }
            set { SetField(ref industryid, value); }
        }

    }
}
