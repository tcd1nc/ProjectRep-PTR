using System;

namespace PTR.Models
{
    public class MilestoneModel : ModelBaseVM
    {
        public int CustomerID { get; set; }       

        DateTime? targetdate;
        public DateTime? TargetDate
        {
            get { return targetdate; }
            set { SetField(ref targetdate, value); }
        }

        DateTime? completeddate;
        public DateTime? CompletedDate
        {
            get { return completeddate; }
            set { SetField(ref completeddate, value); }
        }
        
        int userid;
        public int UserID
        {
            get { return userid; }
            set { SetField(ref userid, value); }
        }

        int projectid;
        public int ProjectID
        {
            get { return projectid; }
            set { SetField(ref projectid, value); }
        }

        string username;
        public string UserName
        {
            get { return username; }
            set { SetField(ref username, value); }
        }
        
    }
}
