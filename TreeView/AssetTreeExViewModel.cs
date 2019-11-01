using System;
using System.Windows.Input;
using System.Windows;
using System.Data;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
//using static AssetManager.DatabaseQueries;
//using static AssetManager.GlobalClass;

namespace PTR.TVViewModels
{
    public class AssetTreeExViewModel : ViewModelBase
    {
        #region Declarations
        //Command declarations ============================================================
        ICommand _clearfilter;      //Clear filter
        ICommand _closeall;         //Close all child windows and application

        Window _owner = Application.Current.Windows[0];
        FullyObservableCollection<TVCustomerViewModel> _tvcustomers = new FullyObservableCollection<TVCustomerViewModel>();
        public FullyObservableCollection<TVCustomerViewModel> _allcustomers = new FullyObservableCollection<TVCustomerViewModel>();
        FullyObservableCollection<Models.SearchFieldModel> _searchfields = new FullyObservableCollection<Models.SearchFieldModel>();
        Collection<MenuItem> _menuitems = new Collection<MenuItem>();
        FullyObservableCollection<Models.AssetAreaModel> _assetareas;
        FullyObservableCollection<Models.AssetGroupModel> _assetgroups;
        FullyObservableCollection<Models.AssetTypeModel> _assettypes;
        Collection<EnumValue> _assetstatuses = new Collection<EnumValue>();
        FullyObservableCollection<Models.CustomerModel> _customers;
        FullyObservableCollection<Models.CountryModel> _countries;
        #endregion Declarations

        #region Constructors

        public AssetTreeExViewModel()
        {
            IsEnabled = IsAdministrator;
            _assetstatuses = EnumerationLists.AssetStatusTypesList;

            _assetareas = GetAssetAreas();
            _assetgroups = GetAssetGroups();
            _assettypes = GetAssetTypes();
            _countries = GetCountries();

            BuildSearchFields();
            GetAllCustomerVMs();

            Customers = _allcustomers;

            SearchTextboxVisibility = Visibility.Visible;
            SearchComboVisibility = Visibility.Collapsed;
            SearchLabelVisibility = Visibility.Collapsed;
            SearchDateRangeVisibility = Visibility.Collapsed;
            SearchForVisibility = Visibility.Visible;
            SearchHitsVisibility = Visibility.Collapsed;
            CreateReportMenu();
            LabelMask = GlobalClass.LabelMask;

            
        }

        #endregion Constructors

        bool _isenabled;
        public bool IsEnabled
        {
            get { return _isenabled; }
            set { SetField(ref _isenabled, value); }
        }

        public FullyObservableCollection<TVCustomerViewModel> Customers
        {
            get { return _tvcustomers; }
            set { SetField(ref _tvcustomers, value); }
        }


        #region Search Properties

        Models.SearchFieldModel _searchselectedstem;
        public Models.SearchFieldModel SearchSelectedItem
        {
            get { return _searchselectedstem; }
            set
            {

                if (value != null)
                {
                    if (value.SearchFieldType == "Text")
                    {
                        SearchTextboxVisibility = Visibility.Visible;
                        SearchComboVisibility = Visibility.Collapsed;
                        SearchLabelVisibility = Visibility.Collapsed;
                        SearchDateRangeVisibility = Visibility.Collapsed;
                        SearchForVisibility = Visibility.Visible;
                    }
                    else
                    if (value.SearchFieldType == "ID")
                    {
                        SearchTextboxVisibility = Visibility.Collapsed;
                        SearchComboVisibility = Visibility.Visible;
                        SearchLabelVisibility = Visibility.Collapsed;
                        SearchDateRangeVisibility = Visibility.Collapsed;
                        SearchForVisibility = Visibility.Visible;
                        PopulateSearchCombo(value);
                    }
                    else
                    if (value.SearchFieldType == "Label")
                    {
                        SearchTextboxVisibility = Visibility.Collapsed;
                        SearchComboVisibility = Visibility.Collapsed;
                        SearchLabelVisibility = Visibility.Visible;
                        SearchDateRangeVisibility = Visibility.Collapsed;
                        SearchForVisibility = Visibility.Visible;
                    }
                    else
                    if (value.SearchFieldType == "Date")
                    {
                        SearchTextboxVisibility = Visibility.Collapsed;
                        SearchComboVisibility = Visibility.Collapsed;
                        SearchLabelVisibility = Visibility.Collapsed;
                        SearchDateRangeVisibility = Visibility.Visible;
                        SearchForVisibility = Visibility.Collapsed;
                    }
                    else
                    if (value.SearchFieldType == "Boolean")
                    {
                        SearchTextboxVisibility = Visibility.Collapsed;
                        SearchComboVisibility = Visibility.Collapsed;
                        SearchLabelVisibility = Visibility.Collapsed;
                        SearchDateRangeVisibility = Visibility.Collapsed;
                        SearchForVisibility = Visibility.Collapsed;
                    }
                }
                SetField(ref _searchselectedstem, value);
            }
        }


        public FullyObservableCollection<Models.SearchFieldModel> SearchFields
        {
            get { return _searchfields; }
            set { _searchfields = value; }
        }

        Visibility _searchtextboxvisibility;
        public Visibility SearchTextboxVisibility
        {
            get { return _searchtextboxvisibility; }
            set { SetField(ref _searchtextboxvisibility, value); }
        }

        Visibility _searchcombovisibility;
        public Visibility SearchComboVisibility
        {
            get { return _searchcombovisibility; }
            set { SetField(ref _searchcombovisibility, value); }
        }

        Visibility _labelsearchvisibility;
        public Visibility SearchLabelVisibility
        {
            get { return _labelsearchvisibility; }
            set { SetField(ref _labelsearchvisibility, value); }
        }

        Visibility _daterangesearchvisibility;
        public Visibility SearchDateRangeVisibility
        {
            get { return _daterangesearchvisibility; }
            set { SetField(ref _daterangesearchvisibility, value); }
        }


        Visibility _searchforvisibility;
        public Visibility SearchForVisibility
        {
            get { return _searchforvisibility; }
            set { SetField(ref _searchforvisibility, value); }
        }

