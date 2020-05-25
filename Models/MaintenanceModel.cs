using System;

namespace PTR.Models
{
    public class MaintenanceModel : ModelBaseVM
    {
        
        string username;
        public string UserName
        {
            get { return username; }
            set { SetField(ref username, value); }
        }

        string projectname;
        public string ProjectName
        {
            get { return projectname; }
            set { SetField(ref projectname, value); }
        }

        string email;
        public string Email
        {
            get { return email; }
            set { SetField(ref email, value); }
        }

        //bool selected;
        //public bool Selected
        //{
        //    get { return selected; }
        //    set { SetField(ref selected, value); }
        //}

        bool enabled;
        public bool Enabled
        {
            get { return enabled; }
            set { SetField(ref enabled, value); }
        }

        int customerid;
        public int CustomerID
        {
            get { return customerid; }
            set { SetField(ref customerid, value); }
        }

        int projectid;
        public int ProjectID
        {
            get { return projectid; }
            set { SetField(ref projectid, value); }
        }

        DateTime? actiondate;
        public DateTime? ActionDate
        {
            get { return actiondate; }
            set { SetField(ref actiondate, value); }
        }

        MaintenanceType mainttypeid;
        public MaintenanceType MaintenanceTypeID
        {
            get { return mainttypeid; }
            set { SetField(ref mainttypeid, value); }
        }
    }
}
