using System;
using System.ComponentModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using static PTR.Constants;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for PlaybookView.xaml
    /// </summary>
    public partial class PlaybookView : Window
    {
  
        public PlaybookView()
        {
            InitializeComponent();
            this.DataContext = new ViewModels.PlaybookViewModel();
            
            ((ViewModels.PlaybookViewModel)DataContext).PropertyChanged += PlaybookView_PropertyChanged;

            UpdateData((DataContext as ViewModels.PlaybookViewModel).SalesFunnel);
            UpdateData((DataContext as ViewModels.PlaybookViewModel).NewBusiness);
        }

        private void PlaybookView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName=="SalesFunnel")
                UpdateData((DataContext as ViewModels.PlaybookViewModel).SalesFunnel);
            else
            if (e.PropertyName == "NewBusiness")
                UpdateData((DataContext as ViewModels.PlaybookViewModel).NewBusiness);
        }
               
        public void UpdateData(DataTable _dt)
        {           
            string _tablename = string.Empty;
            
            GridView gridView = new GridView();
            GridViewColumn gridviewcol;
            for (int i = 0; i < _dt.Columns.Count; i++)
                {
                                
                gridviewcol = new GridViewColumn();
                gridviewcol.Header = _dt.Columns[i].Caption;
                gridviewcol.Width = Double.NaN;

                Binding bind = new Binding(_dt.Columns[i].ColumnName);
                bind.Mode = BindingMode.OneTime;

                DataTemplate cell = new DataTemplate(); // create a datatemplate
                FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
                factory.SetBinding(TextBlock.TextProperty, bind);//the second parameter should be bind   
                factory.SetValue(TextBlock.PaddingProperty, new Thickness(2,0,2,0));
                          
                FrameworkElementFactory bdrfactory = new FrameworkElementFactory(typeof(Border));
                bdrfactory.SetValue(Border.BorderBrushProperty, new SolidColorBrush(Colors.Gray));
                bdrfactory.SetValue(Border.BorderThicknessProperty, new Thickness(0, 0, 0.5, 0.5));
                bdrfactory.SetValue(Border.MarginProperty, new Thickness(-6, -2, -6, -2));
                bdrfactory.AppendChild(factory);
                if (_dt.Columns[i].DataType.Equals(typeof(DateTime)))
                    bind.StringFormat = "d MMM yyyy";// "d";
                else
                {
                    if (_dt.Columns[i].DataType.Equals(typeof(int)) || _dt.Columns[i].DataType.Equals(typeof(decimal)))
                        if(_dt.Columns[i].ExtendedProperties.ContainsKey("Format"))
                        bind.StringFormat = "{0:" + _dt.Columns[i].ExtendedProperties["Format"].ToString() + "}";
                }

                if (_dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Left")
                    factory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Left);
                else                    
                    if (_dt.Columns[i].ExtendedProperties["Alignment"].ToString() == "Right")
                    factory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Right);
                else
                    factory.SetValue(TextBlock.TextAlignmentProperty, TextAlignment.Center);

                cell.VisualTree = bdrfactory;
                gridviewcol.CellTemplate = cell;

                gridView.Columns.Add(gridviewcol);
            }

            ListView _lv = null;

            switch (_dt.TableName)
            {
                case SalesFunnel:
                    _lv = lvSalesFunnel;
                    break;
                case NewBusiness:
                    _lv = lvNewBusiness;
                    break;
            }
            _lv.BeginInit();
            _lv.View = gridView;
            _lv.ItemsSource = _dt.DefaultView;
            _lv.EndInit();                               

            
        }
       
    }

}
