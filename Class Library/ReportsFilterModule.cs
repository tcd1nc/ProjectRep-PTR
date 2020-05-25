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
    public class ReportsFilterModule : ViewModelBase
    {
        FullyObservableCollection<FilterListItem> countriesfilter = new FullyObservableCollection<FilterListItem>();
      
        public ICommand AllCountriesCommand { get; set; }       
        public ICommand ExpandCountriesCommand { get; set; }
       
        bool allcountries = true;
        bool showallcountries = false;
                     
        bool canExecute = true;
        bool canopenprojectdetails = true;

        public ReportsFilterModule()
        {
            BusinessUnits = StaticCollections.BusinessUnits;
            LoadCountries();
            ProjectTypes = StaticCollections.ProjectTypes;
            LoadCountriesList();                      
            AllCountriesCommand = new RelayCommand(SelectCountries, param => this.canExecute);
            ExpandCountriesCommand = new RelayCommand(ShowCountries, param => this.canExecute);

            //initialise filter
            CountriesSrchString = string.Join(",", CountriesFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
                                   
        }

        #region Sales Divisions

        FullyObservableCollection<ModelBaseVM> bus;
        public FullyObservableCollection<ModelBaseVM> BusinessUnits
        {
            get { return bus; }
            set { SetField(ref bus, value); }
        }

        #endregion

        #region Countries

        public FullyObservableCollection<FilterListItem> CountriesFilter
        {
            get { return countriesfilter; }
            set { SetField(ref countriesfilter, value); }
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

        private void LoadCountriesList()
        {
            Collection<int> t = GetUserCountryAccess();

            FilterListItem fi;
            foreach (CountryModel ag in Countries)
            {
                if (t.Contains(ag.ID))
                {
                    fi = new FilterListItem
                    {
                        ID = ag.ID,
                        Name = ag.Name,
                        IsSelected = allcountries,
                        VisibleState = Visibility.Visible
                    };
                    fi.PropertyChanged += FlCountries_PropertyChanged;
                    CountriesFilter.Add(fi);
                }
            }
        }

        FullyObservableCollection<CountryModel> countries;
        public FullyObservableCollection<CountryModel> Countries
        {
            get {return countries; }
            set { SetField(ref countries, value); }
        }

        private void LoadCountries()
        {
            Countries = DatabaseQueries.GetCountries();
        }

        private void FlCountries_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsSelected")
            {
                string temp = string.Join(",", CountriesFilter.Where(t => t.IsSelected == true).Select(x => x.ID).ToList());
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

        #region ProjectTypes

        FullyObservableCollection<ProjectTypeModel> projecttypes;
        public FullyObservableCollection<ProjectTypeModel> ProjectTypes
        {
            get { return projecttypes; }
            set { SetField(ref projecttypes, value); }
        }

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
            foreach (FilterListItem fi in CountriesFilter)
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

        ICommand applyfilterpopup;
        public ICommand ApplyFilterPopup
        {
            get
            {
                if (applyfilterpopup == null)
                    applyfilterpopup = new DelegateCommand(CanExecute, ExecuteApplyPopupFilter);
                return applyfilterpopup;
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


        ICommand exporttoexcel;
        public ICommand ExportToExcel
        {
            get
            {
                if (exporttoexcel == null)
                    exporttoexcel = new DelegateCommand(CanExecuteExportToExcel, ExecuteRFExportToExcel);
                return exporttoexcel;
            }
        }

       // bool canexporttoexcel;
        public bool CanExecuteExportToExcel(object parameter)
        {
            return true;// canexporttoexcel;
        }

        ICommand clearfilterpopup;
        public ICommand ClearFilterPopup
        {
            get
            {
                if (clearfilterpopup == null)
                    clearfilterpopup = new DelegateCommand(CanExecute, ExecuteClearPopupFilter);
                return clearfilterpopup;
            }
        }

        ICommand resetfilterpopup;
        public ICommand ResetFilterPopup
        {
            get
            {
                if (resetfilterpopup == null)
                    resetfilterpopup = new DelegateCommand(CanExecute, ExecuteResetPopupFilter);
                return resetfilterpopup;
            }
        }

        ICommand clearfilters;
        public ICommand ClearFilters
        {
            get
            {
                if (clearfilters == null)
                    clearfilters = new DelegateCommand(CanExecuteClearFilters, ExecuteClearFilters);
                return clearfilters;
            }
        }

        private bool CanExecuteClearFilters(object parameter)
        {
            return true;
        }

        public void SetPopupFilters(string[] excludedcols, DataTable dt)
        {
            try
            {
                foreach (string colname in excludedcols)
                {
                    if (!DictFilterPopup.ContainsKey(colname))
                        DictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = dt.Columns[colname].Caption, IsApplied = false });

                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(colname, out s);
                    
                    foreach (DataRow dr in dt.Rows)
                        if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });
                    s.IsApplied = false;
                }
            }
            catch { }
        }


        public Action<object> ExecuteApplyModuleFilter { get; set; }
        public Action<object> ExecuteClearFilters { get; set; }
        public Action<object> ExecuteApplyPopupFilter { get; set; }
        public Action<object> ExecuteClearPopupFilter { get; set; }        
        public Action<object> ExecuteResetPopupFilter { get; set; }

        public Action<object> ExecuteFMOpenProject { get; set; }
        public Action<object> ExecuteRFExportToExcel { get; set; }

        //------------------ Popup Filter -------------------------------------
        Dictionary<string, FilterPopupModel> dictFilterPopup = new Dictionary<string, FilterPopupModel>();
        public Dictionary<string, FilterPopupModel> DictFilterPopup
        {
            get { return dictFilterPopup; }
            set { SetField(ref dictFilterPopup, value); }
        }

        #endregion

    }
}