        Visibility _searchhitsvisibility;
        public Visibility SearchHitsVisibility
        {
            get { return _searchhitsvisibility; }
            set { SetField(ref _searchhitsvisibility, value); }
        }

        FullyObservableCollection<Models.SearchComboModel> _searchcombo = new FullyObservableCollection<Models.SearchComboModel>();
        public FullyObservableCollection<Models.SearchComboModel> SearchCombo
        {
            get { return _searchcombo; }
            set { SetField(ref _searchcombo, value); }
        }

        string _searchtext;
        public string SearchText
        {
            get { return _searchtext; }
            set { SetField(ref _searchtext, value); }
        }

        int _searchcombovalue;
        public int SearchComboValue
        {
            get { return _searchcombovalue; }
            set { SetField(ref _searchcombovalue, value); }
        }

        DateTime? _startdate;
        public DateTime? StartDate
        {
            get { return _startdate; }
            set { SetField(ref _startdate, value); }
        }
        DateTime? _enddate;
        public DateTime? EndDate
        {
            get { return _enddate; }
            set { SetField(ref _enddate, value); }
        }

        int _searchhits;
        public int SearchHits
        {
            get { return _searchhits; }
            set { SetField(ref _searchhits, value); }
        }

        string _labelmask;
        public string LabelMask
        {
            get { return _labelmask; }
            set { SetField(ref _labelmask, value); }
        }

        string _searchlabeltext;
        public string SearchLabelText
        {
            get { return _searchlabeltext; }
            set { SetField(ref _searchlabeltext, value); }
        }

        int _searchfieldindex;
        public int SearchFieldIndex
        {
            get { return _searchfieldindex; }
            set { SetField(ref _searchfieldindex, value); }
        }

        private void BuildSearchFields()
        {
            SearchFields = GetSearchFields();

            Models.SearchFieldModel _srchfield;
            FullyObservableCollection<Models.AssetSpecificationModel> _specs = GetSpecifications();
            foreach (Models.AssetSpecificationModel aspec in _specs)
            {
                _srchfield = new Models.SearchFieldModel();
                _srchfield.ID = aspec.ID * -1;
                _srchfield.Label = aspec.SpecificationName;
                _srchfield.SearchField = aspec.SpecificationName;
                if (aspec.MeasurementUnitID == (int)MeasurementUnits.Boolean)  //== 4
                    _srchfield.SearchFieldType = "Boolean";
                else
                    _srchfield.SearchFieldType = "Text";
                _srchfield.QueryName = "SearchSpecifications";
                SearchFields.Add(_srchfield);
            }

        }

        private void PopulateSearchCombo(Models.SearchFieldModel _searchitem)
        {
            Models.SearchComboModel _searchcombo;
            SearchCombo.Clear();
            switch (_searchitem.SearchField)
            {

                case "CustomerID":
                    foreach (Models.CustomerModel cm in _customers)
                    {
                        _searchcombo = new Models.SearchComboModel();
                        _searchcombo.ID = cm.ID;
                        _searchcombo.Description = cm.CustomerName;
                        SearchCombo.Add(_searchcombo);
                    }
                    break;

                case "Status":
                    foreach (EnumValue cm in _assetstatuses)
                    {
                        _searchcombo = new Models.SearchComboModel();
                        _searchcombo.ID = cm.ID;
                        _searchcombo.Description = cm.Description;
                        SearchCombo.Add(_searchcombo);
                    }
                    break;

                case "TypeID":
                    foreach (Models.AssetTypeModel cm in _assettypes)
                    {
                        _searchcombo = new Models.SearchComboModel();
                        _searchcombo.ID = cm.ID;
                        _searchcombo.Description = cm.Description;
                        SearchCombo.Add(_searchcombo);
                    }
                    break;
                case "GroupID":
                    foreach (Models.AssetGroupModel cm in _assetgroups)
                    {
                        _searchcombo = new Models.SearchComboModel();
                        _searchcombo.ID = cm.ID;
                        _searchcombo.Description = cm.Description;
                        SearchCombo.Add(_searchcombo);
                    }
                    break;
                case "AreaID":
                    foreach (Models.AssetAreaModel cm in _assetareas)
                    {
                        _searchcombo = new Models.SearchComboModel();
                        _searchcombo.ID = cm.ID;
                        _searchcombo.Description = cm.Description;
                        SearchCombo.Add(_searchcombo);
                    }
                    break;
                case "CountryID":
                    foreach (Models.CountryModel cm in _countries)
                    {
                        _searchcombo = new Models.SearchComboModel();
                        _searchcombo.ID = cm.ID;
                        _searchcombo.Description = cm.CountryName;
                        SearchCombo.Add(_searchcombo);
                    }
                    break;
            }
        }


        #endregion

        #region Report Menu

        public Collection<MenuItem> MenuItems
        {
            get { return _menuitems; }
            set { SetField(ref _menuitems, value); }
        }

        private void CreateReportMenu()
        {
            MenuItem _menuitem;
            Collection<Models.ReportModel> _reports = GetReports();// GlobalClass.Reports;
            Image img = new Image();

            foreach (Models.ReportModel rm in _reports)
            {
                _menuitem = new MenuItem();
                _menuitem.Header = rm.Header;

                if (rm.HasDateFilter)
                    if (rm.Parameter == "GetMovementsReport")
                        _menuitem.Command = OpenMovementsReportDateFilter;
                    else
                        _menuitem.Command = OpenReportDateFilter;
                else
                    _menuitem.Command = OpenReport;
                _menuitem.CommandParameter = rm;
                _menuitem.ToolTip = rm.Tooltip;
                img = new Image();
                img.Source = new BitmapImage(new Uri("Images/" + rm.IconfileName, UriKind.Relative));
                _menuitem.Icon = img;
                MenuItems.Add(_menuitem);
            }
        }
        #endregion

        #region Commands


        ICommand _customerdlg;

        public ICommand OpenCustomerDialog
        {
            get
            {
                if (_customerdlg == null)
                    _customerdlg = new DelegateCommand(CanExecute, ExecOpenCustomerDialog);
                return _customerdlg;
            }
        }

