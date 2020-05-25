using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using static PTR.DatabaseQueries;
using static PTR.StaticCollections;
using PTR.Models;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class UserViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;
        bool isloaded = false;
        private int ID = 0;
        UserModel User;
        TV.TreeViewViewModel TV; 
        Collection<int> dirtycustomers = new Collection<int>();
        FullyObservableCollection<ModelBaseVM> BusinessUnits;

        public UserViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);           
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
           
            DomainName = Config.Domain;
            BusinessUnits = GetBusinessUnits();
            TV = new TV.TreeViewViewModel();
            GetUsers();
            SelectedUser = Users[0];
            IsEnabled = false;
            canexecutesave = false;

        }
               
        #region Private functions

        private void UpdateAccessTree(int userid)
        {
            CleanUpTVEvents();
            TV.LoadTree(userid);
            Nodes = TV.Nodes;           
            TV.NodeChangeEvent += TV_NodeChangeEvent;           
        }       

        private void CleanUpTVEvents()
        {
            TV.DestroyEventHandlers();
        }
                       
        private void GetUsers()
        {
            UserModel blank = new UserModel()
            {
                ID = 0,
                Name = "None Selected",
                LoginName = string.Empty,
                Email = string.Empty,
                ProjectsMnu = string.Empty,
                AdministrationMnu = string.Empty,
                ReportsMnu = string.Empty,
                BusinessUnits = string.Empty,
                GIN = string.Empty,
                Administrator = false,
                ShowOthers = false,
                AllowEditCompletedCancelled = false
            };            
            Users = DatabaseQueries.GetUsers();
            Users.Insert(0, blank);
         }
            
      
        private void LoadBusinessUnits(UserModel user)
        {
            FullyObservableCollection <SelectedItemModel> BusinessUnitsColl = new FullyObservableCollection<SelectedItemModel>();
            List<string> associatessaledivs = user.BusinessUnits.Split(',').ToList();
            foreach (ModelBaseVM sd in BusinessUnits)
            {
                BusinessUnitsColl.Add(new SelectedItemModel
                {
                    ID = sd.ID,
                    IsChecked = associatessaledivs.Contains(sd.ID.ToString()),
                    Name = sd.Name,
                    IsEnabled = !user.Deleted
                });
            }
            BusinessUnitOptions = BusinessUnitsColl;
            BusinessUnitOptions.ItemPropertyChanged += BusinessUnitOptions_ItemPropertyChanged;
        }
              
        private void LoadSelectedUser(UserModel user)
        {
            isloaded = false;
            IsEnabled = user.ID > 0;

            ID = user.ID;
            ShowDataMessage = false;
            DataMessageLabel = string.Empty;

            LoadAdministrationMnuOptions(user);
            LoadProjectMnuOptions(user);
            LoadReportMnuOptions(user);
            LoadBusinessUnits(user);
            UpdateAccessTree(ID);

            Administrator = user.Administrator;
            ShowOthers = user.ShowOthers;
            AllowEditCompletedCancelled = user.AllowEditCompletedCancelled;

            Name = user.Name;
            Deleted = user.Deleted;
            LoginName = user.LoginName;
            Email = user.Email;
            GIN = user.GIN;

            User = user;
            isloaded = true;

            canexecutesave = false;
            cancancelnewuser = false;
            
        }

        private void SetAdministrator()
        {
            ShowOthers = Administrator;
            foreach (SelectedItemModel sd in BusinessUnitOptions)
                sd.IsChecked = Administrator;
            foreach (SelectedItemModel sd in AdministrationMnuOptions)
                sd.IsChecked = Administrator;
            foreach (SelectedItemModel sd in ProjectMnuOptions)
                sd.IsChecked = Administrator;
            foreach (SelectedItemModel sd in ReportMnuOptions)
                sd.IsChecked = Administrator;

        }

        private void LoadAdministrationMnuOptions(UserModel user)
        {
            FullyObservableCollection<SelectedItemModel> AdministrationMnuColl = new FullyObservableCollection<SelectedItemModel>();
            List<string> adminoptions = user.AdministrationMnu.Split(',').ToList();
            foreach (EnumValue ev in EnumerationLists.AdministrationMenuOptionsList)
            {
                AdministrationMnuColl.Add(new SelectedItemModel
                {
                    ID = ev.ID,
                    IsChecked = adminoptions.Contains(ev.ID.ToString()),
                    Name = ev.Description,
                    IsEnabled = !user.Deleted
                });               
            }
            AdministrationMnuOptions = AdministrationMnuColl;
            AdministrationMnuOptions.ItemPropertyChanged += AdministrationMnuOptions_ItemPropertyChanged;
        }

        private void LoadProjectMnuOptions(UserModel user)
        {
            FullyObservableCollection<SelectedItemModel> ProjectsMnuColl = new FullyObservableCollection<SelectedItemModel>();
            List<string> projectoptions = user.ProjectsMnu.Split(',').ToList();
            foreach (EnumValue ev in EnumerationLists.ProjectsOptionsList)
            {
                ProjectsMnuColl.Add(new SelectedItemModel
                {
                    ID = ev.ID,
                    IsChecked = projectoptions.Contains(ev.ID.ToString()),
                    Name = ev.Description,
                    IsEnabled = !user.Deleted
                });               
            }
            ProjectMnuOptions = ProjectsMnuColl;
            ProjectMnuOptions.ItemPropertyChanged += ProjectMnuOptions_ItemPropertyChanged;
        }

        private void LoadReportMnuOptions(UserModel user)
        {
            FullyObservableCollection<SelectedItemModel> ReportsMnuColl = new FullyObservableCollection<SelectedItemModel>();
            List<string> reportoptions = user.ReportsMnu.Split(',').ToList();
            foreach (EnumValue ev in EnumerationLists.ReportsOptionsList)
            {
                ReportsMnuColl.Add(new SelectedItemModel
                {
                    ID = ev.ID,
                    IsChecked = reportoptions.Contains(ev.ID.ToString()),
                    Name = ev.Description,
                    IsEnabled = !user.Deleted
                });               
            }
            ReportMnuOptions = ReportsMnuColl;
            ReportMnuOptions.ItemPropertyChanged += ReportMnuOptions_ItemPropertyChanged;
        }

        private void ClearCurrentUser()
        {
            isdirty = false;
            canexecutesave = false;
            canexecuteadd = true;
            cancancelnewuser = false;
            UserListEnabled = true;
        }

        private void ClearDirtyCustomers()
        {
            dirtycustomers.Clear();
        }

        private void AddDirtyCustomer(int customerid)
        {
            if (!dirtycustomers.Contains(customerid))
            {
                dirtycustomers.Add(customerid);
            }
        }

        #endregion

        #region Event Handlers

        private void TV_NodeChangeEvent(object sender, TV.TreeViewViewModel.NodeChangeEventArgs e)
        {
            isdirty = true;
            CheckAllValidation();           
            AddDirtyCustomer(e.ID);            
        }
                
        private void AdministrationMnuOptions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (isloaded)
            {
                CheckAllValidation();
                isdirty = true;
            }
        }

        private void ProjectMnuOptions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (isloaded)
            {
                CheckAllValidation();
                isdirty = true;
            }
        }

        private void ReportMnuOptions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (isloaded)
            {
                CheckAllValidation();
                isdirty = true;
            }
        }

        private void BusinessUnitOptions_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (isloaded)
            {
                CheckAllValidation();
                isdirty = true;
            }
        }

        #endregion

        #region Properties

        FullyObservableCollection<TreeViewNodeModel> nodes;
        public FullyObservableCollection<TreeViewNodeModel> Nodes
        {
            get { return nodes; }
            set { SetField(ref nodes, value); }                
        }

        FullyObservableCollection<UserModel> users;
        public FullyObservableCollection<UserModel> Users
        {
            get { return users; }
            set { SetField(ref users, value); }
        }
                    
        FullyObservableCollection<SelectedItemModel> adminoptions;
        public FullyObservableCollection<SelectedItemModel> AdministrationMnuOptions
        {
            get { return adminoptions; }
            set { SetField(ref adminoptions, value); }
        }

        FullyObservableCollection<SelectedItemModel> projectoptions;
        public FullyObservableCollection<SelectedItemModel> ProjectMnuOptions
        {
            get { return projectoptions; }
            set { SetField(ref projectoptions, value); }
        }

        FullyObservableCollection<SelectedItemModel> reportoptions;
        public FullyObservableCollection<SelectedItemModel> ReportMnuOptions
        {
            get { return reportoptions; }
            set { SetField(ref reportoptions, value); }
        }
    
        FullyObservableCollection<SelectedItemModel> businessunitoptions;
        public FullyObservableCollection<SelectedItemModel> BusinessUnitOptions
        {
            get { return businessunitoptions; }
            set { SetField(ref businessunitoptions, value); }
        }

        UserModel selecteduser;
        public UserModel SelectedUser
        {
            get { return selecteduser; }
            set
            {
                SetField(ref selecteduser, value);
                LoadSelectedUser(value);               
            }
        }

        bool isenabled=true;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        bool administrator;
        public bool Administrator
        {
            get { return administrator; }
            set
            {
                SetField(ref administrator, value);
                if (isloaded)
                {
                    SetAdministrator();
                    isdirty = true;
                }              
            }
        }

        bool userlistenabled = true;
        public bool UserListEnabled
        {
            get { return userlistenabled; }
            set { SetField(ref userlistenabled, value); }
        }        

        bool showothers;
        public bool ShowOthers
        {
            get { return showothers; }
            set { SetField(ref showothers, value);
                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }

        bool alloweditcompletedcancelled;
        public bool AllowEditCompletedCancelled
        {
            get { return alloweditcompletedcancelled; }
            set
            {
                SetField(ref alloweditcompletedcancelled, value);
                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }
        
        bool deleted;
        public bool Deleted
        {
            get { return deleted; }
            set { SetField(ref deleted, value);
                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }

        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value);
                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }

        string loginname;
        public string LoginName
        {
            get { return loginname; }
            set { SetField(ref loginname, value);

                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }

        string email;
        public string Email
        {
            get { return email; }
            set { SetField(ref email, value);
                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }

        string domainname;
        public string DomainName
        {
            get { return domainname; }
            set { SetField(ref domainname, value); }
        }
        
        string gin;
        public string GIN
        {
            get { return gin; }
            set { SetField(ref gin, value);
                if (isloaded)
                {
                    CheckAllValidation();
                    isdirty = true;
                }
            }
        }

        #endregion

        #region Validation
        
        private bool CheckAllValidation()
        {
            if (!Validate("Name") || !Validate("LoginName") || !Validate("Email") || !Validate("GIN") || !Validate("BU"))
            {
                ShowDataMessage = true;
                canexecutesave = false;
                return false;
            }
            else
            {
                ShowDataMessage = false;
                canexecutesave = true;
                return true;
            }
        }
        
        private bool Validate(string test)
        {            
            DataMessageLabel = string.Empty;
            switch (test)
            {
                case "Name":
                    if (IsMissingName())
                    {                        
                        DataMessageLabel = "Missing Name";
                        return false;
                    }
                    else
                    if (IsDuplicateName())
                    {                        
                        DataMessageLabel = "Duplicate Name";
                        return false;
                    }
                    break;

                case "LoginName":
                    if (IsMissingLoginName())
                    {                        
                        DataMessageLabel = "Missing Login Name";
                        return false;
                    }
                    else
                    if (IsDuplicateLoginName())
                    {                       
                        DataMessageLabel = "Duplicate Login Name";
                        return false;
                    }                       
                    break;

                case "GIN":
                    if (IsDuplicateGIN())
                    {                        
                        DataMessageLabel = "Duplicate GIN";
                        return false;
                    }
                    break;

                case "Email":
                    if (!IsValidEmail())
                    {                        
                        DataMessageLabel = "Invalid email";
                        return false;
                    }
                    else
                    if (IsDuplicateEmail())
                    {                       
                        DataMessageLabel = "Duplicate email";
                        return false;
                    }
                    break;

                case "BU":
                    int countq = BusinessUnitOptions.Count(x => x.IsChecked == true);
                    if(countq == 0)
                    {                        
                        DataMessageLabel = "No Business Unit Selected";
                        return false;
                    }
                    break;                            

            };                                        
            return true;
        }

        private bool IsValidEmail()
        {
            string regstr = Config.Emailformat;

            if (!string.IsNullOrEmpty(regstr.Trim()))
            {
                Regex rx;
                rx = new Regex(regstr, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                bool isvalid = true;

                if (!Deleted)
                {
                    Match match = rx.Match(Email);
                    if (!match.Success)
                    {
                        isvalid = false;
                    }
                }
                return isvalid;
            }
            else
                return true;
        }

        private bool IsDuplicateName()
        {
            int duplicate = Users.Where(x => x.Name.ToUpper() == Name.ToUpper() && (ID != x.ID || ID==0 )).Count();                                     
            return (duplicate > 0);
        }

        private bool IsDuplicateLoginName()
        {
            int duplicate = Users.Where(x => x.LoginName.ToUpper() == LoginName.ToUpper() && (ID != x.ID || ID == 0)).Count();
            return (duplicate > 0);
        }

        private bool IsMissingName()
        {
            return (string.IsNullOrEmpty(Name));                                  
        }

        private bool IsMissingLoginName()
        {
            return (string.IsNullOrEmpty(LoginName));                                      
        }

        private bool IsDuplicateGIN()
        {
            if (string.IsNullOrEmpty(GIN))
                return false;
            int duplicate = Users.Where(x => x.GIN.ToUpper() == GIN.ToUpper() && (ID != x.ID || ID == 0)).Count();
            return (duplicate > 0);
        }

        private bool IsDuplicateEmail()
        {
            int duplicate = Users.Where(x => x.Email.ToUpper() == Email.ToUpper() && (ID != x.ID || ID == 0)).Count();
            return (duplicate > 0);
        }
        
        bool showdatamessage = false;
        public bool ShowDataMessage
        {
            get { return showdatamessage; }
            set { SetField(ref showdatamessage, value); }
        }

        string dataerror;
        public string DataMessageLabel
        {
            get { return dataerror; }
            set { SetField(ref dataerror, value); }
        }

        #endregion

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            return canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            //check for changes and save then add new user
            if (isdirty)            
                SaveUser();
            
            canexecuteadd = false;
            cancancelnewuser = true;
            IsEnabled = true;
            UserListEnabled = false;
            UpdateAccessTree(0);
            ShowDataMessage = false;
            DataMessageLabel = string.Empty;

            if (AdministrationMnuOptions!=null)
                AdministrationMnuOptions.ItemPropertyChanged -= AdministrationMnuOptions_ItemPropertyChanged;
            if(ProjectMnuOptions!=null)
                ProjectMnuOptions.ItemPropertyChanged -= ProjectMnuOptions_ItemPropertyChanged;
            if (ReportMnuOptions != null)
                ReportMnuOptions.ItemPropertyChanged -= ReportMnuOptions_ItemPropertyChanged;
            if (BusinessUnitOptions!=null)
                BusinessUnitOptions.ItemPropertyChanged -= BusinessUnitOptions_ItemPropertyChanged;

            FullyObservableCollection<SelectedItemModel> bucoll = new FullyObservableCollection<SelectedItemModel>();
            foreach (ModelBaseVM sd in BusinessUnits)
            {
                bucoll.Add(new SelectedItemModel()
                {
                    ID = sd.ID,
                    IsChecked = false,
                    IsEnabled = true,
                    Name = sd.Name                   
                });
            }

            FullyObservableCollection<SelectedItemModel> administrationmnucoll = new FullyObservableCollection<SelectedItemModel>();            
            foreach (EnumValue ev in EnumerationLists.AdministrationMenuOptionsList)
            {               
                administrationmnucoll.Add(new SelectedItemModel
                {
                    ID = ev.ID,
                    IsChecked = false,
                    Name = ev.Description,
                    IsEnabled = true
                });                
            }

            FullyObservableCollection<SelectedItemModel> projectsmnucoll = new FullyObservableCollection<SelectedItemModel>();           
            foreach (EnumValue ev in EnumerationLists.ProjectsOptionsList)
            {              
                projectsmnucoll.Add(new SelectedItemModel
                {
                    ID = ev.ID,
                    IsChecked = false,
                    Name = ev.Description,
                    IsEnabled = true
                });                
            }

            FullyObservableCollection<SelectedItemModel> reportsmnucoll = new FullyObservableCollection<SelectedItemModel>();
            foreach (EnumValue ev in EnumerationLists.ReportsOptionsList)
            {
                reportsmnucoll.Add(new SelectedItemModel
                {
                    ID = ev.ID,
                    IsChecked = false,
                    Name = ev.Description,
                    IsEnabled = true
                });
            }

            LoginName = string.Empty;
            GIN = string.Empty;
            BusinessUnitOptions = bucoll;
            Administrator = false;
            AdministrationMnuOptions = administrationmnucoll;
            ProjectMnuOptions = projectsmnucoll;
            ReportMnuOptions = reportsmnucoll;
            
            ShowOthers = false;
            Email = string.Empty;
            Deleted = false;
            ID = 0;
            Name = string.Empty;
            AdministrationMnuOptions.ItemPropertyChanged += AdministrationMnuOptions_ItemPropertyChanged;
            ProjectMnuOptions.ItemPropertyChanged += ProjectMnuOptions_ItemPropertyChanged;
            ReportMnuOptions.ItemPropertyChanged += ReportMnuOptions_ItemPropertyChanged;
            BusinessUnitOptions.ItemPropertyChanged += BusinessUnitOptions_ItemPropertyChanged;
            isloaded = true;
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        private bool CanExecuteSave(object obj)
        {
            return canexecutesave;
        }
                       
        private void ExecuteSave(object parameter)
        {
            try
            {
                SaveUser();
            }
            catch
            {

            }
        }

        private bool SaveUser()
        {
            ShowDataMessage = false;
            if (isdirty)
            {    
                if(User == null)
                {
                    User = new UserModel();
                }             
                
                User.GIN = GIN;
                User.Email = Email;
                User.LoginName = LoginName;
                User.ID = ID;
                User.Name = Name;
                User.Deleted = Deleted;                
                User.ShowOthers = ShowOthers;
                User.Administrator = Administrator;
                User.AllowEditCompletedCancelled = AllowEditCompletedCancelled;

                //convert collections to strings
                var q = from a in BusinessUnitOptions
                        where a.IsChecked == true
                        select a.ID;
                User.BusinessUnits = string.Join(",", q.ToList());

                var q2 = from a in AdministrationMnuOptions
                            where a.IsChecked == true
                            select a.ID;
                User.AdministrationMnu = string.Join(",", q2.ToList());

                var q3 = from a in ProjectMnuOptions
                            where a.IsChecked == true
                            select a.ID;
                User.ProjectsMnu = string.Join(",", q3.ToList());

                var q4 = from a in ReportMnuOptions
                         where a.IsChecked == true
                         select a.ID;
                User.ReportsMnu = string.Join(",", q4.ToList());

                if (ID == 0)
                    ID = AddUser(User);   //get return identity from sql
                else
                {
                    UpdateUser(User);
                    ID = User.ID;
                }
                  
                int accessid = 0;
                if (dirtycustomers.Count > 0)
                {
                    foreach (TreeViewNodeModel n in TV.CustomerNodes)
                    {
                        if (dirtycustomers.Contains(n.ID))
                        {
                            if (n.IsChecked)
                            {
                                if (n.IsROChecked)
                                    accessid = (int)UserPermissionsType.ReadOnly;
                                else                                
                                    if (n.IsFAChecked)
                                        accessid = (int)UserPermissionsType.FullAccess;                                                     
                                    else                                
                                        if (n.IsEditActChecked)
                                            accessid = (int)UserPermissionsType.EditAct;
                                        else
                                            accessid = 0;                                
                            }
                            else                            
                                accessid = 0;
                            

                            if (accessid > 0)
                            {                                
                                if(n.AccessID == 0)                                
                                    AddUserCustomerAccess(n.ID, ID, accessid);                                
                                else                                
                                    UpdateUserCustomerAccess(n.ID, ID, accessid);                                                                                                        
                            }
                            else
                            {
                                if (n.AccessID > 0)                                
                                    UpdateUserCustomerAccess(n.ID, ID, 0);                                
                            }
                        }
                    }
                }
                DeleteUserCustomerAccess();
                ClearDirtyCustomers();
                ClearCurrentUser();
                DataMessageLabel = "Saved"; 
                ShowDataMessage = true;                               
            }            
            return true;
        }
        
        ICommand cancelnewuser;
        public ICommand CancelNewUser
        {
            get
            {
                if (cancelnewuser == null)
                    cancelnewuser = new DelegateCommand(CanCancelNewUser, ExecuteCancelNewUser);
                return cancelnewuser;
            }
        }
        
        bool cancancelnewuser = false;
        private bool CanCancelNewUser(object obj)
        {
            return cancancelnewuser;
        }

        private void ExecuteCancelNewUser(object parameter)
        {
            ClearCurrentUser();
            SelectedUser = Users[0];
        }
                        
        private void ExecuteClosing(object parameter)
        {
            //try
            //{
            //    if (TV != null)
            //    {
            //        CleanUpTVEvents();
            //        if (AdministrationMnuOptions != null)
            //            AdministrationMnuOptions.ItemPropertyChanged -= AdministrationMnuOptions_ItemPropertyChanged;
            //        if (ProjectMnuOptions != null)
            //            ProjectMnuOptions.ItemPropertyChanged -= ProjectMnuOptions_ItemPropertyChanged;
            //        if (ReportMnuOptions != null)
            //            ReportMnuOptions.ItemPropertyChanged -= ReportMnuOptions_ItemPropertyChanged;
            //        if (BusinessUnitOptions != null)
            //            BusinessUnitOptions.ItemPropertyChanged -= BusinessUnitOptions_ItemPropertyChanged;
            //        TV.NodeChangeEvent -= TV_NodeChangeEvent;
            //        TV.DestroyEventHandlers();
            //    }
            //}
            //catch
            //{

            //}
        }

        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {
                if (CheckAllValidation())
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveUser();
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
                    {
                        SaveUser();
                        return true;
                    }
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