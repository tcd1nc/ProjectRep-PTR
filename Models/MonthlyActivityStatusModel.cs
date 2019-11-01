using System;

namespace PTR.Models
{
    public class MonthlyActivityStatusModel : ViewModelBase
    {
        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }

        int projectid;
        public int ProjectID {
            get { return projectid; }
            set { SetField(ref projectid, value); }
        }

        DateTime? statusmonth;
        public DateTime? StatusMonth {
            get { return statusmonth; }
            set { SetField(ref statusmonth, value); }
        }

        int statusid;
        public int StatusID {
            get { return statusid; }
            set { SetField(ref statusid, value); }
        }

        string comments;
        public string Comments {
            get { return comments; }
            set { SetField(ref comments, value); }
        }

        DateTime? expecteddatefirstsales;
        public DateTime? ExpectedDateFirstSales
        {
            get { return expecteddatefirstsales; }
            set { SetField(ref expecteddatefirstsales, value); }
        }

        int trialstatusid;
        public int TrialStatusID
        {
            get { return trialstatusid; }
            set { SetField(ref trialstatusid, value); }
        }

        bool showtrial;
        public bool ShowTrial
        {
            get { return showtrial; }
            set { SetField(ref showtrial, value); }
        }
        
        bool isdirty;
        public bool IsDirty
        {
            get { return isdirty; }
            set { SetField(ref isdirty, value); }
        }

    }
}