        private void ExecOpenCustomerDialog(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            if (_msgboxcommand.OpenCustomersDlg() == true)
            {
                GetAllCustomerVMs();
            }

            _msgboxcommand = null;
        }


        ICommand _searchdata;
        public ICommand SearchData
        {
            get
            {
                if (_searchdata == null)
                    _searchdata = new DelegateCommand(CanExecute, SearchDatabase);
                return _searchdata;
            }
        }

        Dictionary<int, int> _assets = new Dictionary<int, int>();

        /// <summary>
        /// Search database according to SearchFieldType
        /// </summary>
        /// <param name="parameter"></param>
        private void SearchDatabase(object parameter)
        {
            if (SearchSelectedItem != null)
            {

                switch (SearchSelectedItem.SearchFieldType)
                {
                    case "ID":
                        if (SearchSelectedItem.SearchField == "CustomerID")
                        {
                            Customerfilter_SelectionChanged(SearchComboValue);
                            SearchHitsVisibility = Visibility.Collapsed;
                        }
                        else
                        {
                            _assets = GetAssetSearch(SearchSelectedItem.QueryName, SearchComboValue);
                            SearchHits = _assets.Count;
                            SearchHitsVisibility = Visibility.Visible;
                        }
                        break;

                    case "Text":
                        if (!string.IsNullOrEmpty(SearchText))
                        {
                            if (SearchSelectedItem.ID > 0)
                                _assets = GetAssetSearch(SearchSelectedItem.QueryName, SearchText);
                            else
                                _assets = GetAssetSearch(SearchSelectedItem.QueryName, SearchText, SearchSelectedItem.ID * -1);

                            SearchHits = _assets.Count;
                            SearchHitsVisibility = Visibility.Visible;
                        }
                        break;

                    case "Label":
                        _assets = GetAssetSearch(SearchSelectedItem.QueryName, SearchLabelText);

                        SearchHits = _assets.Count;
                        SearchHitsVisibility = Visibility.Visible;

                        break;

                    case "Date":
                        if (StartDate == null)
                            StartDate = DateTime.Now;
                        if (EndDate == null)
                            EndDate = DateTime.Now;
                        _assets = GetAssetSearch(SearchSelectedItem.QueryName, StartDate, EndDate);

                        SearchHits = _assets.Count;
                        SearchHitsVisibility = Visibility.Visible;

                        break;

                    case "Boolean":
                        if (SearchSelectedItem.ID > 0)
                            _assets = GetAssetSearch(SearchSelectedItem.QueryName, "True");
                        else
                            _assets = GetAssetSearch(SearchSelectedItem.QueryName, "True", SearchSelectedItem.ID * -1);

                        SearchHits = _assets.Count;
                        SearchHitsVisibility = Visibility.Visible;

                        break;

                }

                foreach (TVCustomerViewModel cm in Customers)
                {
                    foreach (TVAssetViewModel am in cm.Children)
                    {
                        // if (_assets.Count(x => x == am.Asset.AssetID) == 1)
                        //if (_assets.Contains(am.Asset.AssetID))
                        if (_assets.ContainsKey(am.Asset.AssetID))
                        {
                            am.IsFiltered = true;
                            am.IsExpanded = true;
                            cm.IsExpanded = true;
                            //to scroll into view
                            cm.IsSelected = true;

                            //optimise search by removing element when found
                            //       var id = _assets.Where(x => x == am.Asset.AssetID).FirstOrDefault();
                            //     _assets.Remove(id);
                            _assets.Remove(am.Asset.AssetID);

                        }

                        RecursivelyGetFilteredAsset2(am.Children, cm);
                        //   GetFilteredAsset(am.Children, cm);
                    }
                }

            }
        }

        private void RecursivelyGetFilteredAsset2(ObservableCollection<TreeViewItemViewModel> theseNodes, TVCustomerViewModel cm)
        {
            foreach (TVAssetViewModel am in theseNodes)
            {
                // if(_assets.Contains(am.Asset.AssetID))
                if (_assets.ContainsKey(am.Asset.AssetID))
                {
                    if (am.Parent != null)
                    {
                        am.Parent.IsExpanded = true;
                    }
                    am.IsFiltered = true;
                    am.IsExpanded = true;
                    cm.IsExpanded = true;
                    //optimise search by removing element when found
                    //    var id = _assets.Where(x => x == am.Asset.AssetID).FirstOrDefault();
                    //  _assets.Remove(id);

                    _assets.Remove(am.Asset.AssetID);
                }
                RecursivelyGetFilteredAsset2(am.Children, cm);
            }
        }

        /// <summary>
        /// Non-recursive version
        /// </summary>
        /// <param name="theseNodes"></param>
        /// <param name="cm"></param>
        public void GetFilteredAsset(ObservableCollection<TreeViewItemViewModel> theseNodes, TVCustomerViewModel cm)
        {
            var stack = new Stack<TVAssetViewModel>();
            foreach (TVAssetViewModel tv in theseNodes)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                //  if (_assets.Contains(current.Asset.AssetID))
                if (_assets.ContainsKey(current.Asset.AssetID))
                {
                    if (current.Parent != null)
                    {
                        current.Parent.IsExpanded = true;
                    }
                    current.IsFiltered = true;
                    current.IsExpanded = true;
                    cm.IsExpanded = true;
                    //optimise search by removing element when found
                    //          var id = _assets.Where(x => x == current.Asset.AssetID).FirstOrDefault();
                    //        _assets.Remove(id);

                    _assets.Remove(current.Asset.AssetID);
                }
                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
        }


        /// <summary>
        /// Consider non-recursive approach...
        /// </summary>
        private void ClearSearchFilter()
        {
            SearchText = string.Empty;
            SearchComboValue = -1;
            SearchLabelText = string.Empty;
            SearchFieldIndex = -1;
            SearchHits = 0;
            SearchHitsVisibility = Visibility.Collapsed;
            SearchTextboxVisibility = Visibility.Visible;
            SearchComboVisibility = Visibility.Collapsed;
            SearchLabelVisibility = Visibility.Collapsed;
            SearchDateRangeVisibility = Visibility.Collapsed;
            SearchForVisibility = Visibility.Visible;
            GetAllCustomerVMs();

        }

