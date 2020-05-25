using System.Collections.Generic;
using System.Data;
using System.Linq;
using static PTR.DatabaseQueries;
using static PTR.StaticCollections;
using PTR.Models;

namespace PTR.ViewModels
{
    public class StatusReportViewModel : ReportsFilterModule
    {        

        private readonly string[] excludedcols = { "Customer", "UserName" };

        public StatusReportViewModel()
        {
            ExecuteApplyModuleFilter = ExecuteApplyFilter;
            ExecuteFMOpenProject = ExecuteOpenProject;
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

        DataTable datatable;
        public DataTable Data
        {
            get { return datatable; }
            set { SetField(ref datatable, value); }
        }

        #endregion

        #region Commands 

        DataRowView selectedproject;
        public DataRowView SelectedProject
        {
            get { return selectedproject; }
            set { SetField(ref selectedproject, value); }
        }

        private void ExecuteOpenProject(object parameter)
        {
            try
            {
                if (parameter != null)
                {
                    object[] values = new object[2];
                    values = parameter as object[];
                    int id = (int)values[0];
                    if (id > 0)
                    {       
                        IMessageBoxService msgbox = new MessageBoxService();
                        //if return value is true then Refresh list
                        if (msgbox.OpenProjectDlg((System.Windows.Window)values[1], id))
                        {
                            FilterData();
                            SetPopupFilters(excludedcols, Data);
                            ApplyPopupFilter();
                        }
                        msgbox = null;
                    }               
                }
            }
            catch { }
        }
            
        private void ExecuteExportToExcel(object parameter)
        {
            try
            {
                //DataTable dt = datatable.Copy();
                //foreach (DataColumn dc in datatable.Columns)
                //    if ((int)dc.ExtendedProperties["FieldType"] == 1)
                //        dt.Columns.Remove(dc.ColumnName);
                
                ExcelLib xl = new ExcelLib();           
                xl.MakeGenericReport((System.Windows.Window)parameter, datatable);
                xl = null;
                //dt.Dispose();
                //dt = null;
            }
            catch
            {
                IMessageBoxService msg = new MessageBoxService();
                msg.ShowMessage("There was a problem creating the Excel report","Unable to create Excel report", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
                msg = null;
            }
        }

        private void ExecuteApplyFilter(object parameter)
        {
            FilterData();
            SetPopupFilters(excludedcols, Data);
            ApplyPopupFilter();
        }

        DataTable masterdatatable;
        private void FilterData()
        {
            masterdatatable = GetStatusReportFiltered(CountriesSrchString);
            Data = masterdatatable;
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
                //if (PopupFilterDictContains(Constants.StatusReportPopupList, DictFilterPopup))
                //{
                //    Dictionary<string, List<string>> locdictFilterPopup = new Dictionary<string, List<string>>();
                //    foreach (string s in Constants.StatusReportPopupList)
                //        locdictFilterPopup.Add(s, DictFilterPopup[s].FilterData.Where(y => y.IsChecked == true).Select(x => x.Description).ToList<string>());

                //    var c1 = masterdatatable.AsEnumerable()
                //    .Where(row => locdictFilterPopup["SalesDivision"].Contains(row["SalesDivision"].ToString())
                //        && locdictFilterPopup["KPM"].Contains(row["KPM"].ToString())
                //        && locdictFilterPopup["Customer"].Contains(row["Customer"].ToString())
                //        && locdictFilterPopup["UserName"].Contains(row["UserName"].ToString())
                //        && locdictFilterPopup["ProjectStatus"].Contains(row["ProjectStatus"].ToString())
                //        && locdictFilterPopup["ProjectType"].Contains(row["ProjectType"].ToString())
                //        && locdictFilterPopup["SalesFunnelStage"].Contains(row["SalesFunnelStage"].ToString())
                //    );
                 
                //    if (c1.Count() > 0)
                //    {
                //        DataTable tblFiltered = c1.CopyToDataTable();
                //        ReFormatColumns(ref masterdatatable, ref tblFiltered);
                //        Data = tblFiltered;
                //    }
                //    else
                //        Data = masterdatatable.Clone();
                //}
                //else
                //    Data = masterdatatable;

                Data = DynamicFilter.FilterDataTable(masterdatatable, Constants.StatusReportPopupList, DictFilterPopup);
            }
            catch
            {

            }
        }

        private void InitializePopupFilters()
        {
            try
            {
                foreach (string colname in Constants.StatusReportPopupList)
                {
                    if (!DictFilterPopup.ContainsKey(colname))
                        DictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = Data.Columns[colname].Caption, IsApplied = false });

                    FilterPopupModel s = new FilterPopupModel();
                    bool success = DictFilterPopup.TryGetValue(colname, out s);

                    if (colname == "SalesDivision")
                    {                      
                        foreach (ModelBaseVM ev in BusinessUnits)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Name, IsChecked = (ev.Name == "PT") });
                        s.IsApplied = true;
                    }
                    else
                    if (colname == "KPM")
                    {
                        s.FilterData.Add(new FilterPopupDataModel() { Description = "Yes", IsChecked = true });
                        s.FilterData.Add(new FilterPopupDataModel() { Description = "No", IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    if (colname == "ProjectStatus")
                    {
                        foreach (EnumValue ev in EnumerationLists.ProjectStatusTypesList)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.Description, IsChecked = (ev.Description == "Active") });
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
                     if (colname == "SalesFunnelStage")
                    {
                        foreach (ActivityStatusCodesModel ev in ActivityStatusCodes)
                            s.FilterData.Add(new FilterPopupDataModel() { Description = ev.PlaybookDescription, IsChecked = true });
                        s.IsApplied = false;
                    }
                    else
                    {
                        if (colname == "Customer" || colname == "UserName")
                        {
                            foreach (DataRow dr in Data.Rows)
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
