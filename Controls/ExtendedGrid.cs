using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace PTR
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BladeWear"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:BladeWear;assembly=BladeWear"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Browse to and select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:ExGrid/>
    ///
    /// </summary>
    public class ExtendedGrid : DataGrid
    {
        /*     static ExGrid()
             {
                 DefaultStyleKeyProperty.OverrideMetadata(typeof(ExGrid), new FrameworkPropertyMetadata(typeof(ExGrid)));
             }
             */

              
        public ExtendedGrid() : base()
        {
            

        }

        //protected override void OnPreviewKeyDown(KeyEventArgs e)
        //{
        //    base.OnPreviewKeyDown(e);

        //    if (e.Key == Key.Down)
        //    {
        //        CommitEdit();

        //        if ((e.Source as ExtendedGrid).CurrentColumn != null)
        //        {
        //            if ((e.Source as ExtendedGrid).CurrentColumn.GetType().Equals(typeof(DataGridTemplateColumn)))
        //            {
        //                if ((e.OriginalSource).GetType().Equals(typeof(TextBox)))
        //                {
        //                 //   TextBox t = (TextBox)(e.OriginalSource);
        //                    //get column or column index
        //                    int columnindex = this.Columns.IndexOf(this.CurrentColumn);
        //                    //get current row index
        //                    DataGridRow rowContainer = (DataGridRow)this.ItemContainerGenerator.ContainerFromItem(this.CurrentItem);
        //                    int rowindex = rowContainer.GetIndex();
        //                    //navigate to cell
        //                    if (rowindex < this.Items.Count - 1)                                                                                                                         
        //                        SelectCellByIndex(this, rowindex , columnindex);                                                                        
        //                }
        //            }
        //        }
        //    }
        //}


        //protected override void OnPreviewKeyUp(KeyEventArgs e)
        //{
        //    base.OnPreviewKeyUp(e);

        //    if (e.Key == Key.Up)
        //    {
        //        CommitEdit();
        //        if ((e.Source as ExtendedGrid).CurrentColumn != null)
        //        {
        //            if ((e.Source as ExtendedGrid).CurrentColumn.GetType().Equals(typeof(DataGridTemplateColumn)))
        //            {
        //                if ((e.OriginalSource).GetType().Equals(typeof(TextBox)))
        //                {
        //                 //   TextBox t = (TextBox)(e.OriginalSource);
        //                    //get column or column index
        //                    int columnindex = this.Columns.IndexOf(this.CurrentColumn);
        //                    //get current row index
        //                    DataGridRow rowContainer = (DataGridRow)this.ItemContainerGenerator.ContainerFromItem(this.CurrentItem);
        //                    int rowindex = rowContainer.GetIndex();
        //                    //navigate to cell
        //                    if (rowindex > 0)
        //                        SelectCellByIndex(this, rowindex - 1, columnindex);                                                                    
        //                }
        //            }
        //        }
        //    }
        //}


        protected override void OnPreviewKeyUp(KeyEventArgs e)
        {
            base.OnPreviewKeyUp(e);
            if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
                CommitEdit();
            if ((e.Source as ExtendedGrid).CurrentColumn != null)
            {
                if ((e.Source as ExtendedGrid).CurrentColumn.GetType().Equals(typeof(DataGridTemplateColumn)))
                {
                    if ((e.OriginalSource).GetType().Equals(typeof(TextBox)))
                    {
                        //get column or column index
                        int columnindex = this.Columns.IndexOf(this.CurrentColumn);
                        //get current row index
                        DataGridRow rowContainer = (DataGridRow)this.ItemContainerGenerator.ContainerFromItem(this.CurrentItem);
                        int rowindex = rowContainer.GetIndex();
                        //navigate to cell

                        switch (e.Key)
                        {
                            case Key.Up:
                                if (rowindex > 0)
                                {
                                    DataGridCell cell = SelectCellByIndex(this, rowindex - 1, columnindex);
                                    TextBox tb = FindVisualChild<TextBox>(cell);
                                    if (tb != null)
                                    {
                                        tb.Focus();
                                        tb.Select(0, tb.Text.Length);
                                    }
                                }
                                break;
                            case Key.Down:
                                if (rowindex < this.Items.Count - 1)
                                {
                                    DataGridCell cell = SelectCellByIndex(this, rowindex + 1, columnindex);
                                    TextBox tb = FindVisualChild<TextBox>(cell);
                                    if (tb != null)
                                    {
                                        tb.Focus();
                                        tb.Select(0, tb.Text.Length);
                                    }
                                }
                                break;
                            case Key.Left:
                                if (columnindex > 0)
                                {
                                    DataGridCell cell = SelectCellByIndex(this, rowindex, columnindex - 1);
                                    TextBox tb = FindVisualChild<TextBox>(cell);
                                    if (tb != null)
                                    {
                                        tb.Focus();
                                        tb.Select(0, tb.Text.Length);
                                    }
                                }
                                break;
                            case Key.Right:
                                if (columnindex < this.Columns.Count - 1)
                                {
                                    DataGridCell cell = SelectCellByIndex(this, rowindex, columnindex + 1);
                                    TextBox tb = FindVisualChild<TextBox>(cell);
                                    if (tb != null)
                                    {
                                        tb.Focus();
                                        tb.Select(0, tb.Text.Length);
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }


        public static DataGridCell SelectCellByIndex(DataGrid dataGrid, int rowIndex, int columnIndex)
            {
            if (!dataGrid.SelectionUnit.Equals(DataGridSelectionUnit.Cell))
                throw new ArgumentException("The SelectionUnit of the DataGrid must be set to Cell.");
        
            if (rowIndex < 0 || rowIndex > (dataGrid.Items.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid row index.", rowIndex));

            if (columnIndex < 0 || columnIndex > (dataGrid.Columns.Count - 1))
                throw new ArgumentException(string.Format("{0} is an invalid column index.", columnIndex));

            dataGrid.SelectedCells.Clear();
            object item = dataGrid.Items[rowIndex]; //=Product X

            if (!(dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) is DataGridRow row))
            {
                dataGrid.ScrollIntoView(item);
                row = dataGrid.ItemContainerGenerator.ContainerFromIndex(rowIndex) as DataGridRow;
            }

            DataGridCell cell = null;
            if (row != null)
            {
                cell = GetCell(dataGrid, row, columnIndex);
                if (cell != null)
                {
                    DataGridCellInfo dataGridCellInfo = new DataGridCellInfo(cell);
                    dataGrid.SelectedCells.Add(dataGridCellInfo);
                    cell.Focus();                  
                }
            }
            return cell;
        }

        public static DataGridCell GetCell(DataGrid dataGrid, DataGridRow rowContainer, int column)
        {
            if (rowContainer != null)
            {
                DataGridCellsPresenter presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                if (presenter == null)
                {

                    /* if the row has been virtualized away, call its ApplyTemplate() method 

                     * to build its visual tree in order for the DataGridCellsPresenter

                     * and the DataGridCells to be created */
                    rowContainer.ApplyTemplate();
                    presenter = FindVisualChild<DataGridCellsPresenter>(rowContainer);
                }
                if (presenter != null)
                {
                    DataGridCell cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    if (cell == null)
                    {
                        /* bring the column into view
                         * in case it has been virtualized away */
                        dataGrid.ScrollIntoView(rowContainer, dataGrid.Columns[column]);
                        cell = presenter.ItemContainerGenerator.ContainerFromIndex(column) as DataGridCell;
                    }
                    return cell;
                }
            }
            return null;
        }

        public static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
        {
            if (obj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                    if (child != null && child is T)
                        return (T)child;
                    else
                    {
                        T childOfChild = FindVisualChild<T>(child);
                        if (childOfChild != null)
                            return childOfChild;
                    }
                }
            }
            return null;
        }

        public static T FindChildByIndex<T>(DependencyObject obj, int index) where T : DependencyObject
        {
            int locindex = 0;
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is T)
                {
                    if (locindex == index)
                        return (T)child;
                    else
                        locindex++;
                }
                else
                {
                    T childOfChild = FindChildByIndex<T>(child, index);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            //get parent item
            DependencyObject parentObject = VisualTreeHelper.GetParent(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            T parent = parentObject as T;
            if (parent != null)
                return parent;
            else
                return FindParent<T>(parentObject);
        }

    }

}
