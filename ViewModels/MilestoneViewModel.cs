using System;
using System.Windows.Input;
using PTR.Models;
using System.Text;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class MilestoneViewModel : ViewModelBase
    {
        private const string title = "Milestone";
        public ICommand CopyMilestone { get; set; }
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;

        public MilestoneViewModel(int id, int projectid)
        {
            if (id == 0)
            {
                Milestone = new MilestoneModel()
                {
                    Description = string.Empty,
                    TargetDate = DateTime.Now.AddMonths(1),
                    ProjectID = projectid
                };
                IsEnabled = true;
            }
            else
            {
                Milestone = GetMilestone(id);
                SetUserAccessExistingMilestone(Milestone.CustomerID);
            }
            Milestone.PropertyChanged += Milestone_PropertyChanged;                  

            FullyObservableCollection<UserModel> associatess = GetUsers();
            foreach (UserModel ag in associatess)
            {
                if (!ag.Deleted)
                {
                    users.Add(new UserModel()
                    {
                        ID = ag.ID,
                        Name = ag.Name,
                        LoginName = ag.LoginName,
                        Administrator = ag.Administrator
                    });
                }
            }
            canclearcompleteddate = Milestone.CompletedDate != null;

            if (id == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (ID: " + id.ToString() + ", Project ID: " + projectid.ToString() + ")";

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            CopyMilestone = new RelayCommand(ExecuteCopyMilestone, CanExecuteCopyMilestone);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
           
                        
        }

        #region Event Handlers

        private void Milestone_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            isdirty = true;

            if(e.PropertyName == "CompletedDate")
                canclearcompleteddate = true;
        }

        #endregion

        #region View Properties
              
        string windowtitle;
        public string WindowTitle
        {
            get { return windowtitle; }
            set { SetField(ref windowtitle, value); }
        }

        MilestoneModel milestone;
        public MilestoneModel Milestone
        {
            get { return milestone; }
            set { SetField(ref milestone, value); }
        }

        FullyObservableCollection<UserModel> users = new FullyObservableCollection<UserModel>();
        public FullyObservableCollection<UserModel> Users
        {
            get { return users; }
            set { SetField(ref users, value); }
        }

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        bool returncode = false;
        public bool ReturnObject
        {
            get { return returncode; }
            set { SetField(ref returncode, value); }
        }

        #endregion

        #region Private functions

        private void SetUserAccessExistingMilestone(int customerid)
        {
            int accessid = StaticCollections.GetUserCustomerAccess(customerid);
            if (accessid == (int)UserPermissionsType.FullAccess)
                IsEnabled = true;
            else
            if (accessid == (int)UserPermissionsType.ReadOnly)
                IsEnabled = false;
            else
                IsEnabled = false;

            canexecutesave = IsEnabled;
        }

        private bool HasOwner()
        {
            return (Milestone.UserID != 0);
        }

        private void SetClipboard(MilestoneModel milestone)
        {
            StringBuilder sbhtml = new StringBuilder();

            sbhtml.Append("<p style='font-size:14px;font-family:Arial'><b>");
            sbhtml.Append("Assigned Person:&nbsp;</b>");
            sbhtml.Append(milestone.UserName);
            sbhtml.Append("</p>");

            sbhtml.Append("<p style='font-size:14px;font-family:Arial'><b>");
            sbhtml.Append("Description:</b></p>");
            sbhtml.Append("<p style='font-size:14px;font-family:Arial'>");
            sbhtml.Append(milestone.Description);
            sbhtml.Append("</p><br/>");

            sbhtml.Append("<p style='font-size:14px;font-family:Arial'><b>");
            sbhtml.Append("Due Date:&nbsp;</b>");
            sbhtml.Append(milestone.TargetDate.Value.ToString("dd-MMM-yyyy"));
            sbhtml.Append("</p>");                      

            StringBuilder sbtext = new StringBuilder();
            sbtext.Append("Assigned Person: ");
            sbtext.Append(milestone.UserName);
            sbtext.Append("\n");
            sbtext.Append("Description:\n");
            sbtext.Append(milestone.Description);
            sbtext.Append("\n\n");
            sbtext.Append("Due Date: ");
            sbtext.Append(milestone.TargetDate.Value.ToString("dd-MMM-yyyy"));

            ClipboardHelper.CopyToClipboard(sbhtml.ToString(), sbtext.ToString());

        }

        #endregion

        #region Commands

        private bool CanExecuteSave(object obj)
        {
            if (!isdirty)
                return false;

            if (!HasOwner())
                return false;

            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveMilestone();
            ReturnObject = true;
            CloseWindow();
        }

        private void SaveMilestone()
        {
            if (isdirty)
            { 
                if (Milestone.ID > 0)
                    UpdateMilestone(Milestone);
                else
                    Milestone.ID = AddMilestone(Milestone);
                isdirty = false;
            }
        }
             
        ICommand clearcompleteddate;
        bool canclearcompleteddate = true;
        private bool CanClearCompletedDate(object obj)
        {
            return canclearcompleteddate;
        }

        public ICommand ClearCompletedDate
        {
            get
            {
                if (clearcompleteddate == null)
                    clearcompleteddate = new DelegateCommand(CanClearCompletedDate, ExClearCompletedDate);
                return clearcompleteddate;
            }
        }

        private void ExClearCompletedDate(object parameter)
        {           
            Milestone.CompletedDate = null;
            canclearcompleteddate = false;           
        }

        
        bool cancopymilestone = true;
        public bool CanExecuteCopyMilestone(object parameter)
        {
            return cancopymilestone;
        }

        private void ExecuteCopyMilestone(object parameter)
        {
            SetClipboard(Milestone);
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        private void ExecuteClosing(object parameter)
        {
        }

        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {
                if (HasOwner())
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveMilestone();
                        ReturnObject = true;
                        return true;
                    }
                    else                    
                        return true;                    
                }
                else
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes with errors. Do you want to correct and then save these?", "Unsaved Changes with Errors", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                        return false;
                    else
                        return true;
                }
            }
            else
                return true;
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExecuteClosing);
                return windowclosing;
            }
        }

        #endregion
    }
}
