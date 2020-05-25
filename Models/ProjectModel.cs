using System;

namespace PTR.Models
{
    public class ProjectModel : ModelBaseVM
    {
        
        #region Child Models

        int customerid;
        public int CustomerID
        {
            get { return customerid; }
            set { SetField(ref customerid, value); }
        }

        int ownerid;
        public int OwnerID
        {
            get { return ownerid; }
            set { SetField(ref ownerid, value); }
        }

        int creatorid;
        public int CreatorID
        {
            get { return creatorid; }
            set { SetField(ref creatorid, value); }
        }

        int salesdivisionid;
        public int SalesDivisionID
        {
            get { return salesdivisionid; }
            set { SetField(ref salesdivisionid, value); }
        }
              
        int industrysegmentid;
        public int IndustrySegmentID
        {
            get { return industrysegmentid; }
            set { SetField(ref industrysegmentid, value); }
        }

        int applicationid;
        public int ApplicationID
        {
            get { return applicationid; }
            set { SetField(ref applicationid, value); }
        }

        #endregion
                      
              
        decimal estimatedannualsales;
        public decimal EstimatedAnnualSales
        {
            get { return estimatedannualsales; }
            set { SetField(ref estimatedannualsales, value); }
        }

        decimal estimatedmpc;
        public decimal EstimatedAnnualMPC
        {
            get { return estimatedmpc; }
            set { SetField(ref estimatedmpc, value); }
        }
        
        int projectstatus;
        public int ProjectStatusID
        {
            get { return projectstatus; }
            set { SetField(ref projectstatus, value); }
        }
       
        string products;
        public string Products
        {
            get { return products; }
            set { SetField(ref products, value); }
        }

        string resources;
        public string Resources
        {
            get { return resources; }
            set { SetField(ref resources, value); }
        }
               
        DateTime? activateddate;
        public DateTime? ActivatedDate
        {
            get { return activateddate; }
            set { SetField(ref activateddate, value); }
        }

        DateTime? completeddate;
        public DateTime? CompletedDate
        {
            get { return completeddate; }
            set { SetField(ref completeddate, value); }
        }

        int targetedvolume;
        public int TargetedVolume
        {
            get { return targetedvolume; }
            set { SetField(ref targetedvolume, value); }
        }

        int newbusinesscategoryid;
        public int NewBusinessCategoryID
        {
            get { return newbusinesscategoryid; }
            set { SetField(ref newbusinesscategoryid, value); }
        }

        int salesstatusid;
        public int SalesStatusID
        {
            get { return salesstatusid; }
            set { SetField(ref salesstatusid, value); }
        }       

        decimal probofsuccess;
        public decimal ProbabilityOfSuccess
        {
            get { return probofsuccess; }
            set { SetField(ref probofsuccess, value); }
        }

        decimal gm;
        public decimal GM
        {
            get { return gm; }
            set { SetField(ref gm, value); }
        }

        int smcodeid;
        public int SMCodeID
        {
            get { return smcodeid; }
            set { SetField(ref smcodeid, value); }
        }

        int type;
        public int ProjectTypeID
        {
            get { return type; }
            set { SetField(ref type, value); }
        }

        bool kpm;
        public bool KPM
        {
            get { return kpm; }
            set { SetField(ref kpm, value); }
        }

        bool eprequired;
        public bool EPRequired
        {
            get { return eprequired; }
            set { SetField(ref eprequired, value); }
        }

        bool differentiatedtechnology;
        public bool DifferentiatedTechnology
        {
            get { return differentiatedtechnology; }
            set { SetField(ref differentiatedtechnology, value); }
        }

        FullyObservableCollection<MilestoneModel> milestones;
        public FullyObservableCollection<MilestoneModel> Milestones
        {
            get { return milestones; }
            set { SetField(ref milestones, value); }
        }

        FullyObservableCollection<EPModel> eps;
        public FullyObservableCollection<EPModel> EvaluationPlans
        {
            get { return eps; }
            set { SetField(ref eps, value); }
        }
              
        string comments;
        public string Comments
        {
            get { return comments; }
            set { SetField(ref comments, value); }
        }

        bool isnewbusiness;
        public bool IsNewBusiness
        {
            get { return isnewbusiness; }
            set { SetField(ref isnewbusiness, value); }
        }
                
        int incompletereasonid;
        public int IncompleteReasonID
        {
            get { return incompletereasonid; }
            set { SetField(ref incompletereasonid, value); }
        }

        int priorityid;
        public int PriorityID
        {
            get { return priorityid; }
            set { SetField(ref priorityid, value); }
        }

        int sponsorid;
        public int SponsorID
        {
            get { return sponsorid; }
            set { SetField(ref sponsorid, value); }
        }

        int miscdataid;
        public int MiscDataID
        {
            get { return miscdataid; }
            set { SetField(ref miscdataid, value); }
        }

        bool allownonowneredits;
        public bool AllowNonOwnerEdits
        {
            get { return allownonowneredits; }
            set { SetField(ref allownonowneredits, value); }
        }
        
        bool allownonownermilestoneaccess;
        public bool AllowNonOwnerMileStoneAccess
        {
            get { return allownonownermilestoneaccess; }
            set { SetField(ref allownonownermilestoneaccess, value); }
        }

        decimal unitcost;
        public decimal UnitCost
        {
            get { return unitcost; }
            set { SetField(ref unitcost, value); }
        }
    }
}
