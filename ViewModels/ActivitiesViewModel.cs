using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using static PTR.StaticCollections;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows.Threading;

namespace PTR.ViewModels
{
    public class ActivitiesViewModel : ViewModelBase
    {
        int defaultcivalue = 1;
        bool isdirty = false;
        ProjectReportSummary currentproject;

        //private readonly Window windowref;

        public ActivitiesViewModel()
        {            
            LoadActivityStatusCodes();
            LoadTrialStatuses();

          //  windowref = winref;          
        }

        #region Event Handlers

        public delegate void MyPersonalizedUCEventHandler(bool param);
        public event MyPersonalizedUCEventHandler CanSave;

        public void RaiseMyEvent()
        {
            CanSave?.Invoke(!hasdupcomment);
        }

        private void MonthlyData_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            isdirty = true;

            MonthlyActivityStatusModel row = MonthlyData[e.CollectionIndex];
            row.IsDirty = true;

            CheckDupComments();

            row.ShowTrial = RequiredTrialStatuses.Contains(row.StatusID);

            //if StatusID = 10 and actualsalesforecast == null
            //prompt for updated actual sales forecast            
            if ((e.CollectionIndex == 0) && (e.PropertyName == "StatusID")) //only last row
            {
                if (row.StatusID == 10) //only Status 10
                {
                    if (!SelectedProjectItem.SalesForecastConfirmed) //only change it if it is not confirmed
                    {
                        IMessageBoxService msg1 = new MessageBoxService();
                        object[] values = new object[3];
                        values[0] = SelectedProjectItem.EstimatedAnnualSales;
                        values[1] = SelectedProjectItem.ExpectedDateNewSales;
                        if (MonthlyData[e.CollectionIndex].ExpectedDateFirstSales != null)
                            values[1] = (DateTime)MonthlyData[e.CollectionIndex].ExpectedDateFirstSales;

                        values[2] = SelectedProjectItem.CultureCode;
                        values = msg1.CompletedProjectDialog(null, values);
                        msg1 = null;
                        if (values != null)
                        {
                            SelectedProjectItem.EstimatedAnnualSales = ConvertObjToDecimal(values[0]);
                            SelectedProjectItem.ExpectedDateNewSales = ConvertObjToDate(values[1]);
                            UpdateActualForecastedSales(row.ProjectID, values, true);
                            SelectedProjectItem.SalesForecastConfirmed = true;
                            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            {
                                MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = (DateTime?)values[1];
                            }), null);
                        }
                        else
                        {
                            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                            {
                                MonthlyData.ItemPropertyChanged -= MonthlyData_ItemPropertyChanged;
                                MonthlyData[e.CollectionIndex].StatusID = defaultcivalue;
                                MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = null;
                                MonthlyData.ItemPropertyChanged += MonthlyData_ItemPropertyChanged;
                            }), null);
                            SelectedProjectItem.SalesForecastConfirmed = false;
                        }
                    }
                }
                else
                {
                    //if statusID is anything else then undo SalesForecastConfirmed if it was incorrectly set
                    if (SelectedProjectItem.SalesForecastConfirmed)
                    {
                        object[] values = new object[2];
                        values[0] = SelectedProjectItem.EstimatedAnnualSales;
                        values[1] = SelectedProjectItem.ExpectedDateNewSales;
                        UpdateActualForecastedSales(row.ProjectID, values, false);
                        SelectedProjectItem.SalesForecastConfirmed = false;
                    }

                    IMessageBoxService msg = new MessageBoxService();
                    DateTime newdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

                    if (MonthlyData[e.CollectionIndex].ExpectedDateFirstSales != null)
                        newdate = (DateTime)MonthlyData[e.CollectionIndex].ExpectedDateFirstSales;

                    DateTime? retvalue = msg.ConfirmExpectedSalesDateDialog(null, newdate);
                    msg = null;
                    if (retvalue == null)
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            MonthlyData.ItemPropertyChanged -= MonthlyData_ItemPropertyChanged;
                            MonthlyData[e.CollectionIndex].StatusID = defaultcivalue;
                            MonthlyData.ItemPropertyChanged += MonthlyData_ItemPropertyChanged;
                        }), null);
                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = DateTime.Now;
                        }), null);
                    }
                    else
                    {
                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                        {
                            MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = retvalue;
                        }), null);
                        UpdateForecastedSalesDate(row.ProjectID, (DateTime)retvalue);
                    }
                }
            }

            if ((e.CollectionIndex == 0) && (e.PropertyName == "ExpectedDateNewSales"))
            {
                DateTime newdate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                if (row.ExpectedDateFirstSales >= newdate)
                    UpdateForecastedSalesDate(row.ProjectID, (DateTime)row.ExpectedDateFirstSales);
                else
                    Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
                    {
                        MonthlyData[e.CollectionIndex].ExpectedDateFirstSales = DateTime.Now;
                    }), null);
            }
        }

        #endregion

        #region Properties


        public void ClearActivities()
        {
            MonthlyData?.Clear();
        }
        
        public ProjectReportSummary SelectedProjectItem
        {
            get { return currentproject; }
            set {
                
                UpdateMonthlyActivities();
                
                MonthlyData = GetMonthlyProjectData(value.ID);
                MonthlyData.ItemPropertyChanged += MonthlyData_ItemPropertyChanged;               
                if (MonthlyData?[0] != null)
                    defaultcivalue = MonthlyData[0].StatusID;

                UpdateAccess(value.CustomerID);
                SetField(ref currentproject, value);

                CheckDupComments();
                isdirty = false;
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

        bool showtrialstatus;
        public bool ShowTrialStatus
        {
            get { return showtrialstatus; }
            set { SetField(ref showtrialstatus, value); }
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

        Collection<GenericObjModel> trialstatuses;
        public Collection<GenericObjModel> TrialStatuses
        {
            get { return trialstatuses; }
            set { SetField(ref trialstatuses, value); }
        }

        bool enableactivities;
        public bool EnableActivities
        {
            get { return enableactivities; }
            set { SetField(ref enableactivities, value); }
        }

        #endregion

        #region Private functions

        private void UpdateAccess(int customerid)
        {
            int accessid = StaticCollections.GetUserCustomerAccess(customerid);
            if (accessid == (int)UserPermissionsType.FullAccess)
                EnableActivities = true;
            else            
                if (accessid == (int)UserPermissionsType.ReadOnly)
                    EnableActivities = false;
                else
                //EnableActivities = false;
                    if (accessid == (int)UserPermissionsType.EditAct)
                        EnableActivities = true;
                    else
                        EnableActivities = false;
            
        }

        private void LoadActivityStatusCodes()
        {
            StatusCodes = ActivityStatusCodes;
        }

        private void CommentError(bool iserror)
        {
            if (iserror)
            {
                CommentErrorColour = "Red";
                CommentHeading = "Duplicate Comments";
            }
            else
            {
                CommentErrorColour = "Black";
                CommentHeading = "Comments";
            }
        }

        private void LoadTrialStatuses()
        {
            trialstatuses = new Collection<GenericObjModel>();
            var c = EnumerationLists.TrialStatusTypesList;
            foreach (EnumValue ag in c)
            {
                trialstatuses.Add(new GenericObjModel()
                {
                    ID = Convert.ToInt32(ag.Enumvalue),
                    Name = ag.Description
                }
                );
            }
            TrialStatuses = trialstatuses;
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
            RaiseMyEvent();
            return hasdupcomment;
        }

        #endregion

        #region Commands

        ICommand clearstatus;
        public ICommand ClearStatus
        {
            get
            {
                if (clearstatus == null)
                    clearstatus = new DelegateCommand(CanExecute, ExecuteClearStatus);
                return clearstatus;
            }
        }

        private void ExecuteClearStatus(object parameter)
        {
            if ((parameter as MonthlyActivityStatusModel).StatusID > 0)
            {
                MonthlyData.ItemPropertyChanged -= MonthlyData_ItemPropertyChanged;
                (parameter as MonthlyActivityStatusModel).StatusID = 0;
                MonthlyData.ItemPropertyChanged += MonthlyData_ItemPropertyChanged;
                isdirty = true;
            }
        }

        ICommand cleartrialstatus;
        public ICommand ClearTrialStatus
        {
            get
            {
                if (cleartrialstatus == null)
                    cleartrialstatus = new DelegateCommand(CanExecute, ExecuteClearTrialStatus);
                return cleartrialstatus;
            }
        }

        private void ExecuteClearTrialStatus(object parameter)
        {
            (parameter as MonthlyActivityStatusModel).TrialStatusID = (int)TrialStatusType.NoTrial;
        }

        public void UpdateMonthlyActivities()
        {
            if (isdirty && !HasDupComments)
            {
                try
                {
                    if (MonthlyData != null)
                    {
                        UpdateMonthlyActivityStatus(MonthlyData);
                    }
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
                }
            }
        }
              
        #endregion

    }
}
