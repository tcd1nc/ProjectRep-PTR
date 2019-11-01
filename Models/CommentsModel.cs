using System;

namespace PTR.Models
{
    public class CommentsModel :ViewModelBase
    {
        GenericObjModel gom;
        public GenericObjModel GOM
        {
            get { return gom; }
            set { SetField(ref gom, value); }
        }

        int parentid;
        public int ParentID {
            get { return parentid; }
            set { SetField(ref parentid, value); }
        }
                                
        DateTime? commentmonth;
        public DateTime? CommentMonth {
            get { return commentmonth; }
            set { SetField(ref commentmonth, value); }
        }
    }
}
