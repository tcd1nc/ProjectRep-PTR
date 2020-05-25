using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using static PTR.StaticCollections;
using static PTR.DatabaseQueries;
using System.Collections.Generic;
using PTR.Models;
using System.Windows;
using System.Globalization;

namespace PTR.ViewModels
{
    public class ProjectViewModel : ViewModelBase
    {

        private const string title = "Project Details";
        ProjectModel project;
        bool isnew = false;
        public bool isdirty = false;


        public ProjectViewModel()
        {
            //new project
            isnew = true;
            Initialisedropdowns();
            MakeProductTT();
            project = new ProjectModel
            {
                OwnerID = 0,
                CustomerID = 0,

                ID = 0,
                Deleted = false,
                Name = string.Empty,
                Description = string.Empty,
                ProjectStatusID = (int)ProjectStatusType.Active,
                SalesDivisionID = 0,
                IndustrySegmentID = 0,
                EstimatedAnnualSales = 0,
                EstimatedAnnualMPC = 0,
                Resources = string.Empty,
                ActivatedDate = DateTime.Now,
                TargetedVolume = 0,
                GM = 0,
                SMCodeID = 0,
                ApplicationID = 0,
                NewBusinessCategoryID = 0,
                Products = string.Empty,
                ProbabilityOfSuccess = 100,
                ProjectTypeID = 1,
                KPM = false,               
                DifferentiatedTechnology = false,
                EPRequired = Config.EPRequired,
                Milestones = new FullyObservableCollection<MilestoneModel>(),
                EvaluationPlans = new FullyObservableCollection<EPModel>(),
                Comments = string.Empty,
                IncompleteReasonID = 0,
                PriorityID = 0,
                SponsorID = 0,
                MiscDataID=0,
                AllowNonOwnerEdits = true,
                AllowNonOwnerMileStoneAccess = true,
                UnitCost = 0
                               
            };

            Project.ID = AddProject(project);

            ProjectNameEnabled = true;
            ProjectStatus = Project.ProjectStatusID;
            Project.PropertyChanged += _project_PropertyChanged;
            SetUserAccessForNewProject();

            SetInitial();

        }

        public ProjectViewModel(int projectID)
        {
            Initialisedropdowns();
            MakeProductTT();
            Project = GetSingleProject(projectID);
            Project.Milestones = GetProjectMilestones(projectID);
            Project.EvaluationPlans = GetProjectEvaluationPlans(projectID);

            ProjectStatus = Project.ProjectStatusID;
            Project.PropertyChanged += _project_PropertyChanged;
            ProjectNameEnabled = false;

            SetInitial();

        }


        #region User access control    

        private void SetUserAccessExistingProject(int customerid)
        {
            int accessid = StaticCollections.GetUserCustomerAccess(customerid);

            if (Project.ProjectStatusID == (int)ProjectStatusType.Active)
            {                               
                //Project Owner can always add/delete/edit milestones and add/delete/edit Evaluation Plans
                if ((CurrentUser.ID == Project.OwnerID || CurrentUser.ID == Project.CreatorID) && accessid == (int)UserPermissionsType.FullAccess)
                {
                    IsEnabled = true;
                    EPEnabled = true;
                    AddEPIsEnabled = true;
                    DeleteEPIsEnabled = true;
                    AddMilestoneIsEnabled = true;
                    DeleteMilestoneIsEnabled = true;
                    MilestoneIsEnabled = true;                  
                }
                else
                {                   
                    //Non-project owner or creator can add/delete/edit milestones
                    if (Project.AllowNonOwnerMileStoneAccess)
                    {
                        //must have full access
                        if (accessid == (int)UserPermissionsType.FullAccess)
                        {                            
                            //if current user is a milestone owner (assigned by project owner) and non-project owners are allowed to edit assigned milestones, 
                            //then they can edit but not create or delete the assigned milestones                           
                            if (IsInMilestoneUsers(CurrentUser.ID))
                                MilestoneIsEnabled = true;
                            else
                            {
                                AddMilestoneIsEnabled = false;
                                DeleteMilestoneIsEnabled = false;
                                MilestoneIsEnabled = false;
                            }
                        }
                        else  
                        {
                            AddMilestoneIsEnabled = false;
                            DeleteMilestoneIsEnabled = false;
                            MilestoneIsEnabled = false;
                        }                        
                    }
                    else
                    {
                        AddMilestoneIsEnabled = false;
                        DeleteMilestoneIsEnabled = false;
                        MilestoneIsEnabled = false;
                    }

                    //Non-project owner can edit project details including milestones
                    if (Project.AllowNonOwnerEdits)
                    {
                        //User must have full access permission to edit this
                        if(accessid == (int)UserPermissionsType.FullAccess)
                        {
                            IsEnabled = true;
                            EPEnabled = true;
                            AddEPIsEnabled = true;
                            DeleteEPIsEnabled = true;                            
                        }
                        else
                        {
                            IsEnabled = false;
                            EPEnabled = false;
                            AddEPIsEnabled = false;
                            DeleteEPIsEnabled = false;                           
                        }
                    } 
                    else
                    //prevent non-project owners from editing details other than their assigned milestones
                    {
                        IsEnabled = false;
                        EPEnabled = false;
                        AddEPIsEnabled = false;
                        DeleteEPIsEnabled = false;                        
                    }
                }                                                                                        
            }
            else //cancelled or completed projects
            if (CurrentUser.AllowEditCompletedCancelled
                && accessid == (int)UserPermissionsType.FullAccess
                && (CurrentUser.ID == Project.OwnerID
                    || CurrentUser.ID == Project.CreatorID 
                    || (CurrentUser.ID != Project.OwnerID && Project.AllowNonOwnerEdits)))
            {
                IsEnabled = true;
                EPEnabled = false;
                AddEPIsEnabled = false;
                DeleteEPIsEnabled = false;
                AddMilestoneIsEnabled = false;
                DeleteMilestoneIsEnabled = false;
                MilestoneIsEnabled = false;

            }

            if (CurrentUser.Administrator)
            {
                IsEnabled = true;
                EPEnabled = true;
                AddEPIsEnabled = true;
                DeleteEPIsEnabled = true;
                AddMilestoneIsEnabled = true;
                DeleteMilestoneIsEnabled = true;
                MilestoneIsEnabled = true;
            }
        }

