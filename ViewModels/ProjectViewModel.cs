using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using static PTR.StaticCollections;
using static PTR.DatabaseQueries;
using System.Collections.Generic;
using PTR.Models;
using System.Windows;

namespace PTR.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {

        private const string title = "Project Details";
        ProjectModel project;
        Regex rx;
        bool isnew = false;
        
        public ProjectViewModel()
        {
            //new project
            isnew = true;
            Initialisedropdowns();
            project = new ProjectModel
            {
                OwnerID = 0,
                CustomerID = 0,
                GOM = new GenericObjModel()
                {
                    ID = 0,
                    Deleted = false,
                    Name = string.Empty
                },
                ProjectStatusID = (int)ProjectStatusType.Active,
                SalesDivisionID = 0,
                MarketSegmentID = 0,
                EstimatedAnnualSales = 0,
                EstimatedAnnualMPC = 0,
                SalesForecastConfirmed = false,
                Resources = string.Empty,
                ActivatedDate = DateTime.Now,
                TargetedVolume = 0,
                GM = 0,
                SMCodeID = -1,
                ApplicationID = 0,
                NewBusinessCategoryID = 0,
                Products = string.Empty,
                ExpectedDateFirstSales = DateTime.Now.AddMonths(12),
                ProbabilityOfSuccess = 100,
                ProjectTypeID = 1,
                KPM = false,
                CDPCCPID = -1,
                DifferentiatedTechnology = false,
                EPRequired = true,
                Milestones = new FullyObservableCollection<MilestoneModel>(),
                EvaluationPlans = new FullyObservableCollection<EPModel>()
                                
            };
            culturecode = "en-US";

            int projectid = AddProject(project);
            project.GOM.ID = projectid;

            ExpectedDateFirstSalesEnabled = true;
            ProjectNameEnabled = true;
            ProjectStatus = project.ProjectStatusID;
            Project.PropertyChanged += _project_PropertyChanged;
            Project.GOM.PropertyChanged += _project_PropertyChanged;
            SetUserAccessForNewProject();

            SetInitial();
        }
     
        public ProjectViewModel(int projectID)
        {
            Initialisedropdowns();
            project = GetSingleProject(projectID);
            project.Milestones= GetProjectMilestones(projectID);
            project.EvaluationPlans = GetProjectEvaluationPlans(projectID);

            ProjectStatus = project.ProjectStatusID;
            Project.PropertyChanged += _project_PropertyChanged;
            Project.GOM.PropertyChanged += _project_PropertyChanged;
            ExpectedDateFirstSalesEnabled = false;
            ProjectNameEnabled = false;

            SetInitial();
        }


        #region User access control    

        private void SetUserAccessExistingProject(int customerid)
        {
            int accessid = StaticCollections.GetUserCustomerAccess(customerid);
            if (accessid == (int)UserPermissionsType.FullAccess)
                IsEnabled = true;            
            else
                IsEnabled = false;        
        }

        private void SetUserAccessForNewProject()
        {
            IsEnabled = true;          
        }

        #endregion

        #region Event handlers