        /// <summary>
        /// Consider non-recursive approach...
        /// </summary>
        /// <param name="theseNodes"></param>
        private void RecursivelyResetFilteredAsset(ObservableCollection<TreeViewItemViewModel> theseNodes)
        {
            foreach (TVAssetViewModel am in theseNodes)
            {
                am.IsFiltered = false;
                am.IsExpanded = false;
                if (am.Parent != null)
                    am.Parent.IsExpanded = false;

                RecursivelyResetFilteredAsset(am.Children);
            }
        }

        /// <summary>
        /// Non-recursive option
        /// </summary>
        /// <param name="theseNodes"></param>
        public void ResetFilteredAssets(ObservableCollection<TreeViewItemViewModel> theseNodes)
        {
            var stack = new Stack<TVAssetViewModel>();
            foreach (TVAssetViewModel tv in theseNodes)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                current.IsFiltered = false;
                current.IsExpanded = false;
                if (current.Parent != null)
                    current.Parent.IsExpanded = false;
                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
        }

        ICommand _openreport;
        public ICommand OpenReport
        {
            get
            {
                if (_openreport == null)
                    _openreport = new DelegateCommand(CanExecute, OpenReportWindow);
                return _openreport;
            }
        }

        private void OpenReportWindow(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            bool result = _msgboxcommand.OpenReportDialog((Models.ReportModel)parameter);
            _msgboxcommand = null;
        }


        //OpenReportDateFilter
        ICommand _openreportdatefilter;
        public ICommand OpenReportDateFilter
        {
            get
            {
                if (_openreportdatefilter == null)
                    _openreportdatefilter = new DelegateCommand(CanExecute, OpenReportDateFilterWindow);
                return _openreportdatefilter;
            }
        }

        private void OpenReportDateFilterWindow(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            bool result = _msgboxcommand.OpenReportDateFilterDialog((Models.ReportModel)parameter);
            _msgboxcommand = null;
        }

        //OpenMovementsReportDateFilter
        ICommand _openmovementsreportdatefilter;
        public ICommand OpenMovementsReportDateFilter
        {
            get
            {
                if (_openmovementsreportdatefilter == null)
                    _openmovementsreportdatefilter = new DelegateCommand(CanExecute, OpenMovementsReportDateFilterWindow);
                return _openmovementsreportdatefilter;
            }
        }

        private void OpenMovementsReportDateFilterWindow(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            bool result = _msgboxcommand.OpenMovementsReportDateFilterDialog((Models.ReportModel)parameter);
            _msgboxcommand = null;
        }



        ICommand _newdialog;
        /// <summary>
        /// 
        /// </summary>
        public ICommand OpenDialog
        {
            get
            {
                if (_newdialog == null)
                    _newdialog = new DelegateCommand(CanExecute, OpenSelectedDialog);
                return _newdialog;
            }
        }

        private void OpenSelectedDialog(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            _msgboxcommand.OpenDialog((string)parameter);
            _msgboxcommand = null;
        }

        ICommand _doubleclick;
        public ICommand DoubleClickCommand
        {
            get
            {
                if (_doubleclick == null)
                    _doubleclick = new DelegateCommand(CanExecute, ExecuteDoubleClick);

                return _doubleclick;
            }
        }

        private void ExecuteDoubleClick(object obj)
        {
            if (obj.GetType().Equals(typeof(TVCustomerViewModel)))
                (obj as TVCustomerViewModel).IsExpanded = !(obj as TVCustomerViewModel).IsExpanded;

            if (!obj.GetType().Equals(typeof(TVAssetViewModel)))
                return;
            if (!(obj as TVAssetViewModel).IsSelected)
                return;

            int ID = (obj as TVAssetViewModel).Asset.AssetID;

            if (!AssetTreeExViewModel.OpenAssetWindow(ID))
            {
                IMessageBoxService _msgboxcommand = new MessageBoxService();
                Models.AssetModel result = _msgboxcommand.OpenAssetDlg((obj as TVAssetViewModel).Asset);
                _msgboxcommand = null;
            }
        }

        //Command to call delegate to clear filter
        public ICommand ClearFilter
        {
            get
            {
                if (_clearfilter == null)
                    _clearfilter = new DelegateCommand(CanExecute, ExecuteClearFilter);
                return _clearfilter;
            }
        }
        private void ExecuteClearFilter(object parameter)
        {
            ClearSearchFilter();
        }

        //Command to call delegate to Close application
        public ICommand CloseAll
        {
            get
            {
                if (_closeall == null)
                    _closeall = new DelegateCommand(CanExecute, ExecuteCloseAll);
                return _closeall;
            }
        }

        /// <summary>
        /// Close all child windows before closing program
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteCloseAll(object parameter)
        {
            foreach (Window w in _owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    if (((Views.AssetView)w).Tag != null)
                    {
                        if ((int)((Views.AssetView)w).Tag > 0)
                        {
                            if (IsEnabled)
                                ((ViewModels.AssetViewModel)((Views.AssetView)w).DataContext).SaveAll();
                            // w.Close();
                        }
                        //else
                        //  w.Close();
                    }
                }
                w.Close();
            }
           
            Application.Current.Shutdown();
        }

        ICommand _addnewasset;
        /// <summary>
        /// Add new Asset from menu
        /// The new asset will be added as a child of the selected Customer
        /// </summary>
        public ICommand AddNewAsset
        {
            get
            {
                if (_addnewasset == null)
                    _addnewasset = new DelegateCommand(CanExecute, ExecuteAddNewAsset);
                return _addnewasset;
            }
        }

        private void ExecuteAddNewAsset(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            Models.AssetModel result = _msgboxcommand.OpenAssetDlg(false, 0, 0);
            if (result != null)
            {
                ViewModels.AssetTreeExViewModel.AddAssetNode(result);
            }
            _msgboxcommand = null;
        }

