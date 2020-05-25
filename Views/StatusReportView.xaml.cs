using PTR.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for ReportView.xaml
    /// </summary>
    public partial class StatusReportView : Window
    {
        DataTable dt;
        Dictionary<string, FilterPopupModel> dictFilterPopup;
        public StatusReportView()
        {
            InitializeComponent();            
            try
            {     
                this.DataContext = new ViewModels.StatusReportViewModel();
                ((ViewModels.StatusReportViewModel)DataContext).PropertyChanged += StatusReportView_PropertyChanged;
                dt = (DataContext as ViewModels.StatusReportViewModel).Data;
                dictFilterPopup = (DataContext as ViewModels.StatusReportViewModel).DictFilterPopup;
            }
            catch
            {
            }
        }

        private void StatusReportView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                dt = (DataContext as ViewModels.StatusReportViewModel).Data;
                dictFilterPopup = (DataContext as ViewModels.StatusReportViewModel).DictFilterPopup;
            }
            catch
            {
            }
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                var f = new FrameworkElementFactory(typeof(TextBlock));
                Binding b = new Binding(e.Column.Header.ToString())
                {
                    Mode = BindingMode.OneTime
                };
                string colname = e.PropertyName;
                switch (colname)
                {
                    case "SalesFunnelStage":
                        f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                        f.SetValue(TextBlock.TextProperty, b);

                        if (!dictFilterPopup.ContainsKey(colname))
                            dictFilterPopup.Add(colname, new FilterPopupModel() { ColumnName = colname, Caption = dt.Columns[colname].Caption, IsApplied = false });

                        FilterPopupModel s = new FilterPopupModel();
                        bool success = dictFilterPopup.TryGetValue(colname, out s);

                        foreach (DataRow dr in dt.Rows)
                            if (s.FilterData.Count(x => x.Description == dr[colname].ToString()) == 0 && !string.IsNullOrEmpty(dr[colname].ToString()))
                                s.FilterData.Add(new FilterPopupDataModel() { Description = dr[colname].ToString(), IsChecked = true });

                        //sort!
                        s.FilterData = StaticCollections.SortSFStagePopup(s.FilterData);

                        e.Column = new DataGridTemplateColumn()
                        {
                            Header = s,
                            HeaderStyle = FindResource("ColumnHeaderStyle") as Style,
                            HeaderTemplate = FindResource("ColumnHeaderFilterTemplate") as DataTemplate,
                            CellTemplate = FindResource("SalesFunnelStageTemplate") as DataTemplate,
                            CellStyle = FindResource("CellStyle") as Style
                        };
                        e.Column.SortMemberPath = colname;
                        break;

                    case "StatusColour":
                        e.Cancel = true;
                        break;
                    case "ProjectTypeColour":
                        e.Cancel = true;
                        break;
                    default:

                        if (dt.Columns[colname].ExtendedProperties.ContainsKey("FieldType")
                            && (int)dt.Columns[colname].ExtendedProperties["FieldType"] != (int)ReportFieldType.SystemAndHidden
                            && (int)dt.Columns[colname].ExtendedProperties["FieldType"] != (int)ReportFieldType.SystemAndRemoved)
                        {
                            StaticCollections.FormatGridColumn(ref dt, colname, ref b, ref f);                                                                 
                            StaticCollections.FormatWithNoStatusFilterTemplate(colname, ref dt, this, ref e, ref f, ref dictFilterPopup, Constants.StatusReportPopupList);
                        }
                        else
                            e.Cancel = true;

                        break;
                }
            }
            catch
            {
            }

        }

       
    }
}
