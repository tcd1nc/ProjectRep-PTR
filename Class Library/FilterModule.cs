using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PTR.StaticCollections;
using PTR.Models;

namespace PTR
{
    public class FilterModule : ViewModelBase
    {
        FullyObservableCollection<FilterListItem> countries = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<FilterListItem> projecttypes = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<FilterListItem> projectstatustypes = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<FilterListItem> associates = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<RadioListItem> salesdivisionradios = new FullyObservableCollection<RadioListItem>();
        FullyObservableCollection<FilterListItem> salesdivisions = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<FilterListItem> marketsegments = new FullyObservableCollection<FilterListItem>();

        public ICommand AllCountriesCommand { get; set; }
        public ICommand AllProjectTypesCommand { get; set; }
        public ICommand AllProjectStatusTypesCommand { get; set; }
        public ICommand AllSalesDivisionsCommand { get; set; }
        public ICommand AllMarketSegmentsCommand { get; set; }
        public ICommand AllAssociatesCommand { get; set; }

        public ICommand ExpandMarketSegmentsCommand { get; set; }
        public ICommand ExpandCountriesCommand { get; set; }        
        public ICommand ExpandAssociateButtonCommand { get; set; }

        bool allcountries = true;
        bool allprojecttypes = false;
        bool allprojectstatuses = false;
        bool allsalesdivisions = false;
        bool allmarketsegments = true;
        bool showallmarketsegments = true;
        bool showallcountries = false;
        
        bool canExecuteAssoc = true;

        bool showallassociates = false;
        bool selectallassociates = false;
        
        bool canExecute = true;
        bool canopenprojectdetails = true;

