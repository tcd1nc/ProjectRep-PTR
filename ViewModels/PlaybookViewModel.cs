using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using static PTR.DatabaseQueries;
using static PTR.StaticCollections;

namespace PTR.ViewModels
{
    public class PlaybookViewModel : FilterModule, IDisposable
    {
        DataTable _salesfunnel = new DataTable();
        DataTable _newbusiness = new DataTable();
        DateTime? _startmonthprojects;
        DateTime? _firstmonthcomments;
        DateTime? _lastmonthcomments;
               
        public PlaybookViewModel()
        {                  
            _firstmonthcomments = new DateTime(DateTime.Now.Year-1, 12, 1);
            _lastmonthcomments = new DateTime(DateTime.Now.Year, 12, 1);
            _startmonthprojects = GetStartMonth();

            FilterData();
            MakeProjectStatusList();
            ProjectStatuses.ItemPropertyChanged += ProjectStatuses_ItemPropertyChanged;
             
        }
       
        #region Event handlers

        private void ProjectStatuses_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            //UpdateSelectedProjectStatuses(ProjectStatuses[e.CollectionIndex].Description, ProjectStatuses[e.CollectionIndex].Status);
            UpdateSelectedProjectStatuses(ProjectStatuses[e.CollectionIndex].Description, ProjectStatuses[e.CollectionIndex].Colour);
            //need to tell binding engine so re-draw occurs
            OnPropertyChanged("SalesFunnel");
        }

        #endregion

        #region Properties

        //int selecteddivisionid;
        //public int SelectedDivisionID
        //{
        //    get { return selecteddivisionid; }
        //    set { SetField(ref selecteddivisionid, value); }
        //}

        public DataTable SalesFunnel
        {
            get { return _salesfunnel; }
            set { SetField(ref _salesfunnel, value); }
        }

        public DataTable NewBusiness
        {
            get { return _newbusiness; }
            set { SetField(ref _newbusiness, value); }
        }

        Dictionary<string, string> selectedprojectstatuses = new Dictionary<string, string>();
        private void UpdateSelectedProjectStatuses(string _description, string _status)
        {
            if (SelectedProjectStatuses.ContainsKey(_description))
                SelectedProjectStatuses.Remove(_description);
            else
                SelectedProjectStatuses.Add(_description, _status);
        }

        public Dictionary<string, string> SelectedProjectStatuses
        {
            get { return selectedprojectstatuses; }
            set { SetField(ref selectedprojectstatuses, value); }
        }

        

        FullyObservableCollection<Models.ProjectStatusListItem> _projectstatuses;
        public FullyObservableCollection<Models.ProjectStatusListItem> ProjectStatuses
        {
            get { return _projectstatuses; }
            set { SetField(ref _projectstatuses, value); }
        }

        private void MakeProjectStatusList()
        {
            ProjectStatuses = new FullyObservableCollection<Models.ProjectStatusListItem>();
            foreach (Models.ActivityStatusCodesModel asm in StaticCollections.ActivityStatusCodes)
            {
                if (asm.GOM.ID < 11) 
                    ProjectStatuses.Add(new Models.ProjectStatusListItem() {ID = asm.GOM.ID, Colour = asm.Colour, Status = asm.GOM.Name, Description = asm.GOM.Description, IsSelected=false});
            }
        }

        bool _GetMiscColumns = true;
        public bool GetMiscColumns
        {
            get { return _GetMiscColumns; }
            set { SetField(ref _GetMiscColumns, value); }
        }

        private DateTime GetStartMonth()
        {
            return new DateTime(DateTime.Now.Year, 1, 1);
        }

        public DateTime? StartMonthProjects
        {
            get { return _startmonthprojects; }
            set { SetField(ref _startmonthprojects, value); }
        }

        public DateTime? FirstMonthComments
        {
            get { return _firstmonthcomments; }
            set { SetField(ref _firstmonthcomments, value); }
        }
        public DateTime? LastMonthComments
        {
            get { return _lastmonthcomments; }
            set { SetField(ref _lastmonthcomments, value); }
        }

        #endregion


        #region Commands

        ICommand _exporttoexcel;
        public ICommand ExportToExcel
        {
            get
            {
                if (_exporttoexcel == null)
                    _exporttoexcel = new DelegateCommand(CanExecute, ExecuteExportToExcel);
                return _exporttoexcel;
            }
        }

        private void ExecuteExportToExcel(object parameter)
        {
            try
            {
                CreateExcelFile c = new CreateExcelFile();
                if (SingleDivision)
                    c.CreatePlaybookReport(SalesFunnel, NewBusiness, SelectedProjectStatuses);
                else
                    c.CreatePlaybookSFReport(SalesFunnel, SelectedProjectStatuses);
            }
            catch
            {
                IMessageBoxService _msg = new MessageBoxService();
                _msg.ShowMessage("There was a problem creating the Excel report", "Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                _msg = null;
            }
        }

        ICommand _applyfilter;
        public ICommand ApplyFilter      
        {
            get
            {
                if (_applyfilter == null)
                {
                    _applyfilter = new DelegateCommand(CanExecute, ExecuteApplyFilter);
                }
                return _applyfilter;
            }
        }
        private void ExecuteApplyFilter(object parameter)
        {
            FilterData();
        }

        private void FilterData()
        {
           // SingleDivisionVisibility = (SingleDivision == true) ? Visibility.Visible : Visibility.Hidden;
                  
            SalesFunnel = GetAllSalesFunnelReport(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, (DateTime)StartMonthProjects, (DateTime)FirstMonthComments, (DateTime)LastMonthComments, GetMiscColumns);           
                        
            NewBusiness = GetNewBusinessReport(CountriesSrchString, SalesDivisionSrchString, ProjectStatusTypesSrchString, ProjectTypesSrchString, (DateTime)StartMonthProjects, (DateTime)FirstMonthComments, (DateTime)LastMonthComments);
            
        }

        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }
             
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources  
                if (_newbusiness != null)
                {
                    _newbusiness.Dispose();
                    _newbusiness = null;
                }
                if (_salesfunnel != null)
                {
                    _salesfunnel.Dispose();
                    _salesfunnel = null;
                }
            }
        }
       
        

        #endregion
    }

    //public class ProjectStatusListItem : ViewModelBase
    //{
    //    private int _ID;
    //    private string _description;
    //    private string _status;
    //    private string _colour;
    //    private bool _isSelected;
       
    //    public int ID
    //    {
    //        get { return _ID; }
    //        set { _ID = value; }
    //    }

    //    public string Description
    //    {
    //        get { return _description; }
    //        set {SetField(ref _description,value); }
    //    }

    //    public string Status
    //    {
    //        get { return _status; }
    //        set { SetField(ref _status, value); }
    //    }

    //    public string Colour
    //    {
    //        get { return _colour; }
    //        set { SetField(ref _colour, value); }
    //    }

    //    public bool IsSelected
    //    {
    //        get { return _isSelected; }
    //        set { SetField(ref _isSelected, value);
    //        }
    //    }

    //}
}