        private bool IsInMilestoneUsers(int userid)
        {
            bool isfound = false;
            foreach (MilestoneModel mm in Project.Milestones)
                if (mm.UserID == userid)
                    return true;
            return isfound;
        }

        private void SetUserAccessForNewProject()
        {
            IsEnabled = true;
            EPEnabled = true;
            MilestoneIsEnabled = true;
            AddMilestoneIsEnabled = true;
            DeleteMilestoneIsEnabled = true;
            AddEPIsEnabled = true;
            DeleteEPIsEnabled = true;
        }

        #endregion

        #region Event handlers

        private void _project_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Products")
                InValidProduct = IsInValidProduct((sender as ProjectModel).Products);

            if (e.PropertyName != "EvaluationPlans" && e.PropertyName != "Milestones")
            {
                CheckFieldValidation();
                CalcPercentGM();
                isdirty = true;
            }
        }

        #endregion

        #region Properties

        string windowtitle;
        public string WindowTitle
        {
            get { return windowtitle; }
            set { SetField(ref windowtitle, value); }
        }

        bool isenabled = false;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        bool epenabled = false;
        public bool EPEnabled
        {
            get { return epenabled; }
            set { SetField(ref epenabled, value); }
        }

        bool milestoneisenabled = false;
        public bool MilestoneIsEnabled
        {
            get { return milestoneisenabled; }
            set { SetField(ref milestoneisenabled, value); }
        }

        public bool AddMilestoneIsEnabled;
        public bool DeleteMilestoneIsEnabled;
            
        bool AddEPIsEnabled = false;
        bool DeleteEPIsEnabled = false;
        
        public FullyObservableCollection<ModelBaseVM> ProductGroupNames { get; private set; }
        public FullyObservableCollection<ProjectTypeModel> ProjectTypesList { get; private set; }
        public FullyObservableCollection<ModelBaseVM> NewBusinessCategories { get; private set; }
        public FullyObservableCollection<ModelBaseVM> BusinessUnits { get; private set; }
        public Collection<EnumValue> ProjectStatusTypesList { get; private set; }
        public FullyObservableCollection<ModelBaseVM> Reasons { get; private set; }
        public FullyObservableCollection<ModelBaseVM> Priorities { get; private set; }
       
        FullyObservableCollection<CustomerModel> customers;
        public FullyObservableCollection<CustomerModel> Customers
        {
            get { return customers; }
            set { SetField(ref customers, value); }
        }

        FullyObservableCollection<UserModel> associates;
        public FullyObservableCollection<UserModel> Associates
        {
            get { return associates; }
            set { SetField(ref associates, value); }
        }

        public ProjectModel Project
        {
            get { return project; }
            set { SetField(ref project, value); }
        }

