
namespace PTR.Models
{
    public class LossSubCategoryModel : ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

        int losscategoryid;
        public int LossCategoryID
        {
            get { return losscategoryid; }
            set { SetField(ref losscategoryid, value); }
        }
                
        string losssubcategorydefinition;
        public string LossSubCategoryDefinition
        {
            get { return losssubcategorydefinition; }
            set { SetField(ref losssubcategorydefinition, value); }
        }
    }
}
