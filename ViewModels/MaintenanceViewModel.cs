using System.Windows.Input;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows;
using System.Text;
using System.Linq;
using static PTR.StaticCollections;

namespace PTR.ViewModels
{
    public class MaintenanceViewModel : FilterModule
    {        
        const string overdueactivitieslabel = "Overdue Monthly Activities";
        const string incompleteepslabel = "Evaluation Plans";
        const string requiringcompletionlabel = "Requiring Completion";
        const string milestonesduelabel = "Milestones Overdue";

        string incompleteepsemstr;
        string incompleteepsidstr;
        string missingepsemstr;
        string missingepsidstr;
        string requiringcompletionemstr;
        string requiringcompletionidstr;
        string overdueemstr;
        string overdueemidstr;
        string milestonedueemstr;
        string milestonedueidstr;

        public ICommand OpenComments { get; set; }
        public ICommand CopyOverdueList { get; set; }
        public ICommand CopyRequiringCompletion { get; set; }
        public ICommand CopyIncompleteEPList { get; set; }
        public ICommand CopyMissingEPList { get; set; }
        public ICommand CopyMilestoneDueList { get; set; }
        public ICommand OpenEvaluationPlan { get; set; }
        public ICommand OpenMilestoneDue { get; set; }
        public ICommand OpenMissingEPProject { get; set; }
        public ICommand OpenIncompleteEP { get; set; }
        public ICommand OpenRequiringCompletion { get; set; }
        
        public ICommand EmailOverdueReminder { get; set; }
        public ICommand EmailReqCompletionReminder { get; set; }
        public ICommand EmailIncompleteEPListReminder { get; set; }
        public ICommand EmailMissingEPListReminder { get; set; }
        public ICommand EmailMilestoneDueListReminder { get; set; }

        public MaintenanceViewModel()
        {
           
            OverdueActivities = PTMainViewModel.LoadFilteredColl(GetOverdueMonthlyUpdates);           
            RequiringCompletion = PTMainViewModel.LoadFilteredColl(GetProjectsRequiringCompletion);
            IncompleteEPs = PTMainViewModel.LoadFilteredColl(GetIncompleteEPs);
            MissingEPs = PTMainViewModel.LoadFilteredColl(GetMissingEPs);
            MilestonesDue = PTMainViewModel.LoadFilteredColl(GetOverdueMilestones);

            EvaluationPlansLabel = MakeLabel(incompleteepslabel, IncompleteEPs.Count);
            RequiringCompletionLabel = MakeLabel(requiringcompletionlabel, RequiringCompletion.Count);
            OverdueActivitiesLabel = MakeLabel(overdueactivitieslabel, OverdueActivities.Count);
            MilestonesDueLabel = MakeLabel(milestonesduelabel, MilestonesDue.Count);  
            
            OverdueActivities.ItemPropertyChanged += OverdueActivities_ItemPropertyChanged;
            RequiringCompletion.ItemPropertyChanged += RequiringCompletion_ItemPropertyChanged;
            IncompleteEPs.ItemPropertyChanged += IncompleteEPs_ItemPropertyChanged;
            MissingEPs.ItemPropertyChanged += MissingEPs_ItemPropertyChanged;
            MilestonesDue.ItemPropertyChanged += MilestonesDue_ItemPropertyChanged;

            cancopyoverduelist = false;
            cancopyincompleteeplist = false;
            cancopyreqcompletionlist = false;
            cancopymissingeplist = false;
            cancopymilestoneduelist = false;

            canemailoverduereminder = false;
            canemailincompleteeplist = false;
            canemailmissingeplist = false;
            canemailreqcompletion = false;
            canemailmilestoneduelist = false;
                        
            CopyOverdueList = new RelayCommand(ExecuteCopyOverdueList, CanExecuteCopyOverdue);
            CopyRequiringCompletion = new RelayCommand(ExecuteCopyRequiringCompletion, CanExecutecopyReqCompletion);
            CopyIncompleteEPList = new RelayCommand(ExecuteCopyIncompleteEPList, CanExecuteCopyIncompleteEPList);
            CopyMissingEPList = new RelayCommand(ExecuteCopyMissingEPList, CanExecuteCopyMissingEPList);
            CopyMilestoneDueList = new RelayCommand(ExecuteCopyMilestoneDueList, CanExecuteCopyMilestoneDueList);

            OpenComments = new RelayCommand(ExecuteOpenComments, CanExecuteOpenComments);
            OpenIncompleteEP = new RelayCommand(ExecuteOpenIncompleteEP, CanExecuteOpenIncompleteEP);
            OpenMilestoneDue = new RelayCommand(ExecuteOpenMilestoneDue, CanExecuteOpenMilestoneDue);
            OpenMissingEPProject = new RelayCommand(ExecuteOpenMissingEPProject, CanExecuteOpenMissingEPProject);
            OpenRequiringCompletion = new RelayCommand(ExecuteOpenReqCompletion, CanExecuteOpenReqCompletion);

            EmailOverdueReminder = new RelayCommand(ExecuteEmailOverdue, CanExecuteEmailOverdue);
            EmailReqCompletionReminder = new RelayCommand(ExecuteEmailRequiringCompletion, CanExecuteEmailReqCompletion);
            EmailIncompleteEPListReminder = new RelayCommand(ExecuteEmailIncompleteEPList, CanExecuteEmailIncompleteEPList);
            EmailMissingEPListReminder = new RelayCommand(ExecuteEmailMissingEPList, CanExecuteEmailMissingEPList);
            EmailMilestoneDueListReminder = new RelayCommand(ExecuteEmailMilestoneDueList, CanExecuteEmailMilestoneDueList);
            UseEmail = Config.UseEmail;

        }             
                
