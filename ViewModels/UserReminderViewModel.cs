using System.Windows.Input;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Windows;
using System.Text;
using System.Linq;
using static PTR.StaticCollections;

namespace PTR.ViewModels
{
    public class UserReminderViewModel : FilterModule
    {
        private const string title = "Actions Required";
        const string overdueactivitieslabel = "Overdue Activities";
        const string incompleteepslabel = "Incomplete EPs";
        const string missingepslabel = "Missing EPs";
        const string requiringcompletionlabel = "Requiring Closure";
        const string milestonesduelabel = "Milestones Due";

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
               
        public ICommand CopyOverdueList { get; set; }
        public ICommand CopyRequiringCompletion { get; set; }
        public ICommand CopyIncompleteEPList { get; set; }
        public ICommand CopyMissingEPList { get; set; }
        public ICommand CopyMilestoneDueList { get; set; }

        public ICommand OpenComments { get; set; }
        public ICommand OpenIncompleteEvaluationPlan { get; set; }
        public ICommand OpenMilestoneDue { get; set; }                     
        public ICommand OpenMissingEPProject { get; set; }
        public ICommand OpenReqCompletionProject { get; set; }
               
        public UserReminderViewModel(PTMainViewModel ptmvm)
        {
            PTMVM = ptmvm;

            WindowTitle = title;
            if (StaticCollections.CurrentUser.LastAccessed != null)
                WindowTitle = WindowTitle + "      Last Accessed: " + StaticCollections.CurrentUser.LastAccessed.Value.ToString("dd-MMM-yyyy h:mm tt");

            PTMVM.OverdueActivities = PTMainViewModel.LoadFilteredColl(GetOverdueMonthlyUpdates);
            PTMVM.RequiringCompletion = PTMainViewModel.LoadFilteredColl(GetProjectsRequiringCompletion);
            PTMVM.IncompleteEPs = PTMainViewModel.LoadFilteredColl(GetIncompleteEPs);
            PTMVM.MissingEPs = PTMainViewModel.LoadFilteredColl(GetMissingEPs);
            PTMVM.MilestonesDue = PTMainViewModel.LoadFilteredColl(GetOverdueMilestones);

            OverdueActivitiesLabel = MakeLabel(overdueactivitieslabel, PTMVM.OverdueActivities.Count);
            IncompleteEvaluationPlansLabel = MakeLabel(incompleteepslabel, PTMVM.IncompleteEPs.Count);
            MissingEvaluationPlansLabel = MakeLabel(missingepslabel, PTMVM.MissingEPs.Count);
            RequiringCompletionLabel = MakeLabel(requiringcompletionlabel, PTMVM.RequiringCompletion.Count);
            MilestonesDueLabel = MakeLabel(milestonesduelabel, PTMVM.MilestonesDue.Count);

            PTMVM.OverdueActivities.ItemPropertyChanged += OverdueActivities_ItemPropertyChanged;
            PTMVM.RequiringCompletion.ItemPropertyChanged += RequiringCompletion_ItemPropertyChanged;
            PTMVM.IncompleteEPs.ItemPropertyChanged += IncompleteEPs_ItemPropertyChanged;
            PTMVM.MissingEPs.ItemPropertyChanged += MissingEPs_ItemPropertyChanged;
            PTMVM.MilestonesDue.ItemPropertyChanged += MilestonesDue_ItemPropertyChanged;

            cancopyoverduelist = false;
            cancopyincompleteeplist = false;
            cancopyreqcompletionlist = false;
            cancopymissingeplist = false;
            cancopymilestoneduelist = false;
                        
            CopyOverdueList = new RelayCommand(ExecuteCopyOverdueList, CanExecuteCopyOverdue);
            CopyRequiringCompletion = new RelayCommand(ExecuteCopyRequiringCompletion, CanExecutecopyReqCompletion);
            CopyIncompleteEPList = new RelayCommand(ExecuteCopyIncompleteEPList, CanExecuteCopyIncompleteEPList);
            CopyMissingEPList = new RelayCommand(ExecuteCopyMissingEPList, CanExecuteCopyMissingEPList);
            CopyMilestoneDueList = new RelayCommand(ExecuteCopyMilestoneDueList, CanExecuteCopyMilestoneDueList);

            OpenComments = new RelayCommand(ExecuteOpenComments, CanExecuteOpenComments);
            OpenIncompleteEvaluationPlan = new RelayCommand(ExecuteOpenIncompleteEP, CanExecuteOpenIncompleteEP);            
            OpenMissingEPProject = new RelayCommand(ExecuteOpenMissingEPProject, CanExecuteOpenMissingEPProject);
            OpenReqCompletionProject = new RelayCommand(ExecuteOpenReqCompletionProject, CanExecuteOpenReqCompletionProject);
            OpenMilestoneDue = new RelayCommand(ExecuteOpenMilestoneDue, CanExecuteOpenMilestoneDue);

            SetTabVisibility();
        }             

        #region Event handlers