        public FilterModule()
        {
            LoadCountries();
            LoadProjectTypes();
            LoadProjectStatusTypes();
            LoadSalesDivisionRadios();
            LoadSalesDivisions();
            LoadMarketSegments();

            AllProjectStatusTypesCommand = new RelayCommand(SelectProjectStatuses, param => this.canExecute);
            AllProjectTypesCommand = new RelayCommand(SelectProjectTypes, param => this.canExecute);
            AllSalesDivisionsCommand = new RelayCommand(SelectSalesDivisions, param => this.canExecute);
            AllMarketSegmentsCommand = new RelayCommand(SelectMarketSegments, param => this.canExecute);
            AllCountriesCommand = new RelayCommand(SelectCountries, param => this.canExecute);
            AllAssociatesCommand = new RelayCommand(SelectAssociates, param => this.canExecute);
            ExpandMarketSegmentsCommand = new RelayCommand(ShowMarketSegments, param => this.canExecute);
            ExpandCountriesCommand = new RelayCommand(ShowCountries, param => this.canExecute);
            ExpandAssociateButtonCommand = new RelayCommand(ShowAssociates, param => this.canExecuteAssoc);
            //initialise filters

            InitSalesDivisions();
            SalesDivisionSrchString = string.Join(",", SalesDivisions.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
            InitProjectTypes();
            ProjectTypesSrchString = string.Join(",", ProjectTypes.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
            InitProjectStatusTypes(1);
            ProjectStatusTypesSrchString = string.Join(",", ProjectStatusTypes.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());

            CountriesSrchString = string.Join(",", Countries.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
            MarketSegmentSrchString = string.Join(",", MarketSegments.Select(x => x.ID).ToList());

            InitAssociates();
            AssociatesSrchString = string.Join(",", Associates.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());

        }

        #region Filter Properties


        bool kpm = true;
        public bool KPM
        {
            get { return kpm; }
            set { SetField(ref kpm, value); }
        }

        bool nonkpm = true;
        public bool NonKPM
        {
            get { return nonkpm; }
            set { SetField(ref nonkpm, value); }
        }

        bool cdp = true;
        public bool CDP
        {
            get { return cdp; }
            set { SetField(ref cdp, value); }
        }

        bool ccp = true;
        public bool CCP
        {
            get { return ccp; }
            set { SetField(ref ccp, value); }
        }

        bool allcdpccp = true;
        public bool AllCDPCCP
        {
            get { return allcdpccp; }
            set { SetField(ref allcdpccp, value); }
        }
        
        public bool ShowKPM()
        {            
            bool showkpm = false;
            if (!(KPM && NonKPM))              
            {
                if (KPM)
                {
                    showkpm = true;
                }
            }
            return showkpm;
        }

        public bool ShowAllKPM()
        {
            return (KPM && NonKPM);                    
        }

        public string GetCDPCCPList()
        {
            string cdpccpfilter = string.Empty;
            List<string> ls = new List<string>();

            if (AllCDPCCP)
            {
                ls.Add("-1");
                ls.Add("1");
                ls.Add("2");
            }
            else
            {
                if (CDP)
                    ls.Add("1");
                if (CCP)
                    ls.Add("2");
            }

            cdpccpfilter = string.Join(",", ls);
            return cdpccpfilter;
        }


        #region SalesDivisions

        private void InitSalesDivisions()
        {
            if (CurrentUser.SalesDivisions.Split(',').Contains("4"))
                SelectedDivisionID = Convert.ToInt32(CurrentUser.SalesDivisions.Split(',').Max());
            else
                SelectedDivisionID = Convert.ToInt32(CurrentUser.SalesDivisions.Split(',').FirstOrDefault());

            foreach (FilterListItem fi in SalesDivisions)
                if (fi.ID == SelectedDivisionID)
                    fi.IsSelected = true;

            foreach (RadioListItem ri in SalesDivisionRadios)
                if (ri.ID == SelectedDivisionID)
                    ri.IsSelected = true;
        }          

        private void SelectSalesDivisions(object obj)
        {
            AllSalesDivisions = !AllSalesDivisions;
        }

        public bool AllSalesDivisions
        {
            get { return allsalesdivisions; }
            set { SetField(ref allsalesdivisions, value); }
        }

        string salesdivisionssrchstr;
        public string SalesDivisionsSrchString
        {
            get { return salesdivisionssrchstr; }
            set { SetField(ref salesdivisionssrchstr, value); }
        }
                
        string salesdivisionradiosrchstr;
        public string SalesDivisionRadioSrchString
        {
            get { return salesdivisionradiosrchstr; }
            set { SetField(ref salesdivisionradiosrchstr, value); }
        }

        public FullyObservableCollection<RadioListItem> SalesDivisionRadios
        {
            get { return salesdivisionradios; }
            set { SetField(ref salesdivisionradios, value); }
        }
        
        public FullyObservableCollection<FilterListItem> SalesDivisions
        {
            get { return salesdivisions; }
            set { SetField(ref salesdivisions, value); }
        }

        public bool GetAllSalesDivisions
        {
            get { return allsalesdivisions; }
            set
            {
                SetField(ref allsalesdivisions, value);
                SingleDivision = !value;
            }
        }
        
        bool blnsingledivision = true;
        public bool SingleDivision
        {
            get { return blnsingledivision; }
            set { SetField(ref blnsingledivision, value); }
        }

        Visibility singledivisionvisible = Visibility.Visible;
        public Visibility SingleDivisionVisibility
        {
            get { return singledivisionvisible; }
            set { SetField(ref singledivisionvisible, value); }
        }

        private void LoadSalesDivisionRadios()
        {
            RadioListItem fi;
            List<int> t = CurrentUser.SalesDivisions.Split(',').Select(int.Parse).ToList();

            foreach (GenericObjModel ag in StaticCollections.SalesDivisions)
            {
                if (t.Contains(ag.ID))
                {
                    fi = new RadioListItem()
                    {
                        ID = ag.ID,
                        Name = ag.Name,
                        GroupName = "Industries",
                        VisibleState = Visibility.Visible,
                        IsSelected = (ag.ID == SelectedDivisionID)
                    };
                    fi.PropertyChanged += FlSalesDivisionRadio_PropertyChanged;
                    SalesDivisionRadios.Add(fi);                    
                }
            }
        }

        private void FlSalesDivisionRadio_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", SalesDivisionRadios.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    SalesDivisionRadioSrchString = temp;
                else
                    SalesDivisionRadioSrchString = string.Empty;

                FilterMarketSegments();
            }
        }

        private void LoadSalesDivisions()
        {
            FilterListItem fi;
            foreach (GenericObjModel ag in StaticCollections.SalesDivisions)
            {
                fi = new FilterListItem
                {
                    ID = ag.ID,
                    Name = ag.Name,
                    IsSelected = allsalesdivisions,
                    VisibleState = Visibility.Visible
                };
                fi.PropertyChanged += FlSalesDivisions_PropertyChanged;
                SalesDivisions.Add(fi);
            }
        }

        void FlSalesDivisions_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", SalesDivisions.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    SalesDivisionSrchString = temp;
                else
                    SalesDivisionSrchString = string.Empty;

                FilterMarketSegments();
            }
        }

