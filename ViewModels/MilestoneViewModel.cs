using System;
using System.Windows.Input;
using PTR.Models;
using System.Text;

namespace PTR.ViewModels
{
    public class MilestoneViewModel : ObjectCRUDViewModel
    {
        private const string title = "Milestone";
        public ICommand CopyMilestone { get; set; }
        bool isdirty = false;

        public MilestoneViewModel(int id, int projectid)
        {
            ExCloseWindow = ExecuteClosing;
            if (id == 0)
            {
                Milestone = new MilestoneModel()
                {
                    GOM = new GenericObjModel()
                    {
                        Description = string.Empty
                    },
                    TargetDate = DateTime.Now.AddMonths(1),
                    ProjectID = projectid
                };
                IsEnabled = true;
            }
            else
            {
                Milestone = DatabaseQueries.GetMilestone(id);
                SetUserAccessExistingEP(Milestone.CustomerID);
            }
            Milestone.PropertyChanged += Milestone_PropertyChanged;
            Milestone.GOM.PropertyChanged += Milestone_PropertyChanged;

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);

            FullyObservableCollection<UserModel> associatess = DatabaseQueries.GetUsers();
            foreach (UserModel ag in associatess)
            {
                if (!ag.GOM.Deleted)
                {
                    users.Add(new UserModel()
                    {
                        GOM = new GenericObjModel()
                        {
                            ID = ag.GOM.ID,
                            Name = ag.GOM.Name
                        },
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

            CopyMilestone = new RelayCommand(ExecuteCopyMilestone, CanExecuteCopyMilestone);
        }

        #region Event Handlers

        private void Milestone_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            isdirty = true;
        }

        #endregion

        #region View Properties

        string selectedcompleteddate;
        public string SelectedCompletedDate
        {
            get { return selectedcompleteddate; }
            set
            {
                canclearcompleteddate = !string.IsNullOrEmpty(value);
                SetField(ref selectedcompleteddate, value);
            }
        }

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
        
        bool? blnsave;
        public bool? SaveFlag
        {
            get { return blnsave; }
            set { SetField(ref blnsave, value); }
        }

        #endregion

        #region Private functions

        private void SetUserAccessExistingEP(int customerid)
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
            sbhtml.Append(milestone.GOM.Description);
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
            sbtext.Append(milestone.GOM.Description);
            sbtext.Append("\n\n");
            sbtext.Append("Due Date: ");
            sbtext.Append(milestone.TargetDate.Value.ToString("dd-MMM-yyyy"));

            ClipboardHelper.CopyToClipboard(sbhtml.ToString(), sbtext.ToString());

        }

        #endregion

        #region Commands

        private bool CanExecuteSave(object obj)
        {
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveMilestone();

            ReturnObject = true;
            SaveFlag = true;
            CloseWindow();
        }

        private void SaveMilestone()
        {
            if (isdirty)
            { 
                if (Milestone.GOM.ID > 0)
                    DatabaseQueries.UpdateMilestone(Milestone);
                else
                    DatabaseQueries.AddMilestone(Milestone);
                isdirty = false;
            }
        }

        ICommand cancelandclose;
        public ICommand CancelConfirmation
        {
            get
            {
                if (cancelandclose == null)
                    cancelandclose = new DelegateCommand(CanExecute, ExecuteCancelAndClose);
                return cancelandclose;
            }
        }

        private void ExecuteCancelAndClose(object parameter)
        {
            ReturnObject = false;
            SaveFlag = false;                                            
            CloseWindow();
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
                      
        private void ExecuteClosing(object parameter)
        {
            if (canexecutesave)
            {
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveMilestone();
                }
            }
        }


        #endregion
    }
}
