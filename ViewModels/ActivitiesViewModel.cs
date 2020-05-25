using System;
using System.Collections.ObjectModel;
using static PTR.StaticCollections;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Data;
using System.Linq;
using System.Collections.Generic;

namespace PTR.ViewModels
{
    public class ActivitiesViewModel : ViewModelBase
    {
        const int defaultstatusid = 1;
        bool isdirty = false;
        DataRowView currentproject;

       // private readonly Window windowref;

        public ActivitiesViewModel()
        {            
            LoadActivityStatusCodes();
            LoadTrialStatuses();
            //StatusIDforTrials = Config.StatusIDforTrials;
            //  windowref = winref;          
        }

        //public Window ParentWindow { get; set; }
        

        #region Event Handlers

        public class CanSaveEventArgs : EventArgs { public bool CanSave { get; set; } }
        public delegate void MyPersonalizedUCEventHandler(object sender, CanSaveEventArgs e);
        public event MyPersonalizedUCEventHandler CanSave;

        public void RaiseCanSaveEvent(bool cansave)
        {
            //CanSave?.Invoke(!hasdupcomment);
            CanSaveEventArgs e = new CanSaveEventArgs
            {
                CanSave = cansave
            };
            CanSave?.Invoke(this, e);
        }

        public class IsDirtyDataEventArgs : EventArgs { public bool IsDirtyData { get; set; } }
        public delegate void DirtyDataUCEventHandler(object sender, IsDirtyDataEventArgs e);
        public event DirtyDataUCEventHandler IsDirtyData;

        public void RaiseDirtyDataEvent()
        {
            //IsDirtyData?.Invoke(isdirty);
            IsDirtyDataEventArgs e = new IsDirtyDataEventArgs
            {
                IsDirtyData = isdirty
            };
            IsDirtyData?.Invoke(this, e);
        }
        

        public class ProjectStatusEventArgs : EventArgs { public ProjectStatusType ProjectStatus { get; set; } }
        public delegate void ProjectStatusEventHandler(object sender, ProjectStatusEventArgs e);
        public event ProjectStatusEventHandler ProjectStatus;

        public void RaiseProjectStatusEvent(ProjectStatusType status)
        {
            ProjectStatusEventArgs e = new ProjectStatusEventArgs
            {
                ProjectStatus = status
            };
            ProjectStatus?.Invoke(this, e);
        }
                
        private int GetLastStatus10Row()
        {
            int ctr = -1;
            for (int i = 0; i < MonthlyData.Count; i++)
            {
                if (MonthlyData[i].StatusID == 10)
                {
                    ctr = i;
                    break;
                }
            }
            return ctr;
        }

        private void SetStatus10Rows()
        {
            int stat10rowindex = GetLastStatus10Row();
            bool foundnonstat = false;
            for (int i = 0; i < MonthlyData.Count; i++)
            {
                if (!foundnonstat)
                {
                    if (MonthlyData[i].StatusID < 1)                    
                        MonthlyData[i].ShowStatus10 = true;                    
                    else
                    {
                       foundnonstat = true;
                       MonthlyData[i].ShowStatus10 = true;                                                                          
                    }
                }
                else                
                    MonthlyData[i].ShowStatus10 = false;
                
                MonthlyData[i].IsEnabled = true;
                if (i < stat10rowindex)                
                    MonthlyData[i].IsEnabled = false;                
                else                
                    MonthlyData[i].IsEnabled = true;                
            }                      
        }

        private void DisablePreviousMonthRows()
        {
            if (!CurrentUser.Administrator && Config.DisablePreviousMonths)
            {
                DateTime currentmonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                for (int i = 0; i < MonthlyData.Count; i++)
                {                    
                    if (MonthlyData[i].StatusMonth < currentmonth)
                        MonthlyData[i].IsEnabled = false;                   
                }
            }
        }


        private void MonthlyData_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            MonthlyActivityStatusModel row = MonthlyData[e.CollectionIndex];

            //dont trigger isdirty on load
            if (e.PropertyName == "StatusID" || e.PropertyName == "Comments" || e.PropertyName == "ExpectedDateFirstSales" || e.PropertyName == "TrialStatusID")
            {
                isdirty = true;
                RaiseDirtyDataEvent();
                row.IsDirty = true;
                CheckDupComments();
            }
                                           