        #region Event handlers

        private void IncompleteEPs_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                incompleteepsemstr = GetSelectedEMStr(IncompleteEPs);
                incompleteepsidstr = GetSelectedProjectStr(IncompleteEPs);
                canemailincompleteeplist = (IncompleteEPs.Count(t => t.Selected == true && t.Enabled == true) > 0);
                cancopyincompleteeplist = canemailincompleteeplist;
            }
        }

        private void MissingEPs_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                missingepsemstr = GetSelectedEMStr(MissingEPs);
                missingepsidstr = GetSelectedProjectStr(MissingEPs);
                canemailmissingeplist = (MissingEPs.Count(t => t.Selected == true && t.Enabled == true) > 0);
                cancopymissingeplist = canemailmissingeplist;
            }
        }

        private void RequiringCompletion_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                requiringcompletionemstr = GetSelectedEMStr(RequiringCompletion);
                requiringcompletionidstr = GetSelectedProjectStr(RequiringCompletion);
                canemailreqcompletion = (RequiringCompletion.Count(t => t.Selected == true && t.Enabled == true) > 0);
                cancopyreqcompletionlist = canemailreqcompletion;
            }
        }

        private void OverdueActivities_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                overdueemstr = GetSelectedEMStr(OverdueActivities);
                overdueemidstr = GetSelectedProjectStr(OverdueActivities);
                canemailoverduereminder = (OverdueActivities.Count(t => t.Selected == true && t.Enabled == true) > 0);
                cancopyoverduelist = canemailoverduereminder;
            }
        }

        private void MilestonesDue_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
           if (e.PropertyName == "Selected")
            {
                milestonedueemstr = GetSelectedEMStr(MilestonesDue);
                milestonedueidstr = GetSelectedProjectStr(MilestonesDue);
                canemailmilestoneduelist = (MilestonesDue.Count(t => t.Selected == true && t.Enabled == true) > 0);
                cancopymilestoneduelist = canemailmilestoneduelist;
            }
        }
              
        private string GetSelectedEMStr(FullyObservableCollection<MaintenanceModel> mm)
        {
            string domain = Config.Domain;
            string temp = string.Join(",", mm.Where(t => t.Selected == true).Select(x => x.Email + "@" + domain).Distinct().ToList());
            if (!string.IsNullOrEmpty(temp))
                return temp;
            else
                return string.Empty;
        }

        private string GetSelectedProjectStr(FullyObservableCollection<MaintenanceModel> mm)
        {
            string linebreak = "\n";
            if (Config.IsBodyHtml)
                linebreak = "<br/>";

            string temp = string.Join(linebreak, mm.Where(t => t.Selected == true).Select(x => x.ProjectID.ToString() + " : " + x.ProjectName + " : " + x.UserName).Distinct().ToList());
            if (!string.IsNullOrEmpty(temp))
                return temp;
            else
                return string.Empty;
        }

        #endregion

        #region Properties

        bool useemail;
        public bool UseEmail
        {
            get { return useemail; }
            set { SetField(ref useemail, value); }
        }

        string missingepslbl;
        public string EvaluationPlansLabel
        {
            get { return missingepslbl; }
            set { SetField(ref missingepslbl, value); }
        }

        string requiringcompletionlbl;
        public string RequiringCompletionLabel
        {
            get { return requiringcompletionlbl; }
            set { SetField(ref requiringcompletionlbl, value); }
        }

        string overdueactivitieslbl;
        public string OverdueActivitiesLabel
        {
            get { return overdueactivitieslbl; }
            set { SetField(ref overdueactivitieslbl, value); }
        }

        string milestoneslbl;
        public string MilestonesDueLabel
        {
            get { return milestoneslbl; }
            set { SetField(ref milestoneslbl, value); }
        }

        FullyObservableCollection<MaintenanceModel> incompleteeps;
        public FullyObservableCollection<MaintenanceModel> IncompleteEPs
        {
            get { return incompleteeps; }
            set { SetField(ref incompleteeps, value); }
        }

        FullyObservableCollection<MaintenanceModel> missingeps;
        public FullyObservableCollection<MaintenanceModel> MissingEPs
        {
            get { return missingeps; }
            set { SetField(ref missingeps, value); }
        }

        FullyObservableCollection<MaintenanceModel> requiringcompletion;
        public FullyObservableCollection<MaintenanceModel> RequiringCompletion
        {
            get { return requiringcompletion; }
            set { SetField(ref requiringcompletion, value); }
        }       
       
        FullyObservableCollection<MaintenanceModel> overdueactivities;
        public FullyObservableCollection<MaintenanceModel> OverdueActivities
        {
            get { return overdueactivities; }
            set { SetField(ref overdueactivities, value); }
        }

        FullyObservableCollection<MaintenanceModel> milestonesdue;
        public FullyObservableCollection<MaintenanceModel> MilestonesDue
        {
            get { return milestonesdue; }
            set { SetField(ref milestonesdue, value); }
        }

        MaintenanceModel selectedincompleteepproject;
        public MaintenanceModel SelectedIncompleteEPProject
        {
            get { return selectedincompleteepproject; }
            set { SetField(ref selectedincompleteepproject, value); }
        }

        MaintenanceModel selectedmissingepproject;
        public MaintenanceModel SelectedMissingEPProject
        {
            get { return selectedmissingepproject; }
            set { SetField(ref selectedmissingepproject, value); }
        }

        MaintenanceModel selectedrequiringcompletion;
        public MaintenanceModel SelectedRequiringCompletion
        {
            get { return selectedrequiringcompletion; }
            set { SetField(ref selectedrequiringcompletion, value); }
        }
                
        MaintenanceModel selectedoverdueactivity;
        public MaintenanceModel SelectedOverdueActivity
        {
            get { return selectedoverdueactivity; }
            set { SetField(ref selectedoverdueactivity, value); }
        }

        MaintenanceModel selectedmilestonedue;
        public MaintenanceModel SelectedMilestoneDue
        {
            get { return selectedmilestonedue; }
            set { SetField(ref selectedmilestonedue, value); }
        }

        private string MakeLabel(string source, int counter)
        {
            return source + " (" + counter.ToString() + ")";
        }

        #endregion

        #region Private functions

        private void SetClipboard(FullyObservableCollection<MaintenanceModel> ps)
        {
            StringBuilder sb = new StringBuilder();
            foreach (MaintenanceModel p in ps)
            {
                if (p.Selected)
                {
                    sb.Append(p.ProjectID.ToString() + " " + p.ProjectName + " " + p.UserName);
                    sb.Append("\n");
                }
            }
            if (sb.Length > 0)
                sb.Remove(sb.Length-1,1);

            Clipboard.SetData(DataFormats.Text, sb.ToString());
        }

        private void EmailReminder(string recipients, string subject, string message, string body)
        {                                 
            if (!Mail.SendMail(CurrentUser.Email + "@" + Config.Domain, recipients, subject, message, body))
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("The email failed to send", "Failed to send", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        #endregion

        #region Commands
       
        #region Overdue Activities

        bool cancopyoverduelist = true;
        public bool CanExecuteCopyOverdue(object parameter)
        {
            return cancopyoverduelist;   
        }
       
        private void ExecuteCopyOverdueList(object parameter)
        {
            SetClipboard(OverdueActivities);
        }
        
        bool canemailoverduereminder = true;
        public bool CanExecuteEmailOverdue(object parameter)
        {
            return canemailoverduereminder;
        }          
                
        private void ExecuteEmailOverdue(object parameter)
        {
            EmailReminder(overdueemstr, Config.OverdueEMTitle, Config.OverdueEMMessage, overdueemidstr);
        }

        bool canopenprojectcomments = true;
        public bool CanExecuteOpenComments(object parameter)
        {
            return canopenprojectcomments;
        }

        private void ExecuteOpenComments(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedOverdueActivity.ProjectID;
                    bool result = msgbox.OpenProjectCommentsDlg((Window)parameter, id);
                    //if return value is true then Refresh list
                    if (result == true)
                    {
                        OverdueActivities = PTMainViewModel.LoadFilteredColl(GetOverdueMonthlyUpdates);
                        OverdueActivitiesLabel = MakeLabel(overdueactivitieslabel, OverdueActivities.Count);
                    }
                    msgbox = null;
                }
            }
            catch { }
        }
        
        #endregion

        #region Requiring Completion

        bool cancopyreqcompletionlist = true;
        public bool CanExecutecopyReqCompletion(object parameter)
        {
            return cancopyreqcompletionlist;
        }

        private void ExecuteCopyRequiringCompletion(object parameter)
        {
            SetClipboard(RequiringCompletion);
        }

        bool canemailreqcompletion = true;
        public bool CanExecuteEmailReqCompletion(object parameter)
        {
            return canemailreqcompletion;
        }

        bool canopenreqcompletion = true;
        public bool CanExecuteOpenReqCompletion(object parameter)
        {
            return canopenreqcompletion;
        }

        private void ExecuteOpenReqCompletion(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedRequiringCompletion.ProjectID;

                    bool result = msgbox.OpenProjectDlg((Window)parameter, id);
                    //if return value is true then Refresh list
                    if (result == true)
                    {
                        RequiringCompletion = PTMainViewModel.LoadFilteredColl(GetProjectsRequiringCompletion);
                        RequiringCompletionLabel = MakeLabel(requiringcompletionlbl, RequiringCompletion.Count);
                    }
                    msgbox = null;
                }
            }
            catch { }
        }



        private void ExecuteEmailRequiringCompletion(object parameter)
        {
            EmailReminder(requiringcompletionemstr, Config.RequiringCompletionEMTitle, Config.RequiringCompletionEMMessage, requiringcompletionidstr);
        }

        #endregion

        #region Incomplete Evaluation Plans

        bool cancopyincompleteeplist = true;
        public bool CanExecuteCopyIncompleteEPList(object parameter)
        {
            return cancopyincompleteeplist;
        }

        private void ExecuteCopyIncompleteEPList(object parameter)
        {
            SetClipboard(IncompleteEPs);          
        }
               
        bool canemailincompleteeplist = true;
        public bool CanExecuteEmailIncompleteEPList(object parameter)
        {
            return canemailincompleteeplist;
        }

        private void ExecuteEmailIncompleteEPList(object parameter)
        {
            EmailReminder(incompleteepsemstr, Config.IncompleteEPsEMTitle, Config.IncompleteEPsEMMessage, incompleteepsidstr);
        }
        
        bool canopenep = true;
        public bool CanExecuteOpenIncompleteEP(object parameter)
        {
            return canopenep;
        }

        private void ExecuteOpenIncompleteEP(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedIncompleteEPProject.ID;
                    int projectid = SelectedIncompleteEPProject.ProjectID;

                    bool result = msgbox.EvaluationPlanDialog((Window)parameter, id, projectid);
                    //if return value is true then Refresh list
                    if (result == true)
                        IncompleteEPs = PTMainViewModel.LoadFilteredColl(GetIncompleteEPs);

                    msgbox = null;
                }
            }
            catch { }
        }

        #endregion

        #region Missing Evaluation Plans

        bool cancopymissingeplist = true;
        public bool CanExecuteCopyMissingEPList(object parameter)
        {
            return cancopymissingeplist;
        }

        private void ExecuteCopyMissingEPList(object parameter)
        {
            SetClipboard(MissingEPs);
        }
        
        bool canemailmissingeplist = true;
        public bool CanExecuteEmailMissingEPList(object parameter)
        {
            return canemailmissingeplist;
        }

        private void ExecuteEmailMissingEPList(object parameter)
        {
            EmailReminder(missingepsemstr, Config.MissingEPsEMTitle, Config.MissingEPsEMMessage, missingepsidstr);
        }
                
        bool canexecuteopenmissingepproject = true;
        public bool CanExecuteOpenMissingEPProject(object parameter)
        {
            return canexecuteopenmissingepproject;
        }

        private void ExecuteOpenMissingEPProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedMissingEPProject.ProjectID;
                    bool result = msgbox.OpenProjectDlg((Window)parameter, id);
                    //if return value is true then Refresh list
                    if (result == true)
                    {
                        RequiringCompletion = PTMainViewModel.LoadFilteredColl(GetProjectsRequiringCompletion);
                        MissingEPs = PTMainViewModel.LoadFilteredColl(GetMissingEPs);
                        RequiringCompletionLabel = MakeLabel(requiringcompletionlabel, RequiringCompletion.Count);

                    }
                    msgbox = null;
                }
            }
            catch { }
        }


        #endregion

        #region Milestones

        bool cancopymilestoneduelist = true;
        public bool CanExecuteCopyMilestoneDueList(object parameter)
        {
            return cancopymilestoneduelist;
        }

        private void ExecuteCopyMilestoneDueList(object parameter)
        {
            SetClipboard(MilestonesDue);
        }


        bool canemailmilestoneduelist = true;
        public bool CanExecuteEmailMilestoneDueList(object parameter)
        {
            return canemailmilestoneduelist;
        }

        private void ExecuteEmailMilestoneDueList(object parameter)
        {
            EmailReminder(milestonedueemstr, Config.MilestoneDueEMTitle, Config.MilestoneDueEMMessage, milestonedueidstr);
        }

        bool canopenmilestonedue = true;
        public bool CanExecuteOpenMilestoneDue(object parameter)
        {
            return canopenmilestonedue;
        }

        private void ExecuteOpenMilestoneDue(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedMilestoneDue.ID;
                    int projectid = SelectedMilestoneDue.ProjectID;
                    bool result = msgbox.MilestoneDialog((Window)parameter, id, projectid);
                    //if return value is true then Refresh list
                    if (result == true)
                    {
                        MilestonesDue = PTMainViewModel.LoadFilteredColl(GetOverdueMilestones);
                        MilestonesDueLabel = MakeLabel(milestonesduelabel, MilestonesDue.Count);
                    }
                    msgbox = null;
                }
            }
            catch { }
        }

        #endregion
        
        #endregion
        
    }
}