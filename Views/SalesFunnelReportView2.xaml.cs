﻿using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;


namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for SalesFunnelReportView2.xaml
    /// </summary>
    public partial class SalesFunnelReportView2 : Window
    {
        public SalesFunnelReportView2()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.SalesFunnelReportViewModel2();
        }

        private void ReportGrid_AddingNewItem(object sender, AddingNewItemEventArgs e)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReportGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid dg = (sender as DataGrid);
            var c = dg.Columns[0];

            c.CellStyle = FindResource("StatusStyle") as Style;// StatusStyle;

            for (int i = 1; i < dg.Columns.Count; i++)
            {
                var c1 = dg.Columns[i];
                c1.CellStyle = FindResource("StatusStyle") as Style; // CurrencyStyle;
            }
        }

        private void ReportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {   
            var f = new FrameworkElementFactory(typeof(TextBlock));
            Binding b = new Binding(e.Column.Header.ToString());
            b.Mode = BindingMode.OneTime;

            switch (e.PropertyName)
            {
                case "Status":
                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                    f.SetValue(TextBlock.TextProperty, b);
                    e.Column = new DataGridTemplateColumn()
                    {
                        Header = "Status",
                        Width = 60,
                        HeaderStyle = FindResource("ColumnHeaderStyle") as Style,
                        CellTemplate = new DataTemplate() { VisualTree = f },
                    };
                    break;

                default:
                            
                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
                    b.StringFormat = (e.PropertyName.Contains("%"))? "P0" : "C0";
                    f.SetValue(TextBlock.TextProperty, b);
                    e.Column = new DataGridTemplateColumn()
                    {
                        Width= (e.PropertyName.Contains("%"))? 40 : 70,
                        Header = (e.PropertyName.Contains("%"))? "%" : e.Column.Header,
                        HeaderStyle = FindResource("ColumnHeaderStyle") as Style,
                        CellTemplate = new DataTemplate() { VisualTree = f },
                    };
                    break;

            }
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            DataGridRow r = e.Row as DataGridRow;
            DataRowView c = r.Item as DataRowView;
            if (c != null)
            {
              if(c.Row.Field<string>(0)!=null)                
                r.Background =  StaticCollections.ColorDictionary[c.Row.Field<string>(0).ToString()];
                               
            }
        }

        private void ProjectCountGrid_AutoGeneratedColumns(object sender, EventArgs e)
        {
            DataGrid dg = (sender as DataGrid);
            var c = dg.Columns[0];

            c.CellStyle = FindResource("StatusStyle") as Style;// StatusStyle;

            for (int i = 1; i < dg.Columns.Count; i++)
            {
                var c1 = dg.Columns[i];
                c1.CellStyle = FindResource("StatusStyle") as Style; // CurrencyStyle;
            }
        }

        private void ProjectCountGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {

            var f = new FrameworkElementFactory(typeof(TextBlock));
            Binding b = new Binding(e.Column.Header.ToString());
            b.Mode = BindingMode.OneTime;

            switch (e.PropertyName)
            {
                case "Status":
                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                    f.SetValue(TextBlock.TextProperty, b);
                    e.Column = new DataGridTemplateColumn()
                    {
                        Header = "Status",
                        Width = 60,
                        HeaderStyle = FindResource("ColumnHeaderStyle") as Style,
                        CellTemplate = new DataTemplate() { VisualTree = f },
                    };
                    break;
                
                default:

                    f.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);
                    b.StringFormat = "N0";
                    f.SetValue(TextBlock.TextProperty, b);
                    e.Column = new DataGridTemplateColumn()
                    {
                        Width =  70,
                        Header = e.Column.Header,
                        HeaderStyle = FindResource("ColumnHeaderStyle") as Style,
                        CellTemplate = new DataTemplate() { VisualTree = f },
                    };
                    break;
            }
        }

    }
}
