using System.Collections.Generic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using PTR.Models;

namespace PTR
{
    /// <summary>
    /// Interaction logic for PTMainView.xaml
    /// </summary>
    public partial class PTMainView : Window
    {
        DataTable dt;
        Dictionary<string, FilterPopupModel> dictFilterPopup;

        public PTMainView()
        {                                           
            InitializeComponent();
            try
            {
                this.DataContext = new ViewModels.PTMainViewModel();
                ((ViewModels.PTMainViewModel)DataContext).PropertyChanged += PTMainView_PropertyChanged;
                dt = (DataContext as ViewModels.PTMainViewModel).UserProjects;
                dictFilterPopup = (DataContext as ViewModels.PTMainViewModel).DictFilterPopup;
            }
            catch { }
        }

        private void PTMainView_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            try
            {
                dt = (DataContext as ViewModels.PTMainViewModel).UserProjects;
                dictFilterPopup = (DataContext as ViewModels.PTMainViewModel).DictFilterPopup;
            }
            catch { }
        }

        private void ProjectList_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            try
            {
                var f = new FrameworkElementFactory(typeof(TextBlock));
                Binding b = new Binding(e.Column.Header.ToString())
                {
                    Mode = BindingMode.OneWay,
                };
                string colname = e.PropertyName;
                switch (colname)
                {
                    case "StatusColour":
                        e.Cancel = true;
                        break;
                    case "Colour":
                        e.Cancel = true;
                        break;
                    case "ProjectTypeColour":
                        e.Cancel = true;
                        break;

                    case "Actions":
                        f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                        f.SetValue(TextBlock.TextProperty, b);
                        
                        e.Column = new DataGridTemplateColumn()
                        {
                            Header = dt.Columns[colname].Caption,
                            HeaderStyle = FindResource("ColumnHeaderStyle") as Style,
                            CellTemplate = FindResource("ActionsTemplate") as DataTemplate
                        };                        
                        break;

                    default:                       
                        StaticCollections.FormatGridColumn(ref dt, colname, ref b, ref f);                                               
                       
                        if (dt.Columns[colname].ExtendedProperties.ContainsKey("FieldType") && (int)dt.Columns[colname].ExtendedProperties["FieldType"] == (int)ReportFieldType.General)                        
                            StaticCollections.FormatWithNoStatusFilterTemplate(colname, ref dt, this, ref e, ref f, ref dictFilterPopup, Constants.MainviewPopupList);                                                    
                        else
                            e.Cancel = true;
                        break;
                }
            }
            catch
            {
            }
        }

        private void ProjectList_Sorting(object sender, DataGridSortingEventArgs e)
        {            
            (DataContext as ViewModels.PTMainViewModel).ExecuteUpdateActivities();
            (DataContext as ViewModels.PTMainViewModel).ExecuteClearActivities();
        
            ProjectList.SelectedIndex = -1;
            ProjectList.SelectedItem = null;
          
            e.Handled = false;
            
        }
    }

}