        string salesdivisionsrchstring;
        public string SalesDivisionSrchString
        {
            get { return salesdivisionsrchstring; }
            set { SetField(ref salesdivisionsrchstring, value); }
        }

        int selecteddivisionid;
        public int SelectedDivisionID
        {
            get { return selecteddivisionid; }
            set
            {
                SetField(ref selecteddivisionid, value);
                LoadAssociates(value);
            }
        }

        #endregion

        #region Associates

        private void InitAssociates()
        {
            LoadAssociates(SelectedDivisionID);
        }                 

        private void LoadAssociates(int salesdivisionid)
        {
            FullyObservableCollection<UserModel> associatess = DatabaseQueries.GetUsers();

            FilterListItem fi;
            Associates?.Clear();

            var q = from associat in associatess
                    where !associat.GOM.Deleted && associat.SalesDivisions.Split(',').ToList().Contains(salesdivisionid.ToString())
                    select associat;

            if (CurrentUser.ShowOthers)
            { 
                foreach (UserModel ag in q)
                {                
                        fi = new FilterListItem();
                        {
                            fi.ID = ag.GOM.ID;
                            fi.Name = ag.GOM.Name;
                            fi.IsSelected = false;
                            fi.VisibleState = Visibility.Visible;
                            fi.PropertyChanged += FlAssociates_PropertyChanged;
                            Associates.Add(fi);
                        }                             
                }
                foreach (FilterListItem fil in associates)
                    if (fil.ID == CurrentUser.GOM.ID)
                        fil.IsSelected = true;

            }
            else
            {
                fi = new FilterListItem();
                {
                    fi.ID = CurrentUser.GOM.ID;
                    fi.Name = CurrentUser.GOM.Name;
                    fi.IsSelected = true;
                    fi.VisibleState = Visibility.Visible;
                    fi.PropertyChanged += FlAssociates_PropertyChanged;
                    Associates.Add(fi);
                }
            }
                        
        }

