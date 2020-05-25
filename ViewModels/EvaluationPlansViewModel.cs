using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using PTR.Models;
using static PTR.StaticCollections;

namespace PTR.ViewModels
{
    public class EvaluationPlansViewModel : ReportsFilterModule
    {
        private readonly string[] excludedcols = { "Customer", "UserName" };

        public EvaluationPlansViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            ExecuteRFExportToExcel = ExecuteExportToExcel;
            ExecuteClearPopupFilter = ExecuteClearFilterPopup;
            ExecuteResetPopupFilter = ExecuteResetFilterPopup;
            ExecuteApplyPopupFilter = ExecuteApplyFilterPopup;
            ExecuteClearFilters = ExecuteClearDataFilters;

            FilterData();
            InitializePopupFilters();
            ApplyPopupFilter();
        }

        #region Properties

        DataTable eps;
        public DataTable EPS
        {
            get { return eps; }
            set { SetField(ref eps, value); }
        }

        DataRowView selectedep;
        public DataRowView SelectedEP
        {
            get { return selectedep; }
            set { SetField(ref selectedep, value); }
        }
               
        #endregion

        #region Commands

        DataTable masterdatatable;
        private void ExecuteApplyFilter(object parameter)
        {
            FilterData();
            SetPopupFilters(excludedcols, EPS);
            ApplyPopupFilter();
        }

        private void FilterData()
        {
            masterdatatable = DatabaseQueries.GetEvaluationPlansReport(CountriesSrchString);
            EPS = masterdatatable;
        }

        ICommand openep;
        public ICommand OpenEP
        {
            get
            {
                if (openep == null)
                    openep = new DelegateCommand(CanExecute, ExecuteOpenEP);
                return openep;
            }
        }

        private void ExecuteOpenEP(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    object[] values = new object[2];
                    values = parameter as object[];
                    int projectid = (int)values[0];
                    if (projectid > 0 && SelectedEP != null)
                    {
                        int evalplanid = (int)SelectedEP["ID"];
                        if (evalplanid > 0)
                        {
                            IMessageBoxService msgbox = new MessageBoxService();
                            //if return value is true then Refresh list
                            if (msgbox.EvaluationPlanDialog((System.Windows.Window)values[1], evalplanid, projectid))
                            {
                                FilterData();
                                SetPopupFilters(excludedcols, EPS);
                                ApplyPopupFilter();
                            }
                            msgbox = null;                         
                        }
                    }
                }
            }
            catch { }
        }

        private void ExecuteExportToExcel(object parameter)
        {
            try
            {
                //DataTable dt = eps.Copy();
                //foreach (DataColumn dc in eps.Columns)
                //    if ((int)dc.ExtendedProperties["FieldType"] == 1)
                //        dt.Columns.Remove(dc.ColumnName);

                ExcelLib xl = new ExcelLib();
                xl.MakeGenericReport((System.Windows.Window)parameter, eps);
                xl = null;
                //dt.Dispose();
                //dt = null;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report", "Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        private void ExecuteClearFilterPopup(object parameter)
        {
            try
            {
                FilterPopupModel s = new FilterPopupModel();
                bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                if (success)
                {
                    foreach (FilterPopupDataModel fm in s.FilterData)
                        fm.IsChecked = false;

                    s.IsApplied = true;
                    ApplyPopupFilter();
                }
            }
            catch
            {
            }
        }

        private void ExecuteResetFilterPopup(object parameter)
        {
            try
            {
                FilterPopupModel s = new FilterPopupModel();
                bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                if (success)
                {
                    foreach (FilterPopupDataModel fm in s.FilterData)
                        fm.IsChecked = true;

                    s.IsApplied = false;
                    ApplyPopupFilter();
                }
            }
            catch
            {
            }
        }

        private void ExecuteApplyFilterPopup(object parameter)
        {
            try
            {
                FilterPopupModel s = new FilterPopupModel();
                bool success = DictFilterPopup.TryGetValue(parameter.ToString(), out s);
                if (success)
                {
                    s.IsApplied = false;
                    foreach (FilterPopupDataModel f in s.FilterData)
                    {
                        if (!f.IsChecked)
                        {
                            s.IsApplied = true;
                            break;
                        }
                    }
                    ApplyPopupFilter();
                }
            }
            catch
            {

            }
        }
                
        private void ExecuteClearDataFilters(object parameter)
        {
            foreach (KeyValuePair<string, FilterPopupModel> fd in DictFilterPopup)
            {
                FilterPopupModel fg = fd.Value;
                fg.IsApplied = false;
                foreach (FilterPopupDataModel fdata in fg.FilterData)                
                    fdata.IsChecked = true;
                
            }
            ApplyPopupFilter();
        }
        
        private void ApplyPopupFilter()
        {
            try
            {
                //if (PopupFilterDictContains(Constants.EvaluationPlansListReportPopupList, DictFilterPopup))
                //{                    
                //    Dictionary<string, List<string>> locdictFilterPopup = new Dictionary<string, List<string>>();
                //    foreach (string s in Constants.EvaluationPlansListReportPopupList)
                //        locdictFilterPopup.Add(s, DictFilterPopup[s].FilterData.Where(y => y.IsChecked == true).Select(x => x.Description).ToList<string>());

                //    var c1 = masterdatatable.AsEnumerable()
                //    .Where(row => locdictFilterPopup["SalesDivision"].Contains(row["SalesDivision"].ToString())
                //       && locdictFilterPopup["Customer"].Contains(row["Customer"].ToString())
                //       && locdictFilterPopup["UserName"].Contains(row["UserName"].ToString())
                //       && locdictFilterPopup["ProjectStatus"].Contains(row["ProjectStatus"].ToString())
                //       && locdictFilterPopup["ProjectType"].Contains(row["ProjectType"].ToString())
                //    );

                //    if (c1.Count() > 0)
                //    {                        
                //        DataTable tblFiltered = c1.CopyToDataTable();
                //        ReFormatColumns(ref masterdatatable, ref tblFiltered);
                //        EPS = tblFiltered;
                //    }
                //    else                    
                //        EPS = masterdatatable.Clone();                    
                //}
                //else
                //    EPS = masterdatatable;

                EPS = DynamicFilter.FilterDataTable(masterdatatable, Constants.EvaluationPlansListReportPopupList, DictFilterPopup);
            }
            catch
            {
            }
        }

        private void InitializePopupFilters()
        {
            try
            {
                foreach (string colname in Constants.EvaluationPlansListReportPopupList)
                {
                    if (!DictFilterPopup.ContainsKey(colname))
                        DictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = EPS.Columns[colname].Caption, IsApplied = false });

                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(colname, out s);

                    if (colname == "SalesDivision")
                    {             
                        foreach (ModelBaseVM ev in BusinessUnits)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = (ev.Name == "PT") });
                        s.IsApplied = true;
                    }
                    else
                    if (colname == "ProjectStatus")
                    {
                        foreach (EnumValue ev in EnumerationLists.ProjectStatusTypesList)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Description, IsChecked = (ev.Description == EnumerationManager.GetEnumDescription(ProjectStatusType.Active)) });
                        s.IsApplied = true;
                    }
                    else

                    if (colname == "ProjectType")
                    {
                        foreach (ProjectTypeModel ev in ProjectTypes)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    {
                        if (colname == "Customer" || colname == "UserName")
                        {
                            foreach (DataRow dr in EPS.Rows)
                                if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
                                    s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });
                            s.IsApplied = false;
                        }
                    }
                }
            }
            catch { }
        }
               
        #endregion
    }
}