            //make changes based on selected status            
            if (e.PropertyName == "StatusID")
            {                
                if (row.StatusID == 10) //only Status 10
                {
                    IMessageBoxService msg1 = new MessageBoxService();
                    object[] values = new object[2];                    
                    values = msg1.CompletedProjectDialog(null, ConvertObjToDecimal(SelectedProjectItem["EstimatedAnnualSales"]), MonthlyData[e.CollectionIndex].ExpectedDateFirstSales, (DateTime)MonthlyData[e.CollectionIndex].StatusMonth);
                    msg1 = null;
                    if (values != null)
                    {                                                                
                        MonthlyData[e.CollectionIndex].EstimatedAnnualSales = ConvertObjToDecimal(values[0]);
                        MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = ConvertObjToDate(values[1]);                                        
                        //raise event - Completed
                        RaiseProjectStatusEvent(ProjectStatusType.Completed);
                    }                               
                }
                else  //all other statuses
                {                                  
                    DateTime newdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    if (MonthlyData[e.CollectionIndex].ExpectedDateFirstSales != null)
                        newdate = (DateTime)MonthlyData[e.CollectionIndex].ExpectedDateFirstSales;

                    IMessageBoxService msg = new MessageBoxService();
                    DateTime? retvalue = msg.ConfirmExpectedSalesDateDialog(null, newdate, (DateTime)MonthlyData[e.CollectionIndex].StatusMonth);
                    msg = null;
                    if (retvalue == null)
                        retvalue = DateTime.Now.AddMonths(1);
                                                                               
                    MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = retvalue;                                           
                    //raise event - Active               
                    RaiseProjectStatusEvent(ProjectStatusType.Active);
                }
               
                //set default trial status               
                if (MonthlyData[e.CollectionIndex].StatusID != Config.StatusIDforTrials)                
                    MonthlyData[e.CollectionIndex].TrialStatusID = 0;
                else
                    MonthlyData[e.CollectionIndex].TrialStatusID = Config.DefaultTrialStatusID;
                
                //Set rows to disabled for later months if this row is status=10
                SetStatus10Rows();
                DisablePreviousMonthRows();
            }                                   
        }
               
        #endregion

        #region Properties

        //int statusidfortrials = 9;
        //public int StatusIDforTrials
        //{
        //    get { return statusidfortrials; }
        //    set { SetField(ref statusidfortrials, value); }
        //}
               
        public void ClearActivities()
        {
            MonthlyData?.Clear();
        }
        
        public DataRowView SelectedProjectItem
        {
            get { return currentproject; }
            set {
                
                UpdateMonthlyActivities();

                MonthlyData = GetMonthlyProjectData(ConvertObjToInt(value["ProjectID"]));
                MonthlyData.ItemPropertyChanged += MonthlyData_ItemPropertyChanged;               

                SetField(ref currentproject, value);

                //FilterActivityStatusCodes();

                CheckDupComments();
                isdirty = false;
                //new for version 3.2
                SetActivityAccess(ConvertObjToInt(SelectedProjectItem["AccessID"]));
                SetStatus10Rows();


                DisablePreviousMonthRows();

                RaiseDirtyDataEvent();
            }
        }

        string commenterrorcolour = "Black";
        public string CommentErrorColour
        {
            get { return commenterrorcolour; }
            set { SetField(ref commenterrorcolour, value); }
        }

        string commentheading;
        public string CommentHeading
        {
            get { return commentheading; }
            set { SetField(ref commentheading, value); }
        }

        FullyObservableCollection<MonthlyActivityStatusModel> monthlydatatable = new FullyObservableCollection<MonthlyActivityStatusModel>();
        public FullyObservableCollection<MonthlyActivityStatusModel> MonthlyData
        {
            get { return monthlydatatable; }
            set { SetField(ref monthlydatatable, value); }
        }

        Collection<ActivityStatusCodesModel> statuscodes;
        public Collection<ActivityStatusCodesModel> StatusCodes
        {
            get { return statuscodes; }
            set { SetField(ref statuscodes, value); }
        }