        private void FlAssociates_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", Associates.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    AssociatesSrchString = temp;
                else
                    AssociatesSrchString = string.Empty;
            }
        }

        public FullyObservableCollection<FilterListItem> Associates
        {
            get { return associates; }
            set { SetField(ref associates, value); }
        }

        private void SelectAssociates(object obj)
        {
            AllAssociates = !AllAssociates;
        }

        public bool AllAssociates
        {
            get { return selectallassociates; }
            set { SetField(ref selectallassociates, value); }
        }

        string associatesrchstring;
        public string AssociatesSrchString
        {
            get { return associatesrchstring; }
            set { SetField(ref associatesrchstring, value); }
        }

        private void ShowAssociates(object obj)
        {
            ShowAllAssociates = !ShowAllAssociates;
        }

        public bool ShowAllAssociates
        {
            get { return showallassociates; }
            set { SetField(ref showallassociates, value); }
        }

        #endregion

        #region Countries

        public FullyObservableCollection<FilterListItem> Countries
        {
            get { return countries; }
            set { SetField(ref countries, value); }
        }

        string countriessrchstr;
        public string CountriesSrchString
        {
            get { return countriessrchstr; }
            set { SetField(ref countriessrchstr, value); }
        }

        private void SelectCountries(object obj)
        {
            AllCountries = !AllCountries;
        }

        public bool AllCountries
        {
            get { return allcountries; }
            set { SetField(ref allcountries, value); }
        }

        private void LoadCountries()
        {
            Collection<int> t = GetUserCountryAccess();
            
            FilterListItem fi;
            foreach (CountryModel ag in StaticCollections.Countries)
            {
                if (t.Contains(ag.GOM.ID))
                {
                    fi = new FilterListItem
                    {
                        ID = ag.GOM.ID,
                        Name = ag.GOM.Name,
                        IsSelected = allcountries,
                        VisibleState = Visibility.Visible
                    };
                    fi.PropertyChanged += FlCountries_PropertyChanged;
                    Countries.Add(fi);
                }
            }
        }

        private void FlCountries_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", Countries.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    CountriesSrchString = temp;
                else
                    CountriesSrchString = string.Empty;
            }
        }

        private void ShowCountries(object obj)
        {
            ShowAllCountries = !ShowAllCountries;
        }

        public bool ShowAllCountries
        {
            get { return showallcountries; }
            set { SetField(ref showallcountries, value); }
        }

        #endregion

        #region Project Types

        private void InitProjectTypes()
        {
            foreach (FilterListItem fi in ProjectTypes)
                //if (fi.ID == selectedid)
                    fi.IsSelected = true;
        }

        public FullyObservableCollection<FilterListItem> ProjectTypes
        {
            get { return projecttypes; }
            set { SetField(ref projecttypes, value); }
        }

        private void SelectProjectTypes(object obj)
        {
            AllProjectTypes = !AllProjectTypes;
        }

        public bool AllProjectTypes
        {
            get { return allprojecttypes; }
            set { SetField(ref allprojecttypes, value); }
        }

        string projecttypessrchstr;
        public string ProjectTypesSrchString
        {
            get { return projecttypessrchstr; }
            set { SetField(ref projecttypessrchstr, value); }
        }

        private void LoadProjectTypes()          
        {
            FilterListItem fi;
            foreach (GenericObjModel ag in StaticCollections.ProjectTypes)
            {
                fi = new FilterListItem
                {
                    ID = ag.ID,
                    Name = ag.Name,
                    IsSelected = allprojecttypes,
                    VisibleState = Visibility.Visible,
                    Colour = ag.Description
                };
                fi.PropertyChanged += FlProjectTypes_PropertyChanged;
                ProjectTypes.Add(fi);
            }
        }

        void FlProjectTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", ProjectTypes.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    ProjectTypesSrchString = temp;
                else
                    ProjectTypesSrchString = string.Empty;
            }
        }

        #endregion

        #region Project Status Types

        private void InitProjectStatusTypes(int selectedid)
        {
            foreach (FilterListItem fi in ProjectStatusTypes)
                if (fi.ID == selectedid)
                    fi.IsSelected = true;
        }

        public FullyObservableCollection<FilterListItem> ProjectStatusTypes
        {
            get { return projectstatustypes; }
            set { SetField(ref projectstatustypes, value); }
        }

        private void SelectProjectStatuses(object obj)
        {
            AllProjectStatuses = !AllProjectStatuses;
        }

        public bool AllProjectStatuses
        {
            get { return allprojectstatuses; }
            set { SetField(ref allprojectstatuses, value); }
        }

        string projectstatustypessrchstr;
        public string ProjectStatusTypesSrchString
        {
            get { return projectstatustypessrchstr; }
            set { SetField(ref projectstatustypessrchstr, value); }
        }
       
        private void LoadProjectStatusTypes()                
        {
            var c = EnumerationLists.ProjectStatusTypesList;
            FilterListItem fi;
            foreach (EnumValue ag in c)
            {
                fi = new FilterListItem
                {
                    ID = Convert.ToInt32(ag.Enumvalue),
                    Name = ag.Description,
                    IsSelected = allprojectstatuses,
                    VisibleState = Visibility.Visible
                };
                fi.PropertyChanged += FlProjectStatusTypes_PropertyChanged;
                ProjectStatusTypes.Add(fi);
            }
        }

        private void FlProjectStatusTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", ProjectStatusTypes.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    ProjectStatusTypesSrchString = temp;
                else
                    ProjectStatusTypesSrchString = string.Empty;
            }
        }

        #endregion

        #region Market Segments

        public FullyObservableCollection<FilterListItem> MarketSegments
        {
            get { return marketsegments; }
            set { SetField(ref marketsegments, value); }
        }

        void FlMarketSegment_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Empty;
                temp = string.Join(",", MarketSegments.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    MarketSegmentSrchString = temp;
                else
                    MarketSegmentSrchString = string.Empty;
            }
        }

        public bool ShowAllMarketSegments
        {
            get { return showallmarketsegments; }
            set { SetField(ref showallmarketsegments, value); }
        }

        private void ShowMarketSegments(object obj)
        {
            ShowAllMarketSegments = !ShowAllMarketSegments;
        }

        private void SelectMarketSegments(object obj)
        {
            AllMarketSegments = !AllMarketSegments;
        }

        public bool AllMarketSegments
        {
            get { return allmarketsegments; }
            set { SetField(ref allmarketsegments, value); }
        }

        string marketsegmentsrchstring;
        public string MarketSegmentSrchString
        {
            get { return marketsegmentsrchstring; }
            set { SetField(ref marketsegmentsrchstring, value); }
        }

        FullyObservableCollection<MarketSegmentModel> marketsegmentss;

        private void LoadMarketSegments()
        {
            marketsegmentss = StaticCollections.MarketSegments;
        }

        private void FilterMarketSegments()
        {
            FilterListItem fi;
            MarketSegments.Clear();
            var query = from q in SalesDivisions
                        where q.IsSelected == true
                        select q;
            foreach (var sd in query)
            {
                foreach (MarketSegmentModel ag in StaticCollections.MarketSegments)
                {
                    if (ag.IndustryID == sd.ID)
                    {
                        fi = new FilterListItem
                        {
                            ID = ag.GOM.ID,
                            Name = ag.GOM.Name,
                            IsSelected = true,
                            VisibleState = Visibility.Visible
                        };
                        fi.PropertyChanged += FlMarketSegment_PropertyChanged;
                        MarketSegments.Add(fi);
                    }
                }
            }
        }

        #endregion

        #endregion

        #region Filter Commands

        ICommand clearfilter;
        public ICommand ClearFilter
        {
            get
            {
                if (clearfilter == null)                
                    clearfilter = new DelegateCommand(CanExecute, ExecuteClearFilter);
                
                return clearfilter;
            }
        }

        private void ExecuteClearFilter(object parameter)                                                    
        {                                
            foreach (FilterListItem fi in Countries)
                fi.IsSelected = false;
            foreach (FilterListItem fi in SalesDivisions)
                fi.IsSelected = false;
            foreach (FilterListItem fi in ProjectTypes)
                fi.IsSelected = false;
            foreach (FilterListItem fi in ProjectStatusTypes)
                fi.IsSelected = false;
            foreach (FilterListItem fi in MarketSegments)
                fi.IsSelected = false;
            foreach (FilterListItem fi in Associates)
                fi.IsSelected = false;

            AllCountries = false;
        }

        ICommand applyfilter;
        public ICommand ApplyFilter
        {
            get
            {
                if (applyfilter == null)
                    applyfilter = new DelegateCommand(CanExecute, ExecuteApplyModuleFilter);

                return applyfilter;
            }
        }

        ICommand openproject;
        public ICommand OpenProject
        {
            get
            {
                if (openproject == null)
                    openproject = new DelegateCommand(CanExecuteOpenProject, ExecuteFMOpenProject);
                return openproject;
            }
        }
              
        public bool CanExecuteOpenProject(object parameter)
        {
            return canopenprojectdetails;
        }

        public Action<object> ExecuteApplyModuleFilter { get;  set; }
        public Action<object> ExecuteFMOpenProject { get; set; }


        #endregion

               
    }
}