        #endregion Commands


        #region Context Menu Commands

        ICommand _addnewassetcm;
        public ICommand AddNewAssetCM
        {
            get
            {
                if (_addnewassetcm == null)
                    _addnewassetcm = new DelegateCommand(CanExecute, ExecuteAddNewAssetCM);
                return _addnewassetcm;
            }
        }

        private void ExecuteAddNewAssetCM(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                TVAssetViewModel _asset = parameter as TVAssetViewModel;
                Models.AssetModel result = _msgboxcommand.OpenAssetDlg(true, _asset.Asset.CustomerID, _asset.Asset.AssetID);
                if (result != null)
                {
                    AddAssetNode(result);
                }
            }
            else
            if (parameter.GetType().Equals(typeof(TVCustomerViewModel)))
            {
                TVCustomerViewModel _customer = parameter as TVCustomerViewModel;
                Models.AssetModel result = _msgboxcommand.OpenAssetDlg(true, _customer.Customer.ID, 0);
                if (result != null)
                {
                    AddAssetNode(result);
                }
            }
            _msgboxcommand = null;
        }

        ICommand _deletecustomer;
        public ICommand DeleteCustomer
        {
            get
            {
                if (_deletecustomer == null)
                    _deletecustomer = new DelegateCommand(CanExecute, ExecuteDeleteCustomer);
                return _deletecustomer;
            }
        }

        private void ExecuteDeleteCustomer(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVCustomerViewModel)))
            {
                TVCustomerViewModel _customer = parameter as TVCustomerViewModel;
                IMessageBoxService _msgbox = new MessageBoxService();
                if (_customer.Children.Count == 0)
                {
                    if (_msgbox.ShowMessage("Do you want to delete " + _customer.Customer.CustomerName, "Delete Customer: " + _customer.Customer.CustomerName, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question) == GenericMessageBoxResult.OK)
                    {
                        Customers.Remove(_customer);
                        DeleteItem(_customer.Customer.ID, DeleteSPName.DeleteCustomer);
                        
                    }
                }
                else
                    _msgbox.ShowMessage(_customer.Customer.CustomerName + " has Assets and cannot be deleted" + "\n\nDelete the Assets first and then delete the customer.",
                        "Unable to Delete Customer: " + _customer.Customer.CustomerName, GenericMessageBoxButton.OK, GenericMessageBoxIcon.Information);
                _msgbox = null;
            }
        }

        private bool CanExecuteDeleteAsset(object obj)
        {
            return true;
        }

        ICommand _deleteasset;
        public ICommand DeleteAsset
        {
            get
            {
                if (_deleteasset == null)
                    _deleteasset = new DelegateCommand(CanExecuteDeleteAsset, ExecuteDeleteAsset);
                return _deleteasset;
            }
        }

        private void ExecuteDeleteAsset(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                TVAssetViewModel _asset = parameter as TVAssetViewModel;
                IMessageBoxService _msgbox = new MessageBoxService();
                if (_msgbox.ShowMessage("Are you sure you want to delete this Asset: " + _asset.Asset.Label + " and any associated child Assets?", "Removing Asset: " + _asset.Asset.Label, GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.Yes))
                {
                    //change to delete all children too and set parentid to 0 .
                    ViewModels.AssetTreeExViewModel.DeleteAssetDFS(_asset);
                }
                _msgbox = null;
            }
        }


        private bool CanExecuteMoveAsset(object obj)
        {
            return true;
        }

        ICommand _moveasset;
        public ICommand MoveSelectedAsset
        {
            get
            {
                if (_moveasset == null)
                    _moveasset = new DelegateCommand(CanExecuteMoveAsset, ExecuteMoveAsset);
                return _moveasset;
            }
        }

        TVAssetViewModel _movingasset;
        string _movingassetlabel;
        public string MovingAssetLabel
        {
            get { return _movingassetlabel; }
            set { SetField(ref _movingassetlabel, value); }
        }

        private void ExecuteMoveAsset(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                TVAssetViewModel _asset = parameter as TVAssetViewModel;
                if (_asset != null)
                {
                    _movingasset = _asset;
                    _asset.IsSelected = true;
                    _movingassetlabel = _asset.Asset.Label;
                }
            }
        }

        private bool CanExecutePasteAsset(object obj)
        {
            if (_movingasset == null)
                return false;
            return true;
        }

        ICommand _pasteasset;
        public ICommand PasteSelectedAsset
        {
            get
            {
                if (_pasteasset == null)
                    _pasteasset = new DelegateCommand(CanExecutePasteAsset, ExecutePasteAsset);
                return _pasteasset;
            }
        }

        private void ExecutePasteAsset(object parameter)
        {
            if (parameter.GetType().Equals(typeof(TVAssetViewModel)))
            {
                TVAssetViewModel _targetasset = parameter as TVAssetViewModel;

                if (_targetasset != null
                    && !(_targetasset.Asset.CustomerID == _movingasset.Asset.CustomerID && _targetasset.Asset.AssetID == _movingasset.Asset.ParentAssetID))
                {
                    DatabaseQueries.UpdateParentAssetID(_movingasset.Asset.AssetID, _targetasset.Asset.AssetID, _targetasset.Asset.CustomerID);
                    ViewModels.AssetTreeExViewModel.MoveAsset(_movingasset.Asset.AssetID, _targetasset.Asset.AssetID, _targetasset.Asset.CustomerID);
                    _movingasset = null;
                    MovingAssetLabel = string.Empty;
                }
            }
            else
            if (parameter.GetType().Equals(typeof(TVCustomerViewModel)))
            {
                TVCustomerViewModel _targetcustomer = parameter as TVCustomerViewModel;
                if (_targetcustomer != null
                    && !(_targetcustomer.Customer.ID == _movingasset.Asset.CustomerID && _movingasset.Asset.ParentAssetID == 0))
                {
                    DatabaseQueries.UpdateParentAssetID(_movingasset.Asset.AssetID, 0, _targetcustomer.Customer.ID);
                    ViewModels.AssetTreeExViewModel.MoveAsset(_movingasset.Asset.AssetID, 0, _targetcustomer.Customer.ID);
                    _movingasset = null;
                    MovingAssetLabel = string.Empty;
                }
            }
        }

        #endregion


        #region Tree Data Properties

        /// <summary>
        /// Filter customers
        /// </summary>
        /// <param name="_customerID"></param>
        private void GetCustomers(int _customerID)                  //Proc that starts process of building data structure for treeview
        {                                                          //First stage is to build parent nodes which are the Customer nodes                   
            TVCustomerViewModel _customer;
            Customers.Clear();
            if (_customerID == 0)
                Customers = _allcustomers;
            else
            {
                Models.CustomerModel q = (from a in _customers
                                          where a.ID == _customerID
                                          select a).FirstOrDefault();

                _customer = new TVCustomerViewModel(q);
                Customers.Add(_customer);
            }
        }


        #endregion Tree Data

        /// <summary>
        /// Populate main Customer VM collection
        /// TBD...needs to be re-populated when new customers added
        /// 
        /// </summary>
        public void GetAllCustomerVMs()
        {
            _customers = DatabaseQueries.GetCustomers();
            TVCustomerViewModel _cust;
            _allcustomers.Clear();
            foreach (Models.CustomerModel cm in _customers)
            {
                //add customer to tree
                _cust = new TVCustomerViewModel(cm);
                _allcustomers.Add(_cust);
            }
           
        }

        public void RemoveCustomerVM(int _customerid)
        {
            foreach (TVCustomerViewModel cust in _allcustomers)
            {
                if (cust.Customer.ID == _customerid)
                {
                    _allcustomers.Remove(cust);
                    break;
                }
            }
        }

        private void Customerfilter_SelectionChanged(int _CustomerSelectedValue)
        {
            GetCustomers(_CustomerSelectedValue);
            foreach (TVCustomerViewModel tc in Customers)
            {
                if (tc.Customer.ID == (int)_CustomerSelectedValue)
                {
                    tc.IsFiltered = true;
                    tc.IsExpanded = true;
                }
                else
                {
                    tc.IsFiltered = false;
                    tc.IsExpanded = false;
                }
            }
        }


        //===============================================================================================================================================

        #region Static classes to manipulate the treeview nodes
        //Remove customer
        //Add new Asset node
        //Add new Customer node
        //Move Asset nodes
        //Update child nodes when moving from customer to new customer
        //Get Asset node based on AssetID
        //Get Customer node based on CustomerID
        //Update open windows when changing parent CustomerID      
        //Delete Asset
        //Log Asset movements
        //===============================================================================================================================================

        public static void RemoveCustomer(int custid)
        {
            Window _owner = Application.Current.Windows[0];
            (_owner.DataContext as ViewModels.AssetTreeExViewModel).RemoveCustomerVM(custid);
        }


        public static void AddAssetNode(Models.AssetModel _asset)
        {
            TVCustomerViewModel _customer;
            TVAssetViewModel _parentnode = null;

            //determine if asset is in tree
            //if found then update via bindings
            //else add to tree

            if (_asset.ParentAssetID == 0) //customer is parent
            {
                //get customer node
                _customer = GetCustomerNode(_asset.CustomerID);
                //look for asset under this customer              
                if (GetCustomerAsset(_customer, _asset.AssetID) == null) //not found
                {
                    //new Nov 2016 - added tvasset.Parent = _customer;
                    TVAssetViewModel tvasset = new TVAssetViewModel(_asset, null);
                    tvasset.Parent = _customer;
                    _customer.Children.Add(tvasset);

                    //add to customer node
                    // _customer.Children.Add(new TVAssetViewModel(_asset, null));
                }
            }
            else //asset is parent
            {
                //get customer node
                _customer = GetCustomerNode(_asset.CustomerID);
                //get parent node of asset                            
                _parentnode = GetAssetDFS(_customer, _asset.ParentAssetID);

                //new Nov 2016 - added tvasset.Parent = _parentnode;
                TVAssetViewModel tvasset = new TVAssetViewModel(_asset, null);
                tvasset.Parent = _parentnode;
                _parentnode.Children.Add(tvasset);

                //        _parentnode.Children.Add(new TVAssetViewModel(_asset, null));


            }
            ViewModels.AssetTreeExViewModel.LogMovement(ActivityType.NewAsset, _asset.AssetID, 0, _asset.CustomerID);
        }

        public static TVAssetViewModel GetCustomerAsset(TVCustomerViewModel cv, int _assetid)
        {
            foreach (TVAssetViewModel tvasset in cv.Children)
            {
                if (tvasset.Asset.AssetID == _assetid)
                {
                    return tvasset;
                }
            }
            return null;
        }

        /// <summary>
        /// Add new Customer Node to treeview
        /// </summary>
        /// <param name="_customer">CustomerModel</param>
        public static void AddCustomerNode(Models.CustomerModel _customer)
        {
            Window _owner = Application.Current.Windows[0];
            FullyObservableCollection<TVCustomerViewModel> _customers = (_owner.DataContext as ViewModels.AssetTreeExViewModel)._allcustomers;

            _customers.Add(new TVCustomerViewModel(_customer));

        }


        /// <summary>
        /// Move selected asset to new parent with new customer
        /// Update open Asset windows
        /// </summary>
        /// <param name="_assetid"></param>
        /// <param name="_newparentassetid"></param>
        /// <param name="_newcustomerid"></param>
        public static void MoveAsset(int _assetid, int _newparentassetid, int _newcustomerid)
        {
            Window _owner = Application.Current.Windows[0];
            FullyObservableCollection<TVCustomerViewModel> _customers = (_owner.DataContext as ViewModels.AssetTreeExViewModel)._allcustomers;

            TreeViewItemViewModel _asset = null;
            //get the node representing the asset
            foreach (TVCustomerViewModel cm in _customers)
            {
                _asset = GetAssetDFS(cm, _assetid);
                if (_asset != null)
                    break;
            }

            TVCustomerViewModel _customer;
            if (_asset != null)
            {
                TreeViewItemViewModel _parentasset = null;
                //new parent id = 0 occurs when the asset is added directly to customer
                if (_newparentassetid != 0)
                    foreach (TVCustomerViewModel cm in _customers)
                    {
                        _parentasset = GetAssetDFS(cm, _newparentassetid);
                        if (_parentasset != null)
                            break;
                    }
                else
                    _parentasset = GetCustomerNode(_newcustomerid);

                //in error condition, check that parent asset is not null - rare but still should be checked
                if (_parentasset != null)
                {
                    //check to see if asset is directly under a customer or if its under another asset          
                    if ((_asset as TVAssetViewModel).Asset.ParentAssetID == 0)
                    {
                        //get customer node from customerid
                        //remove asset node from customer node if customerid !=0
                        if ((_asset as TVAssetViewModel).Asset.CustomerID != 0)
                        {
                            _customer = GetCustomerNode((_asset as TVAssetViewModel).Asset.CustomerID);
                            _customer.Children.Remove(_asset);
                        }
                    }
                    else
                        _asset.Parent.Children.Remove(_asset);


                    //create movement data
                    Models.AssetMovementModel _amm = new Models.AssetMovementModel();
                    _amm.ActivityCodeID = 1;
                    _amm.DateMoved = DateTime.Now;
                    _amm.SourceCustomerID = (_asset as TVAssetViewModel).Asset.CustomerID;
                    _amm.DestinationCustomerID = _newcustomerid;

                    //add movement to log
                    LogMovement(ActivityType.Transfer, _assetid, _amm.SourceCustomerID, _newcustomerid);

                    //update child assets to correct customerid and status
                    UpdateChildAsset(_assetid, _newcustomerid, (int)StatusType.InUse);
                    UpdateCustomerID(_asset.Children, _newcustomerid, _amm);
                    //update open windows to reflect the changes made to data
                    UpdateRelatedWindow(_assetid, _newcustomerid, (int)StatusType.InUse, _owner);
                    (_asset as TVAssetViewModel).Asset.ParentAssetID = _newparentassetid;
                    (_asset as TVAssetViewModel).Asset.CustomerID = _newcustomerid;
                    (_asset as TVAssetViewModel).Asset.StatusID = (int)StatusType.InUse;
                    (_asset as TVAssetViewModel).IsExpanded = true;
                    _parentasset.Children.Add(_asset);
                    _parentasset.IsExpanded = true;
                    _asset.Parent = _parentasset;
                }
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="theseNodes"></param>
        /// <param name="_custid"></param>
        /// <param name="_amm"></param>
        public static void UpdateCustomerID(ObservableCollection<TreeViewItemViewModel> theseNodes, int _custid, Models.AssetMovementModel _amm)
        {
            foreach (TVAssetViewModel am in theseNodes)
            {
                LogMovement(ActivityType.Transfer, am.Asset.AssetID, am.Asset.CustomerID, _custid);

                am.Asset.CustomerID = _custid;
                //Call datalayer update
                UpdateChildAsset(am.Asset.AssetID, _custid, (int)StatusType.InUse);

                //if asset window is open then update window values
                UpdateRelatedWindow(am.Asset.AssetID, _custid, (int)StatusType.InUse, Application.Current.Windows[0]);

                if (am.Children != null)
                    UpdateCustomerID(am.Children, _custid, _amm);
            }

        }

        /// <summary>
        /// Depth first search of asset tree for the given node
        /// If found the node is returned as a static variable
        /// else null
        /// </summary>
        /// This avoids recursion and finds the first occurrence of the target node 
        /// in the case where there could be multiple instances or circular references.
        /// Need to be careful about size of stack. Data tree shape might prefer BFS instead.
        /// Loop should run faster than recursion
        /// <returns></returns>
        public static TVAssetViewModel GetAssetDFS(TVCustomerViewModel cv, int _targetid)
        {
            var stack = new Stack<TVAssetViewModel>();
            foreach (TVAssetViewModel tv in cv.Children)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                TVAssetViewModel current = stack.Pop();
                //assign current node to found asset
                if (current.Asset.AssetID == _targetid)
                {
                    return current;
                }
                foreach (TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
            return null;
        }

        /// <summary>
        /// Get Customer matching given ID
        /// </summary>
        /// <param name="cv">Customer node collection</param>
        /// <param name="_targetid">Customer ID</param>
        /// <returns></returns>
        public static TVCustomerViewModel GetCustomerNode(int _targetid)
        {
            Window _owner = Application.Current.Windows[0];
            FullyObservableCollection<TVCustomerViewModel> _customers = (_owner.DataContext as ViewModels.AssetTreeExViewModel)._allcustomers;

            foreach (TVCustomerViewModel cm in _customers)
            {
                if (cm.Customer.ID == _targetid)
                    return cm;
            }
            return null;
        }

        public static TVAssetViewModel GetAssetNode(int _assetid, int _customerid)
        {
            TVCustomerViewModel _customer = GetCustomerNode(_customerid);
            return GetAssetDFS(_customer, _assetid);
        }

        /// <summary>
        /// Update open windows with new CustomerID and Status
        /// </summary>
        /// <param name="_id">Current Asset ID</param>
        /// <param name="_customerid">New Customer ID</param>
        /// <param name="_statusid">New Status ID</param>
        /// <param name="_owner">Owner of window</param>
        public static void UpdateRelatedWindow(int _id, int _customerid, int _statusid, Window _owner)
        {
            foreach (Window w in _owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    ViewModels.AssetViewModel _assetvm = ((ViewModels.AssetViewModel)((Views.AssetView)w).DataContext);
                    if (_assetvm.AssetID == _id)
                    {
                        _assetvm.AssetData.CustomerID = _customerid;
                        _assetvm.AssetData.StatusID = _statusid;
                        break;
                    }
                }
            }
        }

        public static void CloseRelatedWindow(int _id, Window _owner)
        {
            foreach (Window w in _owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    AssetViewModel _assetvm = ((AssetViewModel)((Views.AssetView)w).DataContext);
                    if (_assetvm.AssetID == _id)
                    {
                        _assetvm.CloseWindow();
                        break;
                    }
                }
            }
        }

        public static bool OpenAssetWindow(int _assetid)
        {
            bool _isopen = false;
            Window _owner = Application.Current.Windows[0];
            foreach (Window w in _owner.OwnedWindows)
            {
                if (w.GetType().Equals(typeof(Views.AssetView)))
                {
                    if (((ViewModels.AssetViewModel)((Views.AssetView)w).DataContext).AssetID == _assetid)
                    {
                        w.Activate();
                        _isopen = true;
                        break;
                    }
                }
            }
            return _isopen;
        }

        /// <summary>
        /// DFS search to undelete assets undet the given asset. 
        /// Undeleted assets are located at root of selected customer
        /// </summary>
        /// <param name="_assetid">Current Asset ID</param>
        /// <param name="_newcustomerid">New (default) Customer ID for undeleted assets</param>
        public static void UnDeleteChildAssets(int _assetid, int _newcustomerid)
        {
            FullyObservableCollection<Models.AssetModel> _assets = GetDeletedChildAssets(_assetid);
            var stack = new Stack<Models.AssetModel>();
            foreach (Models.AssetModel tv in _assets)
                stack.Push(tv);
            while (stack.Count != 0)
            {
                Models.AssetModel current = stack.Pop();
                //assign current node to found asset
                DatabaseQueries.UnDeleteAsset(current.AssetID, _newcustomerid, (int)StatusType.Available);

                LogMovement(ActivityType.Undeleted, _assetid, 0, _newcustomerid);

                foreach (Models.AssetModel child in GetChildAssets(current.AssetID))
                    stack.Push(child);

            }
           
        }

        /// <summary>
        /// Build new subtree with deleted asset
        /// </summary>
        /// <param name="_assetid">Current Asset ID</param>
        /// <param name="_newcustomerid">New Customer ID</param>
        // public static void UnDeleteAsset(Models.AssetSummaryModel _asset, int _newcustomerid)
        public static void UnDeleteAsset(Models.AssetModel _asset, int _newcustomerid)
        {
            TVCustomerViewModel _customer;
            _customer = GetCustomerNode(_newcustomerid);
            DatabaseQueries.UnDeleteAsset(_asset.AssetID, _newcustomerid, (int)StatusType.InUse);
            LogMovement(ActivityType.Undeleted, _asset.AssetID, 0, _newcustomerid);

            //update child assets to correct customer and status
            UnDeleteChildAssets(_asset.AssetID, _newcustomerid);

            //add first node
            Models.AssetModel am = GetAsset(_asset.AssetID);
            am.CustomerID = _newcustomerid;
            _customer.Children.Add(new TVAssetViewModel(am, null));

        }


        /// <summary>
        /// Delete asset matching _assetid. Child assets are also deleted
        /// </summary>
        /// <param name="_assetid">Current Asset ID</param>
        /// <param name="_assetparentid">Asset parent ID</param>
        /// <param name="_customerid">Asset customer ID</param>
        public static void DeleteAssetDFS(TVAssetViewModel _tvasset)
        {
            Window _owner = Application.Current.Windows[0];
            TVCustomerViewModel cm = GetCustomerNode(_tvasset.Asset.CustomerID);
            TVAssetViewModel _asset = null;
            _asset = GetAssetDFS(cm, _tvasset.Asset.AssetID);
            TVAssetViewModel _assetparent = null;
            _assetparent = GetAssetDFS(cm, _tvasset.Asset.ParentAssetID);

            if (_asset != null)
            {
                if (_assetparent != null)
                    _assetparent.Children.Remove(_asset);
                else
                    cm.Children.Remove(_asset);

                SetParentAssetID(_tvasset.Asset.AssetID, 0);

                var stack = new Stack<TVAssetViewModel>();
                stack.Push(_asset);
                while (stack.Count != 0)
                {
                    TVAssetViewModel current = stack.Pop();
                    DeleteItem((current).Asset.AssetID, DeleteSPName.DeleteAsset);
                    CloseRelatedWindow(current.Asset.AssetID, _owner);
                    LogMovement(ActivityType.Deleted, current.Asset.AssetID, _tvasset.Asset.CustomerID, 0);

                    foreach (TVAssetViewModel child in (current.Children).Reverse())
                        stack.Push(child);
                }
            }
           
        }

        public static Collection<int> _ChildNodeIDs = new Collection<int>();

        /// <summary>
        /// Create list of child nodes that cannot be dropped onto.
        /// </summary>
        /// <param name="_tvasset"></param>
        /// <returns></returns>
        public static Collection<int> GetChildIDs(ViewModels.TVAssetViewModel _tvasset)
        {
            _ChildNodeIDs.Clear();
            var stack = new Stack<ViewModels.TVAssetViewModel>();
            stack.Push(_tvasset);
            while (stack.Count != 0)
            {
                ViewModels.TVAssetViewModel current = stack.Pop();
                //add current asset id to collecton
                _ChildNodeIDs.Add(current.Asset.AssetID);

                foreach (ViewModels.TVAssetViewModel child in (current.Children).Reverse())
                    stack.Push(child);
            }
            return null;
        }


        /// <summary>
        /// Log asset creation, deletion and movements within customers
        /// </summary>
        /// <param name="_activitycodeid"></param>
        /// <param name="_assetid">Current Asset</param>
        /// <param name="_srccustomerid">Source customer ID</param>
        /// <param name="_destcustomerid">Destination customer ID</param>
        public static void LogMovement(ActivityType _activitycodeid, int _assetid, int _srccustomerid, int _destcustomerid)
        {
            Models.AssetMovementModel _amm = new Models.AssetMovementModel();
            _amm.AssetID = _assetid;
            _amm.ActivityCodeID = (int)_activitycodeid;
            _amm.DateMoved = DateTime.Now;
            _amm.SourceCustomerID = _srccustomerid;
            _amm.DestinationCustomerID = _destcustomerid;
            AddAssetMovement(_amm);
        }





        #endregion
    }

}