        private void _project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Products")
                InValidProduct = IsInValidProduct((sender as ProjectModel).Products);
            CheckFieldValidation();
            CalcPercentGM();
        }

        #endregion
        
        #region Properties

        string windowtitle;
        public string WindowTitle
        {
            get { return windowtitle; }
            set { SetField(ref windowtitle, value); }
        }

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        bool milestoneisenabled;
        public bool MilestoneIsEnabled
        {
            get { return milestoneisenabled; }
            set { SetField(ref milestoneisenabled, value); }
        }

        public Collection<GenericObjModel> CDPCCP { get; private set; }
        public Collection<string> ProductGroupNames { get; private set; }
        public FullyObservableCollection<GenericObjModel> ProjectTypesList { get; private set; }
        public Collection<EnumValue> ProjectStatusTypesList { get; private set; }
        public FullyObservableCollection<CustomerModel> Customers { get; private set; }
        public FullyObservableCollection<UserModel> Associates { get; private set; }
        public Collection<GenericObjModel> NewBusinessCategories { get; private set; }
        public FullyObservableCollection<GenericObjModel> SalesDivisions { get; private set; }
        FullyObservableCollection<SMCodeModel> smcodes;

        public ProjectModel Project
        {
            get {return project; }
            set {
                ProjectStatus = value.ProjectStatusID;
                SetField(ref project, value); }
        }

        int projstatus;
        public int ProjectStatus
        {
            get { return projstatus; }
            set
            {
                if (project.ProjectStatusID != (int)ProjectStatusType.Completed && (value == (int)ProjectStatusType.Completed))
                {
                    if (value == (int)ProjectStatusType.Completed)
                    {
                        CompletedDateEnabled = true;
                        Project.CompletedDate = DateTime.Now;
                    }
                }
                else
                if (value == (int)ProjectStatusType.Active || value == (int)ProjectStatusType.Cancelled)
                {
                    //clear date completed
                    CompletedDateEnabled = false;
                    Project.CompletedDate = null;
                }
                else
                if (project.ProjectStatusID == (int)ProjectStatusType.Completed)
                    CompletedDateEnabled = true;

                Project.ProjectStatusID = value;
                SetField(ref projstatus, value);
            }
        }
       
        bool completeddateenabled;
        public bool CompletedDateEnabled
        {
            get { return completeddateenabled; }
            set { SetField(ref completeddateenabled, value); }
        }

        bool expecteddatefirstsalesenabled;
        public bool ExpectedDateFirstSalesEnabled
        {
            get { return expecteddatefirstsalesenabled; }
            set { SetField(ref expecteddatefirstsalesenabled, value); }
        }
                        
        bool projectnameenabled;
        public bool ProjectNameEnabled
        {
            get { return projectnameenabled; }
            set { SetField(ref projectnameenabled, value); }
        }
        
        bool? blnsaveproject;
        public bool? SaveProjectFlag
        {
            get { return blnsaveproject; }
            set { SetField(ref blnsaveproject, value); }
        }

        decimal percentgm;
        public decimal PercentGM
        {
            get { return percentgm; }
            set { SetField(ref percentgm, value); }
        }

        GenericObjModel selectedsalesdivision;
        public GenericObjModel SelectedSalesDivision
        {
            get { return selectedsalesdivision; }
            set
            {
                SetField(ref selectedsalesdivision, value);
                if (value != null)
                {
                    FilterMarketSegments(value.ID);
                    FilterSMCodes(value.ID);
                    if (SelectedIndustrySegment!=null)
                         FilterApplications(value.ID, SelectedIndustrySegment.GOM.ID);
                    else
                         FilterApplications(value.ID, 0);
                  
                }               
            }
        }

        MarketSegmentModel selectedmktseg;
        public MarketSegmentModel SelectedIndustrySegment
        {
            get { return selectedmktseg; }
            set
            {
                SetField(ref selectedmktseg, value);
                if (value != null && SelectedSalesDivision!=null)                
                    FilterApplications(SelectedSalesDivision.ID, value.GOM.ID);                                   
            }
        }

        CustomerModel selectedcustomer;
        public CustomerModel SelectedCustomer
        {
            get { return selectedcustomer; }
            set
            {
                SetField(ref selectedcustomer, value);
                if (value != null)
                {
                    CultureCode = value.CultureCode;
                    SetUserAccessExistingProject(value.GOM.ID);
                }
                else
                    CultureCode = "en-US";                                   
            }
        }

        string culturecode;
        public string CultureCode
        {
            get { return culturecode; }
            set { SetField(ref culturecode, value); }
        }

        bool invalidproduct;
        public bool InValidProduct
        {
            get { return invalidproduct; }
            set { SetField(ref invalidproduct, value); }
        }

        int maxprojnamelength = 25;
        public int MaxProjNameLength
        {
            get { return maxprojnamelength; }
            set { SetField(ref maxprojnamelength, value); }
        }
        
        #endregion

        #region Private Functions

        private void SetInitial()
        {
            GetAssociatesList();
            InValidProduct = IsInValidProduct(project.Products);
            CheckFieldValidation();
            CalcPercentGM();

            if (project.GOM.ID == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (ID: " + project.GOM.ID.ToString() + ")";
        }

        private void Initialisedropdowns()
        {
            GetProjectTypesList();
            GetProjectStatusList();
            if (isnew)
                GetNewProjectCustomerList();
            else
                GetCustomerList();
            GetSalesDivisionList();
            GetMarketSegmentsMaster();
            GetApplicationCategoriesList();
            GetNewBusinessCategories();
            GetProductGroupNames();
            GetCDPCCP();
          
            rx = new Regex(StaticCollections.Config.Productformat, RegexOptions.Compiled | RegexOptions.IgnoreCase);

          //  ^([\w+\s]*\w+)*(;\s[\w+\s]*\w+)*$

        }

        private void GetProjectTypesList()
        {           
            ProjectTypesList = StaticCollections.ProjectTypes;
        }

        private void GetProjectStatusList()
        {
            ProjectStatusTypesList = EnumerationLists.ProjectStatusTypesList;
        }

        private void GetNewProjectCustomerList()
        {
            FullyObservableCollection<CustomerModel> cm = new FullyObservableCollection<CustomerModel>();

            int accessid = 0;
            foreach (CustomerModel cust in StaticCollections.Customers)
            {
                accessid = StaticCollections.GetUserCustomerAccess(cust.GOM.ID);
                if (accessid == (int)UserPermissionsType.FullAccess)
                    if (!cust.GOM.Deleted)
                    {
                        cust.IsEnabled = true;// (accessid == (int)UserPermissionsType.FullAccess);
                        cm.Add(cust);
                    }                  
            }
            Customers = cm;
        }        

        private void GetCustomerList()
        {           
            FullyObservableCollection<CustomerModel> cm = new FullyObservableCollection<CustomerModel>();                        
            foreach (CustomerModel cust in StaticCollections.Customers)
            {
                if (!cust.GOM.Deleted)                
                    cust.IsEnabled = (StaticCollections.GetUserCustomerAccess(cust.GOM.ID) == (int)UserPermissionsType.FullAccess);                                   
                else                
                    cust.IsEnabled = false;                    
                
                cm.Add(cust);
            }
            Customers = cm;            
        }

        private string GetCultureCode(int countryid)
        {
            foreach(CountryModel cm in Countries)            
                if (cm.GOM.ID == countryid)
                    return cm.CultureCode;            
            return "en-US";
        }

        private void GetAssociatesList()
        {
            if(!isnew)
                Associates = GetUsers();
            else //remove any deleted associates from list
            {
                FullyObservableCollection<UserModel> associatess = GetUsers();
                FullyObservableCollection<UserModel> newassociatess = new FullyObservableCollection<UserModel>();               
                        
                foreach (UserModel ag in associatess)
                {
                    if (!ag.GOM.Deleted)
                    {                                                                    
                        newassociatess.Add(new UserModel()
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
                Associates = newassociatess;
            }
        }

        private void GetNewBusinessCategories()
        {
            NewBusinessCategories = StaticCollections.NewBusinessCategories;
        }

        private void GetSalesDivisionList()
        {
            SalesDivisions = StaticCollections.SalesDivisions;
        }

        private void GetCDPCCP()
        {
            CDPCCP = StaticCollections.CDPCCP;            
        }

        private void CalcPercentGM()
        {
            if(Project.EstimatedAnnualSales >0 && Project.GM >0)            
                PercentGM = Project.GM / Project.EstimatedAnnualSales;            
            else
                PercentGM = 0;
        }           

        #endregion

        #region Validation

        private bool IsInValidProduct(string product)
        {
            if (!string.IsNullOrEmpty(Config.Productformat.Trim()))
            {
                Match match = rx.Match(product);
                if (match.Success)
                    if (CheckProductNames(product))
                        return false;
                    else
                        return true;
                else
                    return true;
            }
            else
                return false;

        }
        
        private string ConvertoTitleCase(string input)
        {
            return input.Trim().Substring(0, 1).ToUpper() + input.Trim().Substring(1).ToLower();
        }

        private bool CheckProductNames(string srcproduct)
        {
            bool ok = false;
            Collection<string> productsubstrings = new Collection<string>();
            try
            {
                         
                List<string> prodslist = srcproduct.Split(';').ToList();
                List<string> prodname;
                string product = string.Empty;
                foreach (var s in prodslist)
                {
                    product = string.Empty;
                    prodname = s.Trim().Split(' ').ToList();
                                       
                    ok = false;
                    foreach (var pr in StaticCollections.ProductGroupNames)
                    {
                        string temp = string.Empty;
                        if (prodname.Count() == 1 || prodname.Count() == 2)
                        {
                            temp = prodname[0].Trim();
                            prodname[0] = ConvertoTitleCase(prodname[0]);
                            if (prodname[0].ToUpper() == "BLX")
                                prodname[0] = "BLX";
                            if (prodname.Count() == 2)
                                prodname[1] = ConvertoTitleCase(prodname[1]);
                        }
                        else
                        if (prodname.Count() == 3)
                        {
                            temp = prodname[0].Trim() + " " + prodname[1].Trim();
                            prodname[0] = ConvertoTitleCase(prodname[0]);
                            prodname[1] = ConvertoTitleCase(prodname[1]);
                            prodname[2] = prodname[2].ToUpper();
                        }
                                                    
                        if (pr.ToLower() == temp.ToLower())
                        {
                            ok = true;
                            break;
                        }
                    }
                    if (ok)
                    {
                        product = string.Join(" ", prodname);
                        productsubstrings.Add(product);
                    }
                    else
                        break;
                }                              
            }
            catch
            {
                return false;
            }
            if (ok)
            {               
                Project.Products = string.Join("; ", productsubstrings);
                return true;
            }
            else
                return false;
        }

        private void GetProductGroupNames()
        {
            ProductGroupNames = StaticCollections.ProductGroupNames;
        }

        private bool SalesCompletionDateMissing()
        {
            if ((Project.ProjectStatusID == (int)ProjectStatusType.Completed))
            {
                if (Project.CompletedDate != null)
                    return false;
                else
                    return true;
            }
            return false;
        }

        private void CheckFieldValidation()
        {
            bool ApplicationRequired;
            bool AssociateRequired;
            bool CustomerRequired;
            bool EstimatedAnnualGMRequired;
            bool EstimatedAnnualSalesRequired;          
            bool MarketSegmentRequired;
            bool NewBusinessCategoryRequired;
            bool SalesDivisionRequired;
            bool ProjectNameRequired;
            bool EstimatedAnnualMPCRequired;
            bool ProbabilityOfSuccessRequired;

            SalesDivisionRequired = !(Project.SalesDivisionID > 0);
            ProjectNameRequired = (Project.GOM.Name == string.Empty);
            MarketSegmentRequired = !(Project.MarketSegmentID > 0);
            ApplicationRequired = !(Project.ApplicationID > 0);
            CustomerRequired = !(Project.CustomerID > 0);
            AssociateRequired = !(Project.OwnerID > 0);
            NewBusinessCategoryRequired = !(Project.NewBusinessCategoryID > 0 && Project.NewBusinessCategoryID != -1);
            EstimatedAnnualSalesRequired = !(Project.EstimatedAnnualSales > 0);
            EstimatedAnnualGMRequired = !(Project.GM > 0);
            EstimatedAnnualMPCRequired = !(Project.EstimatedAnnualMPC > 0);
               
            ProbabilityOfSuccessRequired = !(Project.ProbabilityOfSuccess > 0);
            InvalidField = (
                SalesDivisionRequired
                || MarketSegmentRequired
                || ApplicationRequired 
                || CustomerRequired
                || AssociateRequired                
                || ProjectNameRequired
                || NewBusinessCategoryRequired
                || InValidProduct
                || EstimatedAnnualSalesRequired
                || EstimatedAnnualGMRequired
                || EstimatedAnnualMPCRequired                                            
                || ProbabilityOfSuccessRequired
                );

            if (SalesDivisionRequired)
                DataMissingLabel = "Industry Missing";
            else
                if (MarketSegmentRequired)
                DataMissingLabel = "Industry Segment Missing";
            else
                if (ApplicationRequired)
                DataMissingLabel = "Application Missing";
            else
                if (CustomerRequired)
                DataMissingLabel = "Customer Missing";
            else
                if (AssociateRequired)
                DataMissingLabel = "Associate Missing";
            else
                if (ProjectNameRequired)
                DataMissingLabel = "Project Name Missing";
            else
                if (NewBusinessCategoryRequired)
                DataMissingLabel = "Opportunity Category Missing";
            else
                if (InValidProduct)
                DataMissingLabel = "Error in Products List";
            else
                if (EstimatedAnnualSalesRequired)
                DataMissingLabel = "Estimated Annual Sales Missing";
            else
                if (EstimatedAnnualGMRequired)
                DataMissingLabel = "Estimated Gross Margin Missing";
            else              
                if (EstimatedAnnualMPCRequired)
                DataMissingLabel = "Estimated MPC Missing";            
            else
                if (ProbabilityOfSuccessRequired)
                DataMissingLabel = "Probability Of Success Missing";                                                
        }

        bool invalidfield;
        public bool InvalidField
        {
            get { return invalidfield; }
            set { SetField(ref invalidfield, value); }
        }           
       
        string datamissing;
        public string DataMissingLabel
        {
            get { return datamissing; }
            set { SetField(ref datamissing, value); }
        }
        
        MilestoneModel selectedmilestone;
        public MilestoneModel SelectedMilestone
        {
            get { return selectedmilestone; }
            set { SetField(ref selectedmilestone, value); }
        }

        EPModel selectedevaluationplan;
        public EPModel SelectedEvaluationPlan
        {
            get { return selectedevaluationplan; }
            set { SetField(ref selectedevaluationplan, value); }
        }

        #endregion

        #region Application Filtering

        FullyObservableCollection<ApplicationCategoriesModel> applicationcategoriesmaster;
        FullyObservableCollection<ApplicationCategoriesModel> applicationcategories;
        public FullyObservableCollection<ApplicationCategoriesModel> ApplicationCategories
        {
            get { return applicationcategories; }
            set
            {
                SetField(ref applicationcategories, value);
                if (value == null)                
                    Project.ApplicationID = 0;                
            }
        }

        private void GetApplicationCategoriesList()
        {
            applicationcategoriesmaster = GetApplicationCategories();
        }     

        private void FilterApplications(int indvalue, int mktsegid)
        {
            var subs = from p in applicationcategoriesmaster
                        where p.IndustryID == indvalue
                        || indvalue == 4 && mktsegid == 4 && p.IndustryID == 1
                        select p;
             
            FullyObservableCollection<ApplicationCategoriesModel> newapplicationcategories = new FullyObservableCollection<ApplicationCategoriesModel>();
            bool applicationidfound = false;
            foreach (ApplicationCategoriesModel ac in subs)
            {
                newapplicationcategories.Add(ac);
                if (ac.GOM.ID == Project.ApplicationID)
                    applicationidfound = true;
            }

            if (!applicationidfound)
                Project.ApplicationID = 0;

            ApplicationCategories = newapplicationcategories;
        }

        #endregion

        #region MarketSegment Fitering

        FullyObservableCollection<MarketSegmentModel> marketsegments;
        public FullyObservableCollection<MarketSegmentModel> MarketSegments
        {
            get { return marketsegments; }
            set {
                SetField(ref marketsegments, value);
                if (value == null)
                    Project.MarketSegmentID = 0;
            }
        }

        FullyObservableCollection<MarketSegmentModel> marketsegmentsmaster;
        private void GetMarketSegmentsMaster()
        {
            marketsegmentsmaster = GetMarketSegments();
        }

        private void FilterMarketSegments(int idvalue)
        {
            var subs = marketsegmentsmaster.Where(x => x.IndustryID == idvalue);
            FullyObservableCollection<MarketSegmentModel> newmarketsegments = new FullyObservableCollection<MarketSegmentModel>();

            bool marketsegmentidfound = false;
            foreach (MarketSegmentModel ac in subs)
            {
                newmarketsegments.Add(ac);
                if (ac.GOM.ID == Project.MarketSegmentID)
                    marketsegmentidfound = true;
            }

            if (!marketsegmentidfound)
                Project.MarketSegmentID = 0;

            MarketSegments = newmarketsegments;
        }


        #endregion
        
        #region SMCode Filtering
               
        public FullyObservableCollection<SMCodeModel> SMCodes
        {
            get { return smcodes; }
            set
            {
                SetField(ref smcodes, value);
                if (value == null)                
                    Project.SMCodeID = -1;
                
            }
        }

        SMCodeModel smcode;
        public SMCodeModel SelectedSMCode
        {
            get { return smcode; }
            set { SetField(ref smcode, value); }
        }

        int smcodeindx;
        public int SelectedSMCodeIndex
        {
            get { return smcodeindx; }
            set { SetField(ref smcodeindx, value); }
        }

        private void FilterSMCodes(int indvalue)
        {           
            FullyObservableCollection<SMCodeModel> newsmcodes = new FullyObservableCollection<SMCodeModel>();
            int index=0;
            int i = 0;
            foreach (SMCodeModel ac in StaticCollections.SMCodes)
            {
                if (indvalue == ac.SalesDivisionID)
                {
                    newsmcodes.Add(ac);
                    if (ac.GOM.ID == Project.SMCodeID)
                        index = i;
                
                    i++;
                }
            }
           
            SMCodes = newsmcodes;
            if(index==0)
                Project.SMCodeID = -1;

            SelectedSMCodeIndex = index;
           
        }

        #endregion
        
        #region Commands

        private bool CanExecuteSave(object obj)
        {
            if (InValidProduct)
                return false;

            if (InvalidField)
                return false;

            return true;
        }

        ICommand saveproject;
        public ICommand SaveProject
        {
            get
            {
                if (saveproject == null)
                    saveproject = new DelegateCommand(CanExecuteSave, ExecuteSaveProject);
                return saveproject;
            }
        }

        private void ExecuteSaveProject(object parameter)
        {
            IMessageBoxService msgbox = new MessageBoxService();
            if (parameter != null)
            {
                if ((parameter.GetType().Equals(typeof(ProjectModel))))
                {
                    SaveProjectFlag = true;
                  
                    if ((parameter as ProjectModel).GOM.ID != 0)
                    {
                        //update db
                        try
                        {                                
                          //  BusyIndicator.ShowBusy("Updating Project...");
                            UpdateProject(project);                            
                        }
                        catch (Exception e)
                        {
                            msgbox.ShowMessage("Unable to update project\n " + e.Message, "Error during updating project", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
                        }
                        finally
                        {
                  //          BusyIndicator.CloseBusy();
                            //close dialog
                            CloseWindow();
                        }                            
                    }                    
                }
            }
            msgbox = null;
        }

              
        private bool CanExecuteAddNewMilestone(object obj)
        {
            return true;
        }

        ICommand addnewmilestone;
        public ICommand AddNewMilestone
        {
            get
            {
                if (addnewmilestone == null)
                    addnewmilestone = new DelegateCommand(CanExecuteAddNewMilestone, ExecuteAddNewMilestone);
                return addnewmilestone;
            }
        }

        private void ExecuteAddNewMilestone(object parameter)
        {
            try
            {
                Window owner;
                owner = Application.Current.Windows[0];
                IMessageBoxService msgbox = new MessageBoxService();
                bool result = msgbox.MilestoneDialog(owner, 0, project.GOM.ID);
                //if return value is true then Refresh list
                if (result == true)
                    project.Milestones = GetProjectMilestones(project.GOM.ID);                            

                msgbox = null;               
            }
            catch { }            
        }

        private bool CanExecuteEditMilestone(object obj)
        {
            return (Project.GOM.ID > 0);
        }

        ICommand editmilestone;
        public ICommand OpenMilestone
        {
            get
            {
                if (editmilestone == null)
                    editmilestone = new DelegateCommand(CanExecuteEditMilestone, ExecuteEditMilestone);
                return editmilestone;
            }
        }

        private void ExecuteEditMilestone(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    if (parameter.GetType().Equals(typeof(MilestoneModel)))
                    {
                        Window owner;
                        owner = Application.Current.Windows[0];
                        IMessageBoxService msgbox = new MessageBoxService();
                        bool result = msgbox.MilestoneDialog(owner,((MilestoneModel)parameter).GOM.ID, ((MilestoneModel)parameter).ProjectID);
                        //if return value is true then Refresh list
                        if (result == true)
                            project.Milestones = GetProjectMilestones(project.GOM.ID);

                        msgbox = null;
                    }
                }
            }
            catch { }
        }

        private bool CanExecuteDeleteMilestone(object obj)
        {           
           return (Project.Milestones.Count > 0);           
        }

        ICommand deletemilestone;
        public ICommand DeleteMilestone
        {
            get
            {
                if (deletemilestone == null)
                    deletemilestone = new DelegateCommand(CanExecuteDeleteMilestone, ExecuteDeleteMilestone);
                return deletemilestone;
            }
        }

        private void ExecuteDeleteMilestone(object parameter)
        {
            if (SelectedMilestone != null)
            {
                IMessageBoxService msgbox = new MessageBoxService();
                GenericMessageBoxResult result = msgbox.ShowMessage("Please confirm that you want to delete this milestone", "Confirm Deletion", GenericMessageBoxButton.OKCancel,
                     GenericMessageBoxIcon.Question);
                //if return value is Ok then Refresh list
                if (result.Equals(GenericMessageBoxResult.OK))
                {                    
                    SelectedMilestone.GOM.Deleted = true;
                    DatabaseQueries.UpdateMilestone(SelectedMilestone);
                    project.Milestones = GetProjectMilestones(project.GOM.ID);
                }
                msgbox = null;
            }
        }

        private bool CanExecuteAddNewEvaluationPlan(object obj)
        {
            return true;
        }

        ICommand addnewep;
        public ICommand AddNewEvaluationPlan
        {
            get
            {
                if (addnewep == null)
                    addnewep = new DelegateCommand(CanExecuteAddNewEvaluationPlan, ExecuteAddNewEvaluationPlan);
                return addnewep;
            }
        }

        private void ExecuteAddNewEvaluationPlan(object parameter)
        {
            try
            {
                Window owner;
                owner = Application.Current.Windows[0];
                IMessageBoxService msgbox = new MessageBoxService();
                bool result = msgbox.EvaluationPlanDialog(owner, 0, project.GOM.ID);
                //if return value is true then Refresh list
                if (result == true)
                    project.EvaluationPlans = GetProjectEvaluationPlans(project.GOM.ID);

                msgbox = null;
            }
            catch { }
        }

        private bool CanExecuteEditEvaluationPlan(object obj)
        {
            return (Project.GOM.ID > 0);
        }

        ICommand editep;
        public ICommand OpenEvaluationPlan
        {
            get
            {
                if (editep == null)
                    editep = new DelegateCommand(CanExecuteEditEvaluationPlan, ExecuteEditEvaluationPlan);
                return editep;
            }
        }

        private void ExecuteEditEvaluationPlan(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    if (parameter.GetType().Equals(typeof(EPModel)))
                    {
                        Window owner;
                        owner = Application.Current.Windows[0];
                        IMessageBoxService msgbox = new MessageBoxService();
                        bool result = msgbox.EvaluationPlanDialog(owner, ((EPModel)parameter).GOM.ID, ((EPModel)parameter).ProjectID);
                        //if return value is true then Refresh list
                        if (result == true)
                            project.EvaluationPlans = GetProjectEvaluationPlans(project.GOM.ID);

                        msgbox = null;
                    }
                }
            }
            catch { }
        }

        private bool CanExecuteDeleteEvaluationPlan(object obj)
        {
            return (Project.EvaluationPlans.Count > 0);
        }

        ICommand deleteep;
        public ICommand DeleteEvaluationPlan
        {
            get
            {
                if (deleteep == null)
                    deleteep = new DelegateCommand(CanExecuteDeleteEvaluationPlan, ExecuteDeleteEvaluationPlan);
                return deleteep;
            }
        }

        private void ExecuteDeleteEvaluationPlan(object parameter)
        {
            if (SelectedEvaluationPlan != null)
            {
                IMessageBoxService msgbox = new MessageBoxService();
                GenericMessageBoxResult result = msgbox.ShowMessage("Please confirm that you want to delete this evaluation plan", "Confirm Deletion", GenericMessageBoxButton.OKCancel,
                     GenericMessageBoxIcon.Question);
                //if return value is Ok then Refresh list
                if (result.Equals(GenericMessageBoxResult.OK))
                {
                    SelectedEvaluationPlan.GOM.Deleted = true;
                    DatabaseQueries.UpdateEvaluationPlan(SelectedEvaluationPlan);
                    project.EvaluationPlans = GetProjectEvaluationPlans(project.GOM.ID);
                }
                msgbox = null;
            }
        }
        #endregion

    }
}