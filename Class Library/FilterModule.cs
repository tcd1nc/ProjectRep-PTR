using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PTR.StaticCollections;
using PTR.Models;

namespace PTR
{
    public class FilterModule : ReportsFilterModule
    {
        FullyObservableCollection<FilterListItem> projecttypes = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<FilterListItem> projectstatustypes = new FullyObservableCollection<FilterListItem>();
        FullyObservableCollection<FilterListItem> associates = new FullyObservableCollection<FilterListItem>();        
        FullyObservableCollection<FilterListItem> bus = new FullyObservableCollection<FilterListItem>();
        public ICommand AllProjectTypesCommand { get; set; }
        public ICommand AllProjectStatusTypesCommand { get; set; }
        public ICommand AllBusinessUnitsCommand { get; set; }
        public ICommand AllAssociatesCommand { get; set; }
        public ICommand ExpandAssociateButtonCommand { get; set; }

        bool allprojecttypes = false;
        bool allprojectstatuses = false;
        bool allbusinessunits = false;
        bool canExecuteAssoc = true;
        bool showallassociates = false;
        bool selectallassociates = false;
        bool canExecute = true;
       
        public FilterModule()
        {
            LoadProjectTypesFilter();
            LoadProjectStatusTypesFilter();
            LoadBusinessUnitFilter();

            AllProjectStatusTypesCommand = new RelayCommand(SelectProjectStatuses, param => this.canExecute);
            AllProjectTypesCommand = new RelayCommand(SelectProjectTypes, param => this.canExecute);
            AllBusinessUnitsCommand = new RelayCommand(SelectBusinessUnits, param => this.canExecute);        
            AllAssociatesCommand = new RelayCommand(SelectAssociates, param => this.canExecute);
            ExpandAssociateButtonCommand = new RelayCommand(ShowAssociates, param => this.canExecuteAssoc);
            //initialise filters

            InitBusinessUnits();
            BusinessUnitSrchString = string.Join(",", BusinessUnitFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
            InitProjectTypes();
            ProjectTypesSrchString = string.Join(",", ProjectTypesFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
            InitProjectStatusTypes(1);
            ProjectStatusTypesSrchString = string.Join(",", ProjectStatusTypesFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
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

        public bool ShowKPM()
        {
            bool showkpm = false;
            if (!(KPM && NonKPM))
            {
                if (KPM)                
                    showkpm = true;                
            }
            return showkpm;
        }

        public bool ShowAllKPM()
        {
            return (KPM && NonKPM);
        }

        #region Business Units

        private void InitBusinessUnits()
        {
            if (CurrentUser.BusinessUnits.Split(',').Contains("4"))
                SelectedDivisionID = 4;
            else
                SelectedDivisionID = Convert.ToInt32(CurrentUser.BusinessUnits.Split(',').FirstOrDefault());

            foreach (FilterListItem fi in BusinessUnitFilter)
                if (fi.ID == SelectedDivisionID)
                    fi.IsSelected = true;
        }          

        private void SelectBusinessUnits(object obj)
        {
            AllBusinessUnits = !AllBusinessUnits;
        }

        public bool AllBusinessUnits
        {
            get { return allbusinessunits; }
            set { SetField(ref allbusinessunits, value); }
        }

        //string salesdivisionssrchstr;
        //public string SalesDivisionsSrchString
        //{
        //    get { return salesdivisionssrchstr; }
        //    set { SetField(ref salesdivisionssrchstr, value); }
        //}
        
        public FullyObservableCollection<FilterListItem> BusinessUnitFilter
        {
            get { return bus; }
            set { SetField(ref bus, value); }
        }

        //public bool GetAllBusinessUnits
        //{
        //    get { return AllBusinessUnits; }
        //    set { SetField(ref AllBusinessUnits, value); }
        //}
                      
        private void LoadBusinessUnitFilter()
        {
            FilterListItem fi;
            foreach (ModelBaseVM ag in BusinessUnits)
            {
                fi = new FilterListItem
                {
                    ID = ag.ID,
                    Name = ag.Name,
                    IsSelected = AllBusinessUnits,
                    VisibleState = Visibility.Visible
                };
                fi.PropertyChanged += BusinessUnits_PropertyChanged;
                BusinessUnitFilter.Add(fi);
            }
        }

        void BusinessUnits_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", BusinessUnitFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    BusinessUnitSrchString = temp;
                else
                    BusinessUnitSrchString = string.Empty;
            }
        }

        string busrchstring;
        public string BusinessUnitSrchString
        {
            get { return busrchstring; }
            set { SetField(ref busrchstring, value); }
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
                    where !associat.Deleted && associat.BusinessUnits.Split(',').ToList().Contains(salesdivisionid.ToString())
                    select associat;

            if (CurrentUser.ShowOthers)
            { 
                foreach (UserModel ag in q)
                {                
                        fi = new FilterListItem();
                        {
                            fi.ID = ag.ID;
                            fi.Name = ag.Name;
                            fi.IsSelected = false;
                            fi.VisibleState = Visibility.Visible;
                            fi.PropertyChanged += FlAssociates_PropertyChanged;
                            Associates.Add(fi);
                        }                             
                }
                foreach (FilterListItem fil in associates)
                    if (fil.ID == CurrentUser.ID)
                        fil.IsSelected = true;

            }
            else
            {
                fi = new FilterListItem();
                {
                    fi.ID = CurrentUser.ID;
                    fi.Name = CurrentUser.Name;
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

        #region Project Types

        private void InitProjectTypes()
        {
            foreach (FilterListItem fi in ProjectTypesFilter)
                //if (fi.ID == selectedid)
                fi.IsSelected = true;
        }

        public FullyObservableCollection<FilterListItem> ProjectTypesFilter
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

        private void LoadProjectTypesFilter()
        {
            FilterListItem fi;
            foreach (ProjectTypeModel ag in ProjectTypes)
            {
                fi = new FilterListItem
                {
                    ID = ag.ID,
                    Name = ag.Name,
                    IsSelected = allprojecttypes,
                    VisibleState = Visibility.Visible,
                    Colour = ag.Colour
                };
                fi.PropertyChanged += FlProjectTypes_PropertyChanged;
                ProjectTypesFilter.Add(fi);
            }
        }

        void FlProjectTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", ProjectTypesFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
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
            foreach (FilterListItem fi in ProjectStatusTypesFilter)
                if (fi.ID == selectedid)
                    fi.IsSelected = true;
        }

        public FullyObservableCollection<FilterListItem> ProjectStatusTypesFilter
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

        private void LoadProjectStatusTypesFilter()
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
                ProjectStatusTypesFilter.Add(fi);
            }
        }

        private void FlProjectStatusTypes_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", ProjectStatusTypesFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                if (!string.IsNullOrEmpty(temp))
                    ProjectStatusTypesSrchString = temp;
                else
                    ProjectStatusTypesSrchString = string.Empty;
            }
        }

        #endregion

        
        #endregion

        #region Filter Commands

        ICommand clearallfilters;
        public ICommand ClearAllFilters
        {
            get
            {
                if (clearallfilters == null)
                    clearallfilters = new DelegateCommand(CanExecute, ExecuteClearAllFilters);

                return clearallfilters;
            }
        }

        private void ExecuteClearAllFilters(object parameter)                                                    
        {                                
            foreach (FilterListItem fi in CountriesFilter)
                fi.IsSelected = false;
            foreach (FilterListItem fi in BusinessUnitFilter)
                fi.IsSelected = false;
            foreach (FilterListItem fi in ProjectTypesFilter)
                fi.IsSelected = false;
            foreach (FilterListItem fi in ProjectStatusTypesFilter)
                fi.IsSelected = false;
            foreach (FilterListItem fi in Associates)
                fi.IsSelected = false;
            KPM = false;
            NonKPM = false;
            AllCountries = false;
        }
                                
        #endregion

    }
}