        int projstatus;
        public int ProjectStatus
        {
            get { return projstatus; }
            set
            {
                if ((value == (int)ProjectStatusType.Completed) || (value == (int)ProjectStatusType.Cancelled))
                {
                    CompletedDateEnabled = true;
                    Project.CompletedDate = DateTime.Now;
                }
                else
                if (value == (int)ProjectStatusType.Active)
                {
                    //clear date completed
                    CompletedDateEnabled = false;
                    Project.CompletedDate = null;
                }
                //else
                //if ((project.ProjectStatusID == (int)ProjectStatusType.Completed)
                //    || (project.ProjectStatusID == (int)ProjectStatusType.Cancelled))                    
                //    CompletedDateEnabled = true;

                Project.ProjectStatusID = value;

                ReasonForIncompleteProjectEnabled = !Project.IsNewBusiness && (value == (int)ProjectStatusType.Completed || value == (int)ProjectStatusType.Cancelled);
                if (!ReasonForIncompleteProjectEnabled)
                    Project.IncompleteReasonID = 0;

                SetField(ref projstatus, value);
            }
        }

        bool completeddateenabled;
        public bool CompletedDateEnabled
        {
            get { return completeddateenabled; }
            set { SetField(ref completeddateenabled, value); }
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


        ProjectTypeModel selectedprojecttype;
        public ProjectTypeModel SelectedProjectType
        {
            get { return selectedprojecttype; }
            set
            {
                SetField(ref selectedprojecttype, value);
                if (value != null)
                {
                    ///////<<<<<<<<<<<<<<<<<<<<<<<<<<<<<
                    FilterMiscellaneousData(value.ID);
                    ShowSponsor = value.ShowSponsor;
                    ShowUnitCost = value.ShowUnitCost;
                    MiscellaneousDataLabel = value.MiscellaneousDataLabel;
                }
            }
        }

        bool misccomboenabled;
        public bool MiscComboEnabled
        {
            get { return misccomboenabled; }
            set { SetField(ref misccomboenabled, value); }
        }

        bool showsponsor;
        public bool ShowSponsor
        {
            get { return showsponsor; }
            set { SetField(ref showsponsor, value);
                if(value == false)
                    Project.SponsorID = 0;
            }
        }

        bool showunitcost = false;
        public bool ShowUnitCost
        {
            get { return showunitcost; }
            set
            {
                SetField(ref showunitcost, value);
                if (value == false)
                    Project.UnitCost = 0;
            }
        }

        string miscellaneousdatalabel;
        public string MiscellaneousDataLabel
        {
            get { return miscellaneousdatalabel; }
            set { SetField(ref miscellaneousdatalabel, value); }
        }
        
        ModelBaseVM selectedsalesdivision;
        public ModelBaseVM SelectedSalesDivision
        {
            get { return selectedsalesdivision; }
            set
            {
                SetField(ref selectedsalesdivision, value);
                if (value != null)
                {
                    FilterIndustrySegments(value.ID);
                    FilterSMCodes(value.ID);
                }
            }
        }

        IndustrySegmentModel selectedmktseg;
        public IndustrySegmentModel SelectedIndustrySegment
        {
            get { return selectedmktseg; }
            set
            {
                SetField(ref selectedmktseg, value);
                if (value != null && Project.SalesDivisionID != 0) // SelectedSalesDivision != null)
                    FilterApplications(value.ID);
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
                    SetUserAccessExistingProject(value.ID);
                    GetOwnersList(value.ID);
                    GetCurrencySymbol(value.CultureCode);
                }
            }
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

        string currencysymbol = "$";
        public string CurrencySymbol
        {
            get { return currencysymbol; }
            set { SetField(ref currencysymbol, value); }
        }

        bool estimatedsaleserror;
        public bool EstimatedSalesError
        {
            get { return estimatedsaleserror; }
            set {
                SetField(ref estimatedsaleserror, value);
                CheckFieldValidation();
            }
        }

        bool estimatedgmerror;
        public bool EstimatedGMError
        {
            get { return estimatedgmerror; }
            set
            {
                SetField(ref estimatedgmerror, value);
                CheckFieldValidation();
            }
        }

        bool estimatedmpcerror;
        public bool EstimatedMPCError
        {
            get { return estimatedmpcerror; }
            set
            {
                SetField(ref estimatedmpcerror, value);
                CheckFieldValidation();
            }
        }

        bool targetedvolumeerror;
        public bool TargetedVolumeError
        {
            get { return targetedvolumeerror; }
            set
            {
                SetField(ref targetedvolumeerror, value);
                CheckFieldValidation();
            }
        }

        bool probofsuccesserror;
        public bool ProbOfSuccessError
        {
            get { return probofsuccesserror; }
            set
            {
                SetField(ref probofsuccesserror, value);
                CheckFieldValidation();
            }
        }

        bool unitcosterror;
        public bool UnitCostError
        {
            get { return unitcosterror; }
            set
            {
                SetField(ref unitcosterror, value);
                CheckFieldValidation();
            }
        }

        bool reasonforincompleteprojectenabled = false;
        public bool ReasonForIncompleteProjectEnabled
        {
            get { return reasonforincompleteprojectenabled; }
            set { SetField(ref reasonforincompleteprojectenabled, value); }
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

        string producttt = "Product names must be separated by a comma and space and formatted like: \nBusan 1009, Busan 1455, BLX 12345\n";
        public string ProductTT
        {
            get { return producttt; }
            set { SetField(ref producttt, value); }
        }


        #endregion

        #region Private Functions

        private void MakeProductTT()
        {
            if(Config.ProductDelimiter == ',')
                ProductTT = "Product names must be separated by a comma and space and formatted like: \nBusan 1009, Busan 1455, BLX 12345\n";            
            else
                ProductTT = "Product names must be separated by a semi-colon and space and formatted like: \nBusan 1009; Busan 1455; BLX 12345\n";
        }

        private void SetInitial()
        {
            //CheckFieldValidation();
            CalcPercentGM();

            if (project.ID == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (ID: " + project.ID.ToString() + ")";

            MaxProjNameLength = Config.MaxProjectNameLength;
                       
        }

        private void Initialisedropdowns()
        {
            GetIndustrySegmentsMaster();
            GetMiscellaneousDataMaster();
            GetApplicationsList();
            ProjectTypesList = ProjectTypes;
            GetProjectStatusList();
            if (isnew)
                GetNewProjectCustomerList();
            else
                GetCustomerList();
            BusinessUnits = StaticCollections.BusinessUnits;

            GetNewBusinessCategories();
            GetProductGroupNames();
            GetSMCodesMaster();
            GetReasonsForIncompleteProject();
            GetPriorities();
            GetMiscellaneousData();
        }

        private void GetProjectStatusList()
        {
            ProjectStatusTypesList = EnumerationLists.ProjectStatusTypesList;
        }

        private void GetNewProjectCustomerList()
        {
            FullyObservableCollection<CustomerModel> cm = new FullyObservableCollection<CustomerModel>();
            FullyObservableCollection<CustomerModel> customers = GetCustomers();
            int accessid = 0;
            foreach (CustomerModel cust in customers)
            {
                accessid = StaticCollections.GetUserCustomerAccess(cust.ID);
                if (accessid == (int)UserPermissionsType.FullAccess)
                    if (!cust.Deleted)
                    {
                        cust.IsEnabled = true;
                        cm.Add(cust);
                    }
            }
            Customers = cm;
        }

        private void GetCustomerList()
        {
            FullyObservableCollection<CustomerModel> cm = new FullyObservableCollection<CustomerModel>();
            FullyObservableCollection<CustomerModel> customers = GetCustomers();
            foreach (CustomerModel cust in customers)
            {
                if (!cust.Deleted)
                    cust.IsEnabled = (StaticCollections.GetUserCustomerAccess(cust.ID) == (int)UserPermissionsType.FullAccess);
                else
                    cust.IsEnabled = false;

                cm.Add(cust);
            }
            Customers = cm;
        }

        private void GetReasonsForIncompleteProject()
        {
            Reasons = DatabaseQueries.GetReasonsForIncompleteProject();
        }

        private void GetPriorities()
        {
            Priorities = DatabaseQueries.GetPriorities();
        }
       
        UserModel selecteduser;
        public UserModel SelectedUser
        {
            get { return selecteduser; }
            set { SetField(ref selecteduser, value); }
        }

        private void GetOwnersList(int customerid)
        {
            Associates?.Clear();

            if (customerid > 0)
            {
                FullyObservableCollection<UserModel> associatess = GetCustomerUserAccess(customerid);
                FullyObservableCollection<UserModel> newassociatess = new FullyObservableCollection<UserModel>();

                if (!isnew)
                {
                    foreach (UserModel ag in associatess)
                    {
                        if (!ag.Deleted || (ag.Deleted && ag.ID == Project.OwnerID))
                            newassociatess.Add(new UserModel()
                            {
                                ID = ag.ID,
                                Name = ag.Name,
                                LoginName = ag.LoginName,
                                Administrator = ag.Administrator,
                                IsEnabled = !ag.Deleted
                            });
                    }
                    Associates = newassociatess;
                }
                else //remove any deleted associates from list
                {
                    foreach (UserModel ag in associatess)
                    {
                        if (!ag.Deleted)
                        {
                            newassociatess.Add(new UserModel()
                            {
                                ID = ag.ID,
                                Name = ag.Name,
                                LoginName = ag.LoginName,
                                Administrator = ag.Administrator,
                                IsEnabled = true

                            });
                        }
                    }
                    Associates = newassociatess;
                }
                UserModel q = Associates.Where(x => x.ID == Project.OwnerID).FirstOrDefault();
                if (q != null)
                    SelectedUser = q;
                else
                {
                    SelectedUser = null;
                    Project.OwnerID = 0;
                }
            }
        }

        private void GetNewBusinessCategories()
        {
            NewBusinessCategories = DatabaseQueries.GetNewBusinessCategories();
        }

        private void CalcPercentGM()
        {            
            if (Project.EstimatedAnnualSales > 0 && Project.GM > 0)
                PercentGM = Project.GM / Project.EstimatedAnnualSales;
            else
                PercentGM = 0;
            
        }

        private void GetCurrencySymbol(string culturecode)
        {
            CultureInfo ci = new CultureInfo(culturecode);
            CurrencySymbol = ci.NumberFormat.CurrencySymbol;
        }              
               
        #endregion

        #region Validation

        private bool IsInValidProduct(string product)
        {
            if (Config.ValidateProducts)
            {
                if (string.IsNullOrEmpty(product.Trim()) && SelectedProjectType.ProductRequired)
                    return true;
                else
                {
                    if (ProductNamesOK(product))
                        return false;
                    else
                        return true;
                }
            }
            else
                return false;
        }

        private string ConvertoTitleCase(string input)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return textInfo.ToTitleCase(input.TrimStart());
        }

        private bool ProductNamesOK(string srcproduct)
        {
            Collection<string> productsubstrings = new Collection<string>();
            List<string> prodslist = srcproduct.Split(Config.ProductDelimiter).ToList();
            try
            {
                //check for trailing ,
                if (srcproduct.Length > 0)
                    if (srcproduct.LastIndexOf(Config.ProductDelimiter) == srcproduct.Length - 1)
                        return false;

                foreach (string s in prodslist)
                {
                    char[] chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

                    string cleaneds = ConvertoTitleCase(s.ToLower());
                    // busan =>Busan
                    //Optimyze pLUS =>Optimyze Plus
                    //   optimyze plus 444=>Optimyze Plus 444

                    //if (cleaneds.StartsWith("Blx"))
                    //    cleaneds = cleaneds.Replace("Blx", "BLX");
                    //if (cleaneds.StartsWith("Brd"))
                    //    cleaneds = cleaneds.Replace("Brd", "BRD");

                    //parse up to first number or end of string
                    int idxnumber = cleaneds.IndexOfAny(chars);

                    int termlen = 0;
                    if (idxnumber == -1)
                        termlen = cleaneds.Length;
                    else
                    {
                        if (idxnumber < 2)
                            termlen = cleaneds.Length;
                        else
                        {
                            if (idxnumber > 1)
                            {
                                if (cleaneds[idxnumber - 1] == ' ')
                                    termlen = idxnumber - 1;
                                else
                                    termlen = cleaneds.Length;
                            }
                            else
                                termlen = cleaneds.Length;
                        }
                    }
                    string temp = string.Empty;
                    temp = cleaneds.Substring(0, termlen);
                    bool containsOther = ProductGroupNames.Select(x => x.Name == "Other").Count() > 0;

                    foreach (ModelBaseVM v in ProductGroupNames)
                    {                        
                        if (temp == v.Name || (temp.StartsWith("Other") && containsOther))
                        {
                            productsubstrings.Add(cleaneds);
                            break;
                        }
                        else                                                                     
                        if (temp.ToUpper() == v.Name.ToUpper() && v.IsSelected)
                        {
                            productsubstrings.Add(cleaneds.ToUpper());
                            break;
                        }                        
                    }
                }
            }
            catch
            {
                return true;
            }

            if (prodslist.Count == productsubstrings.Count)
            {
                Project.Products = string.Join(Config.ProductDelimiter.ToString() + " ", productsubstrings);
                return true;
            }
            else
                return false;
        }

        private void GetProductGroupNames()
        {
            ProductGroupNames = DatabaseQueries.GetProductGroupNames();
        }

        private void CheckFieldValidation()
        {
            bool ApplicationRequired;
            bool AssociateRequired;
            bool CustomerRequired;
            bool EstimatedAnnualGMRequired;
            bool EstimatedAnnualSalesRequired;
            bool SalesVolumeRequired;
            bool IndustrySegmentRequired;
            bool NewBusinessCategoryRequired;
            bool BusinessUnitRequired;
            bool ProjectNameRequired;
            bool EstimatedAnnualMPCRequired;
            bool ProbabilityOfSuccessRequired;
            bool ReasonForIncompleteRequired;
            bool SponsorRequired;
            bool MiscellaneousDataRequired;
            bool PriorityRequired;

            BusinessUnitRequired = !(Project.SalesDivisionID > 0);
            ProjectNameRequired = string.IsNullOrEmpty(Project.Name);
            IndustrySegmentRequired = !(Project.IndustrySegmentID > 0);
            ApplicationRequired = !(Project.ApplicationID > 0);
            CustomerRequired = !(Project.CustomerID > 0);
            AssociateRequired = !(Project.OwnerID > 0);
            NewBusinessCategoryRequired = !(Project.NewBusinessCategoryID > 0) && SelectedProjectType.OpportunityCatRequired;
            EstimatedAnnualSalesRequired = !(Project.EstimatedAnnualSales > 0) && SelectedProjectType.SalesRequired;            
            EstimatedAnnualGMRequired = !(Project.GM > 0) && SelectedProjectType.GMRequired;
            EstimatedAnnualMPCRequired = !(Project.EstimatedAnnualMPC > 0) && SelectedProjectType.MPCRequired;
            SalesVolumeRequired = !(Project.TargetedVolume > 0) && SelectedProjectType.SalesVolumeRequired;
            ProbabilityOfSuccessRequired = !(Project.ProbabilityOfSuccess > 0) && SelectedProjectType.ProbabilityRequired;
            ReasonForIncompleteRequired = !(Project.IncompleteReasonID > 0)
                && !Project.IsNewBusiness && (Project.ProjectStatusID == (int)ProjectStatusType.Completed || Project.ProjectStatusID == (int)ProjectStatusType.Cancelled);

            SponsorRequired = ShowSponsor && Project.SponsorID == 0;
            MiscellaneousDataRequired = (MiscellaneousData?.Count > 0) && !(Project.MiscDataID > 0);

            PriorityRequired = !(Project.PriorityID > 0) && SelectedProjectType.ShowPriority;
            InValidProduct = IsInValidProduct(project.Products);

            InvalidField = (
                BusinessUnitRequired
                || IndustrySegmentRequired
                || ApplicationRequired
                || CustomerRequired
                || AssociateRequired
                || ProjectNameRequired
                || NewBusinessCategoryRequired
                || ReasonForIncompleteRequired
                || InValidProduct
                || EstimatedAnnualSalesRequired              
                || EstimatedAnnualGMRequired
                || EstimatedAnnualMPCRequired
                || SalesVolumeRequired
                || ProbabilityOfSuccessRequired
                || EstimatedSalesError
                || EstimatedGMError
                || EstimatedMPCError
                || TargetedVolumeError
                || ProbOfSuccessError 
                || SponsorRequired
                || MiscellaneousDataRequired
                || PriorityRequired
                );

            if (BusinessUnitRequired)
                DataMissingLabel = "Business Unit Missing";
            else
            if (PriorityRequired)
                DataMissingLabel = "Priority not selected";
            else
                if (IndustrySegmentRequired)
                DataMissingLabel = "Industry Segment Missing";
            else
                if (ApplicationRequired)
                DataMissingLabel = "Application Missing";
            else
                if (CustomerRequired)
                DataMissingLabel = "Customer Missing";
            else
                if (AssociateRequired)
                DataMissingLabel = "Owner Missing";
            else
                if (SponsorRequired)
                DataMissingLabel = "Sponsor Missing";
            else
                if (ProjectNameRequired)
                DataMissingLabel = "Project Name Missing";
            else
                if (ReasonForIncompleteRequired)
                DataMissingLabel = "Incomplete Project Reason Missing";
            else
                if (NewBusinessCategoryRequired)
                DataMissingLabel = "Opportunity Category Missing";
            else
                if (MiscellaneousDataRequired)
                DataMissingLabel = MiscellaneousDataLabel + " Missing";            
            else
                if (InValidProduct)
                DataMissingLabel = "Error in Products List";
            else
                if (EstimatedAnnualSalesRequired)
                DataMissingLabel = "Estimated Annual Sales Missing";
            else
                if (EstimatedSalesError)
                DataMissingLabel = "Estimated Annual Sales Error";
            else
                if (UnitCostError)
                DataMissingLabel = "Unit Cost Error";
            else
                if (EstimatedAnnualGMRequired)
                DataMissingLabel = "Estimated Gross Margin Missing";
            else
                if (EstimatedGMError)
                DataMissingLabel = "Estimated Gross Margin Error";
            else
                if (EstimatedAnnualMPCRequired)
                DataMissingLabel = "Estimated MPC Missing";
            else
                if (EstimatedMPCError)
                DataMissingLabel = "Estimated MPC Error";
            else
                if (SalesVolumeRequired)
                DataMissingLabel = "Sales Volume Missing";
            else
                if (TargetedVolumeError)
                DataMissingLabel = "Targeted Volume Error";
            else
                if (ProbabilityOfSuccessRequired)
                DataMissingLabel = "Probability Of Success Missing";
            else
                if (ProbOfSuccessError)
                DataMissingLabel = "Probability Of Success Error";

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
                      
        #endregion

        #region Application Filtering

        FullyObservableCollection<ApplicationModel> applicationsmaster;
        FullyObservableCollection<ApplicationModel> applications;
        public FullyObservableCollection<ApplicationModel> Applications
        {
            get { return applications; }
            set { SetField(ref applications, value); }
        }
        
        Collection<IndustrySegmentApplicationJoinModel> industrysegsapps;

        private void GetApplicationsList()
        {
            applicationsmaster = GetApplications();
            industrysegsapps = GetIndustrySegmentApplicationJoin();
        }     

        private void FilterApplications(int mktsegid)
        {
            var subs = from p in applicationsmaster
                       join q in industrysegsapps on p.ID equals q.ApplicationID                       
                       where q.IndustrySegmentID == mktsegid
                       select new
                       {
                           p.ID,
                           p.Name                           
                       };

            FullyObservableCollection<ApplicationModel> newapplications = new FullyObservableCollection<ApplicationModel>();
            int tempid = 0;
            foreach (var ac in subs)
            {
                newapplications.Add(new ApplicationModel() { ID = ac.ID, Name = ac.Name });
                if (ac.ID == Project.ApplicationID)            
                    tempid = Project.ApplicationID;                                                
            }           

            Project.ApplicationID = 0;
            Applications = newapplications;
            Project.ApplicationID = tempid;

        }

        #endregion

        #region Industry Segments Fitering

        FullyObservableCollection<IndustrySegmentModel> industrysegmentsmaster;
        FullyObservableCollection<IndustrySegmentModel> industrysegments;

        public FullyObservableCollection<IndustrySegmentModel> IndustrySegments
        {
            get { return industrysegments; }
            set { SetField(ref industrysegments, value); }
        }
        
        private void GetIndustrySegmentsMaster()
        {
            industrysegmentsmaster = GetIndustrySegments();
        }

        private void FilterIndustrySegments(int idvalue)
        {
            var subs = industrysegmentsmaster.Where(x => x.IndustryID == idvalue);
            FullyObservableCollection<IndustrySegmentModel> newindustrysegments = new FullyObservableCollection<IndustrySegmentModel>();

            int tempid = 0;
            foreach (IndustrySegmentModel ac in subs)
            {
                newindustrysegments.Add(ac);
                if (ac.ID == Project.IndustrySegmentID)
                    tempid = Project.IndustrySegmentID;
            }
            Project.IndustrySegmentID = 0;
            
            if (tempid == 0)
            {
                Project.ApplicationID = 0;
                Applications?.Clear();
            }

            IndustrySegments = newindustrysegments;
            Project.IndustrySegmentID = tempid;
        }


        #endregion

        #region SMCode Filtering

        FullyObservableCollection<SMCodeModel> smcodes;
        FullyObservableCollection<SMCodeModel> smcodesmaster;

        public FullyObservableCollection<SMCodeModel> SMCodes
        {
            get { return smcodes; }
            set { SetField(ref smcodes, value); }
        }

        private void GetSMCodesMaster()
        {
            smcodesmaster = GetSMCodes();
        }

        private void FilterSMCodes(int indvalue)
        {                       
            FullyObservableCollection<SMCodeModel> newsmcodes = new FullyObservableCollection<SMCodeModel>();
            int tempid = 0;
            foreach (SMCodeModel ac in smcodesmaster)
            {
                if (indvalue == ac.IndustryID || ac.IndustryID == 0)
                {
                    newsmcodes.Add(ac);
                    if (ac.ID == Project.SMCodeID)                    
                       tempid = Project.SMCodeID;                    
                }
            }

            Project.SMCodeID = 0;
            SMCodes = newsmcodes;         
            Project.SMCodeID = tempid;

        }

        #endregion

        #region Miscellaneous Data Fitering

        FullyObservableCollection<MiscellaneousDataModel> miscellaneousdatamaster;
        FullyObservableCollection<MiscellaneousDataModel> miscellaneousdata;

        public FullyObservableCollection<MiscellaneousDataModel> MiscellaneousData
        {
            get { return miscellaneousdata; }
            set { SetField(ref miscellaneousdata, value); }
        }

        private void GetMiscellaneousDataMaster()
        {
            miscellaneousdatamaster = DatabaseQueries.GetMiscellaneousData();
        }

        private void FilterMiscellaneousData(int fkidvalue)
        {
            var subs = miscellaneousdatamaster.Where(x => x.FKID == fkidvalue);
            FullyObservableCollection<MiscellaneousDataModel> newmiscdata = new FullyObservableCollection<MiscellaneousDataModel>();

            int tempid = 0;
            foreach (MiscellaneousDataModel ac in subs)
            {
                newmiscdata.Add(ac);
                if (ac.ID == Project.MiscDataID)
                    tempid = Project.MiscDataID;
            }
            Project.MiscDataID = 0;

            MiscellaneousData = newmiscdata;
            Project.MiscDataID = tempid;

            MiscComboEnabled = MiscellaneousData.Count > 0;
            
        }


        #endregion

        #region Commands

        private bool CanExecuteSave(object obj)
        {
            if (InValidProduct)
                return false;

            if (InvalidField)
                return false;          

            if (!isdirty)
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
            SaveCurrentProject();            
            //close dialog
            CloseWindow();                                                                                   
        }

        private void SaveCurrentProject()
        {
            try
            {

                if (isdirty)
                {
                    UpdateProject(project);
                    isdirty = false;
                    SaveProjectFlag = true;
                }
                else                
                    SaveProjectFlag = false;                
            }
            catch (Exception e)
            {
                IMessageBoxService msgbox = new MessageBoxService();
                msgbox.ShowMessage("Unable to update project\n " + e.Message, "Error during updating project", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
                msgbox = null;
            }
        }
                      

        private bool CanExecuteAddNewMilestone(object obj)
        {
            return AddMilestoneIsEnabled;
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
                //Window owner;
                //owner = Application.Current.Windows[0];
                IMessageBoxService msgbox = new MessageBoxService();
                bool result = msgbox.MilestoneDialog(null, 0, project.ID);
                //if return value is true then Refresh list
                if (result == true)
                    Project.Milestones = GetProjectMilestones(project.ID);                            

                msgbox = null;               
            }
            catch { }            
        }

        private bool CanExecuteEditMilestone(object obj)
        {
            return (Project.ID > 0);
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
                    object[] values = new object[3];
                    values = parameter as object[];
                    int projectid = (int)values[0];
                    int mid = (int)values[1];
                    if (projectid > 0 && mid > 0)
                    {
                        IMessageBoxService msgbox = new MessageBoxService();
                        //if return value is true then Refresh list
                        if (msgbox.MilestoneDialog((Window)values[2], mid, projectid))
                            Project.Milestones = GetProjectMilestones(projectid);

                        msgbox = null;
                    }
                }
            }
            catch { }
        }

        private bool CanExecuteDeleteMilestone(object obj)
        {
            return (SelectedMilestone != null) && DeleteMilestoneIsEnabled;        
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
                    SelectedMilestone.Deleted = true;
                    UpdateMilestone(SelectedMilestone);
                    project.Milestones = GetProjectMilestones(project.ID);
                }
                msgbox = null;
            }
        }

        private bool CanExecuteAddNewEvaluationPlan(object obj)
        {
            return AddEPIsEnabled;
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
                //Window owner;
                //owner = Application.Current.Windows[0];
                IMessageBoxService msgbox = new MessageBoxService();
                bool result = msgbox.EvaluationPlanDialog(null, 0, project.ID);
                //if return value is true then Refresh list
                if (result == true)
                    Project.EvaluationPlans = GetProjectEvaluationPlans(project.ID);

                msgbox = null;
            }
            catch { }
        }

        private bool CanExecuteEditEvaluationPlan(object obj)
        {
            return (Project.ID > 0) && DeleteEPIsEnabled;
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
                    object[] values = new object[3];
                    values = parameter as object[];
                    int projectid = (int)values[0];
                    int epid = (int)values[1];

                    if (projectid > 0 && epid > 0)
                    {                        
                        IMessageBoxService msgbox = new MessageBoxService();                       
                        //if return value is true then Refresh list
                        if (msgbox.EvaluationPlanDialog((Window)values[2], epid, projectid))
                            project.EvaluationPlans = GetProjectEvaluationPlans(projectid);

                        msgbox = null;
                    }
                }
            }
            catch { }
        }

        private bool CanExecuteDeleteEvaluationPlan(object obj)
        {
            return (SelectedEvaluationPlan != null);// (Project.EvaluationPlans.Count > 0);
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
                    SelectedEvaluationPlan.Deleted = true;
                    UpdateEvaluationPlan(SelectedEvaluationPlan);
                    project.EvaluationPlans = GetProjectEvaluationPlans(project.ID);
                }
                msgbox = null;
            }
        }


        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {
                if (!InvalidField)
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveCurrentProject();                        
                        return true;
                    }
                    else
                    {
                        SaveProjectFlag = false;
                        return true;
                    }
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
                        SaveProjectFlag = false;
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

        private void ExecuteClosing(object parameter)
        {

        }

        //========================
        private bool CanExecuteWindowLoaded(object obj)
        {
            return true;
        }

        ICommand windowloaded;
        public ICommand WindowLoaded
        {
            get
            {
                if (windowloaded == null)
                    windowloaded = new DelegateCommand(CanExecuteWindowLoaded, ExecuteWindowLoaded);
                return windowloaded;
            }
        }

        private void ExecuteWindowLoaded(object parameter)
        {
            isdirty = false;
        }

        #endregion

    }
}