        private void IncompleteEPs_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                incompleteepsemstr = GetSelectedEMStr(PTMVM.IncompleteEPs);
                incompleteepsidstr = GetSelectedProjectStr(PTMVM.IncompleteEPs);
                cancopyincompleteeplist  = (PTMVM.IncompleteEPs.Count(t => t.Selected == true && t.Enabled == true) > 0);               
            }
        }

        private void MissingEPs_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                missingepsemstr = GetSelectedEMStr(PTMVM.MissingEPs);
                missingepsidstr = GetSelectedProjectStr(PTMVM.MissingEPs);
                cancopymissingeplist = (PTMVM.MissingEPs.Count(t => t.Selected == true && t.Enabled == true) > 0);                
            }
        }

        private void RequiringCompletion_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                requiringcompletionemstr = GetSelectedEMStr(PTMVM.RequiringCompletion);
                requiringcompletionidstr = GetSelectedProjectStr(PTMVM.RequiringCompletion);
                cancopyreqcompletionlist = (PTMVM.RequiringCompletion.Count(t => t.Selected == true && t.Enabled == true) > 0);                
            }
        }

        private void OverdueActivities_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                overdueemstr = GetSelectedEMStr(PTMVM.OverdueActivities);
                overdueemidstr = GetSelectedProjectStr(PTMVM.OverdueActivities);
                cancopyoverduelist = (PTMVM.OverdueActivities.Count(t => t.Selected == true && t.Enabled == true) > 0);                 
            }
        }

        private void MilestonesDue_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Selected")
            {
                milestonedueemstr = GetSelectedEMStr(PTMVM.MilestonesDue);
                milestonedueidstr = GetSelectedProjectStr(PTMVM.MilestonesDue);
                cancopymilestoneduelist = (PTMVM.MilestonesDue.Count(t => t.Selected == true && t.Enabled == true) > 0);                
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
        
        string windowtitle;
        public string WindowTitle
        {
            get { return windowtitle; }
            set { SetField(ref windowtitle, value); }
        }

        PTMainViewModel ptmvm;
        public PTMainViewModel PTMVM
        {
            get { return ptmvm; }
            set {SetField(ref ptmvm , value); }
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

        bool overdueactivitiesvis;
        public bool OverdueActivitiesVis
        {
            get { return overdueactivitiesvis; }
            set { SetField(ref overdueactivitiesvis, value); }
        }
                
        bool overdueactivitiesselected;
        public bool OverdueActivitiesSelected
        {
            get { return overdueactivitiesselected; }
            set { SetField(ref overdueactivitiesselected, value); }
        }
                     
        bool missingevaluationplansvis;
        public bool MissingEvaluationPlansVis
        {
            get { return missingevaluationplansvis; }
            set { SetField(ref missingevaluationplansvis, value); }
        }
                
        bool missingevaluationplansselected;
        public bool MissingEvaluationPlansSelected
        {
            get { return missingevaluationplansselected; }
            set { SetField(ref missingevaluationplansselected, value); }
        }

        bool incompleteevaluationplansvis;
        public bool InCompleteEvaluationPlansVis
        {
            get { return incompleteevaluationplansvis; }
            set { SetField(ref incompleteevaluationplansvis, value); }
        }
        
        bool incompleteevaluationplansselected;
        public bool InCompleteEvaluationPlansSelected
        {
            get { return incompleteevaluationplansselected; }
            set { SetField(ref incompleteevaluationplansselected, value); }
        }
               
        bool requiringcompletionvis;
        public bool RequiringCompletionVis
        {
            get { return requiringcompletionvis; }
            set { SetField(ref requiringcompletionvis, value); }
        }
        
        bool requiringcompletionselected;
        public bool RequiringCompletionSelected
        {
            get { return requiringcompletionselected; }
            set { SetField(ref requiringcompletionselected, value); }
        }
        
        bool milestonesduevis;
        public bool MilestonesDueVis
        {
            get { return milestonesduevis; }
            set { SetField(ref milestonesduevis, value); }
        }

        bool milestonesdueselected;
        public bool MilestonesDueSelected
        {
            get { return milestonesdueselected; }
            set { SetField(ref milestonesdueselected, value); }
        }
        
        string incompleteepslbl;
        public string IncompleteEvaluationPlansLabel
        {
            get { return incompleteepslbl; }
            set { SetField(ref incompleteepslbl, value); }
        }

        string missingepslbl;
        public string MissingEvaluationPlansLabel
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

        #endregion

        #region Private functions       

        private string MakeLabel(string source, int counter)
        {
            return source + " (" + counter.ToString() + ")";
        }

        private void SetTabVisibility()
        {
            OverdueActivitiesVis = PTMVM.OverdueActivities.Count > 0;         
            MissingEvaluationPlansVis = PTMVM.MissingEPs.Count > 0;
            InCompleteEvaluationPlansVis = PTMVM.IncompleteEPs.Count > 0;
            RequiringCompletionVis = PTMVM.RequiringCompletion.Count > 0;
            MilestonesDueVis = PTMVM.MilestonesDue.Count > 0;

            if (OverdueActivitiesVis)
                OverdueActivitiesSelected = true;
            else
            if (MissingEvaluationPlansVis)
                MissingEvaluationPlansSelected = true;
            else
            if (InCompleteEvaluationPlansVis)
                InCompleteEvaluationPlansSelected = true;
            else
            if (RequiringCompletionVis)
                RequiringCompletionVis = true;
            else
                MilestonesDueSelected = true;

            if (!OverdueActivitiesVis && !MissingEvaluationPlansVis && !InCompleteEvaluationPlansVis && !RequiringCompletionVis && !MilestonesDueVis)
            {
                ReturnObject = true;
                SaveFlag = true;
            };          
        }

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
                sb.Remove(sb.Length - 1, 1);

            Clipboard.SetData(DataFormats.Text, sb.ToString());
        }

        #endregion

        #region Commands

        #region Requiring Completion

        bool canopenreqcompletionproject = true;
        public bool CanExecuteOpenReqCompletionProject(object parameter)
        {
            return canopenreqcompletionproject;
        }

        private void ExecuteOpenReqCompletionProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {                   
                    IMessageBoxService msgbox = new MessageBoxService();
                    int id = SelectedRequiringCompletion.ProjectID;
                    bool result = msgbox.OpenProjectDlg((Window)parameter, id);
                    //if return value is true then Refresh list
                 
                    PTMVM.RequiringCompletion = PTMainViewModel.LoadFilteredColl(GetProjectsRequiringCompletion);
                    PTMVM.IncompleteEPs = PTMainViewModel.LoadFilteredColl(GetIncompleteEPs);
                    PTMVM.MissingEPs = PTMainViewModel.LoadFilteredColl(GetMissingEPs);
                    RequiringCompletionLabel = MakeLabel(requiringcompletionlabel, PTMVM.RequiringCompletion.Count);
                    IncompleteEvaluationPlansLabel = MakeLabel(incompleteepslabel, PTMVM.IncompleteEPs.Count);
                    MissingEvaluationPlansLabel = MakeLabel(missingepslabel, PTMVM.MissingEPs.Count);                        
                    SetTabVisibility();
                    
                    msgbox = null;                    
                }
            }
            catch { }
        }

        bool cancopyreqcompletionlist = true;
        public bool CanExecutecopyReqCompletion(object parameter)
        {
            return cancopyreqcompletionlist;
        }

        private void ExecuteCopyRequiringCompletion(object parameter)
        {
            SetClipboard(PTMVM.RequiringCompletion);
        }
        #endregion

        #region Overdue Activities

        bool cancopyoverduelist = true;
        public bool CanExecuteCopyOverdue(object parameter)
        {
            return cancopyoverduelist;
        }

        private void ExecuteCopyOverdueList(object parameter)
        {
            SetClipboard(PTMVM.OverdueActivities);
        }

        readonly bool canopenprojectcomments = true;
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
                  
                    PTMVM.OverdueActivities = PTMainViewModel.LoadFilteredColl(GetOverdueMonthlyUpdates);
                    OverdueActivitiesLabel = MakeLabel(overdueactivitieslabel, PTMVM.OverdueActivities.Count);
                    SetTabVisibility();
                    
                    msgbox = null;
                }
            }
            catch { }
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
            SetClipboard(PTMVM.IncompleteEPs);
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
                  
                    PTMVM.IncompleteEPs = PTMainViewModel.LoadFilteredColl(GetIncompleteEPs);
                    IncompleteEvaluationPlansLabel = MakeLabel(incompleteepslabel, PTMVM.IncompleteEPs.Count);
                    SetTabVisibility();
                    
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
            SetClipboard(PTMVM.MissingEPs);
        }

        bool canopenmissingepproject = true;
        public bool CanExecuteOpenMissingEPProject(object parameter)
        {
            return canopenmissingepproject;
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
                   
                    PTMVM.RequiringCompletion = PTMainViewModel.LoadFilteredColl(GetProjectsRequiringCompletion);
                    PTMVM.IncompleteEPs = PTMainViewModel.LoadFilteredColl(GetIncompleteEPs);
                    PTMVM.MissingEPs = PTMainViewModel.LoadFilteredColl(GetMissingEPs);
                    RequiringCompletionLabel = MakeLabel(requiringcompletionlabel, PTMVM.RequiringCompletion.Count);
                    IncompleteEvaluationPlansLabel = MakeLabel(incompleteepslabel, PTMVM.IncompleteEPs.Count);                  
                    MissingEvaluationPlansLabel = MakeLabel(missingepslabel, PTMVM.MissingEPs.Count);                        
                    SetTabVisibility();
                    
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
            SetClipboard(PTMVM.MilestonesDue);
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
                   
                    PTMVM.MilestonesDue = PTMainViewModel.LoadFilteredColl(GetOverdueMilestones);
                    MilestonesDueLabel = MakeLabel(milestonesduelabel, PTMVM.MilestonesDue.Count);
                    SetTabVisibility();
                    
                    msgbox = null;
                }
            }
            catch { }
        }
        #endregion

        #endregion
        
    }

}