        Collection<TrialStatusModel> trialstatuses;
        public Collection<TrialStatusModel> TrialStatuses
        {
            get { return trialstatuses; }
            set { SetField(ref trialstatuses, value); }
        }

        bool enableactivities = false;
        public bool EnableActivities
        {
            get { return enableactivities; }
            set { SetField(ref enableactivities, value); }
        }

        #endregion

        #region Private functions

        private void SetActivityAccess(int accessid)
        {
            if (SelectedProjectItem["ProjectStatus"].ToString() == EnumerationManager.GetEnumDescription(ProjectStatusType.Active))
            {
                if (accessid == (int)UserPermissionsType.FullAccess)
                    EnableActivities = true;
                else
                    if (accessid == (int)UserPermissionsType.ReadOnly)
                    EnableActivities = false;
                else
                    if (accessid == (int)UserPermissionsType.EditAct)
                    EnableActivities = true;
                else
                    EnableActivities = false;
            }
            else
                EnableActivities = false;

            //prevent non-owners from editing
            if (CurrentUser.ID != ConvertObjToInt(SelectedProjectItem["OwnerID"]) && ConvertObjToBool(SelectedProjectItem["AllowNonOwnerEdits"]) == false)            
                EnableActivities = false;
            
            if (CurrentUser.Administrator)
                EnableActivities = true;

            
        }
        
        private void LoadActivityStatusCodes()
        {
            StatusCodes = ActivityStatusCodes;

            //FilterActivityStatusCodes();
        }

        //private void FilterActivityStatusCodes()
        //{
        //    if (SelectedProjectItem != null)
        //    {
        //        FullyObservableCollection<ActivityStatusCodesModel> ActivitiesColl = new FullyObservableCollection<ActivityStatusCodesModel>();
        //        //get project type             
        //        string q = ProjectTypes.Where(x => x.ID == ConvertObjToInt(SelectedProjectItem["ProjectTypeID"])).Select(x => x.ActivityCodes).FirstOrDefault();
        //        List<string> activities = q.Split(',').ToList();
        //        foreach (ActivityStatusCodesModel am in ActivityStatusCodes)
        //        {
        //            if (activities.Contains(am.ID.ToString()))
        //                ActivitiesColl.Add(am);
        //        }
        //        StatusCodes = ActivitiesColl;
        //    }
        //}


        private void CommentError(bool iserror)
        {
            if (iserror)
            {
                CommentErrorColour = "Red";
                CommentHeading = "Repeated Comments for Last Two Months";
            }
            else
            {
                CommentErrorColour = "Black";
                CommentHeading = "Comments";
            }
        }

        private void LoadTrialStatuses()
        {
            TrialStatuses = StaticCollections.TrialStatuses;           
        }

        bool hasdupcomment = false;
        public bool HasDupComments
        {
            get { return hasdupcomment; }
            set { SetField(ref hasdupcomment, value); }
        }
      
        private bool CheckDupComments()
        {

            if (MonthlyData.Count > 1)
            {
                if (MonthlyData[0].Comments.ToUpper().Trim() == MonthlyData[1].Comments.ToUpper().Trim())
                    hasdupcomment = true;
                else
                    hasdupcomment = false;
            }
            else
                hasdupcomment = false;

            CommentError(hasdupcomment);
            HasDupComments = hasdupcomment;
            RaiseCanSaveEvent(!HasDupComments);
            return hasdupcomment;
        }

        #endregion

        #region Commands
                
        public void UpdateMonthlyActivities()
        {
            if (isdirty)
            {
                try
                {
                    if (MonthlyData != null)                    
                        MonthlyData = UpdateMonthlyActivityStatus(MonthlyData);                    
                }
                catch (Exception e)
                {
                    IMessageBoxService msg = new MessageBoxService();
                    msg.ShowMessage("Unable to save. " + e.Message, "Saving data failed", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                    msg = null;
                }
                finally
                {
                    isdirty = false;
                    foreach (MonthlyActivityStatusModel m in MonthlyData)
                        m.IsDirty = false;
                    RaiseDirtyDataEvent();
                }
            }
        }
              
        #endregion

    }
}
