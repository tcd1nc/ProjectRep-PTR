using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Data;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System;

namespace PTR
{
    
    public class MouseDoubleClick
    {
        public static DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(MouseDoubleClick), new UIPropertyMetadata(CommandChanged));
        public static DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(MouseDoubleClick), new UIPropertyMetadata(null));

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Control control)
            {
                if ((e.NewValue != null) && (e.OldValue == null))                
                    control.MouseDoubleClick += OnMouseDoubleClick;                
                else if ((e.NewValue == null) && (e.OldValue != null))                
                    control.MouseDoubleClick -= OnMouseDoubleClick;                
            }
        }

        private static void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(CommandProperty);
            object commandParameter = control.GetValue(CommandParameterProperty);

            if (sender is TreeViewItem)
            {                
                if (!((TreeViewItem)sender).IsSelected)
                    return;
            }

            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
                e.Handled = true;
            }
        }
    }

    public class TVNodeSelected
    {
        public static DependencyProperty TreeViewItemSelectedProperty = DependencyProperty.RegisterAttached("TreeViewItemSelected", typeof(ICommand),
          typeof(TVNodeSelected), new UIPropertyMetadata(TreeViewItemSelectedChanged));

        public static DependencyProperty TVSelectedCommandParameterProperty = DependencyProperty.RegisterAttached("TVSelectedCommandParameter", typeof(object), typeof(TVNodeSelected), new UIPropertyMetadata(null));
        
        public static object GetTreeViewItemSelected(DependencyObject dependencyObject)
        {
            return (object)dependencyObject.GetValue(TreeViewItemSelectedProperty);
        }

        public static void SetTreeViewItemSelected(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(TreeViewItemSelectedProperty, value);
        }

        public static void SetTVSelectedCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(TVSelectedCommandParameterProperty, value);
        }

        public static object GetTVSelectedCommandParameter(DependencyObject target)
        {
            return target.GetValue(TVSelectedCommandParameterProperty);
        }               

        private static void TreeViewItemSelectedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (dependencyObject is TreeViewItem control)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                    control.Selected += Control_Selected;
                else if ((e.NewValue == null) && (e.OldValue != null))
                    control.Selected -= Control_Selected;
            }
        }

        private static void Control_Selected(object sender, RoutedEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(TreeViewItemSelectedProperty);
            object commandParameter = control.GetValue(TVSelectedCommandParameterProperty);           
      
            if (sender is TreeViewItem)
            {
                if (!((TreeViewItem)sender).IsSelected)
                    return;
            }            

            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
                e.Handled = true;
            }          
        }
            
        public static DependencyProperty TVIsSenderParameterProperty = DependencyProperty.RegisterAttached("TVIsSenderParameter", typeof(bool), typeof(TVNodeSelected), new UIPropertyMetadata(false));

        public static void SetTVIsSenderParameter(DependencyObject target, bool value)
        {
            target.SetValue(TVIsSenderParameterProperty, value);
        }
        public static bool GetTVIsSenderParameter(DependencyObject target)
        {
            return (bool)target.GetValue(TVIsSenderParameterProperty);
        }

     
    }
    
    public static class DialogCloser
    {
        public static readonly DependencyProperty DialogResultProperty = DependencyProperty.RegisterAttached("DialogResult", typeof(bool?),
                typeof(DialogCloser), new PropertyMetadata(DialogResultChanged));

        private static void DialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.DialogResult = e.NewValue as bool?;
        }
        public static void SetDialogResult(Window target, bool? value)
        {
            target.SetValue(DialogResultProperty, value);
        }

        public static readonly DependencyProperty NumericDialogResultProperty = DependencyProperty.RegisterAttached("NumericDialogResult", typeof(double),
                typeof(DialogCloser), new PropertyMetadata(NumericDialogResultChanged));

        private static void NumericDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.Tag = (double)e.NewValue;
        }
        public static void SetNumericDialogResult(Window target, double value)
        {
            target.SetValue(NumericDialogResultProperty, value);
        }


        public static readonly DependencyProperty DialogCloseProperty = DependencyProperty.RegisterAttached("DialogClose", typeof(bool?),
                typeof(DialogCloser), new PropertyMetadata(false, DialogCloseChanged));

        private static void DialogCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window && (e.NewValue as bool?) == true)
                window.Close();
        }
        public static void SetDialogClose(Window target, bool? value)
        {
            target.SetValue(DialogCloseProperty, value);
        }

        public static readonly DependencyProperty ObjectDialogResultProperty = DependencyProperty.RegisterAttached("ObjectDialogResult", typeof(object),
               typeof(DialogCloser), new PropertyMetadata(ObjectDialogResultChanged));

        private static void ObjectDialogResultChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is Window window)
                window.Tag = e.NewValue;
        }
        public static void SetObjectDialogResult(Window target, double value)
        {
            target.SetValue(ObjectDialogResultProperty, value);
        }
    }
    
    public static class Commands
    {
        public static readonly DependencyProperty DataGridRowClickProperty =
          DependencyProperty.RegisterAttached("DataGridRowClickCommand", typeof(ICommand), typeof(Commands),
                            new PropertyMetadata(new PropertyChangedCallback(AttachOrRemoveDataGridRowClickEvent)));

        public static ICommand GetDataGridRowClickCommand(DependencyObject obj)
        {
            return (ICommand)obj.GetValue(DataGridRowClickProperty);
        }

        public static void SetDataGridRowClickCommand(DependencyObject obj, ICommand value)
        {
            obj.SetValue(DataGridRowClickProperty, value);
        }

        public static void AttachOrRemoveDataGridRowClickEvent(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (obj is DataGrid dataGrid)
            {
                ICommand cmd = (ICommand)args.NewValue;

                if (args.OldValue == null && args.NewValue != null)
                {
                    dataGrid.MouseLeftButtonUp += ExecuteDataGridRowClick;
                }
                else if (args.OldValue != null && args.NewValue == null)
                {
                    dataGrid.MouseLeftButtonUp -= ExecuteDataGridRowClick;
                }
            }
        }

        private static void ExecuteDataGridRowClick(object sender, MouseButtonEventArgs args)
        {
            DependencyObject obj = sender as DependencyObject;
            ICommand cmd = (ICommand)obj.GetValue(DataGridRowClickProperty);

            var row = (sender as DataGrid).SelectedItem;
            if(row.GetType().Equals(typeof(DataRowView)))                         
            {
                if (cmd != null)
                {
                    if (cmd.CanExecute(row))
                        cmd.Execute(row);
                }
            }
        }
    }

    public static class MouseDataGridCellMove
    {
        public static DependencyProperty MouseGridCellMoveProperty = DependencyProperty.RegisterAttached("MouseGridCellMove", typeof(ICommand), typeof(MouseDataGridCellMove), new UIPropertyMetadata(MouseMovedChanged));
        
        public static void SetMouseGridCellMove(DependencyObject target, RoutedEvent value)
        {
            target.SetValue(MouseGridCellMoveProperty, value);
        }

        private static void MouseMovedChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Control control)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                {
                    control.MouseMove += OnMouseMove;
                }
                else if ((e.NewValue == null) && (e.OldValue != null))
                {
                    control.MouseMove -= OnMouseMove;
                }
            }
        }

        private static void OnMouseMove(object sender, MouseEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is DataGridCell) && !(dep is DataGridColumnHeader))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            int rowindex = -1;
            int colindex = -1;
            if (dep == null)
            {
          //      (DataContext as ViewModels.ReportViewModel).CurrentRowIndex = -1;
            //    (DataContext as ViewModels.ReportViewModel).CurrentColumnIndex = -1;
            };
            if (dep is DataGridColumnHeader)
            {
           //     (DataContext as ViewModels.ReportViewModel).CurrentRowIndex = -1;
           //     (DataContext as ViewModels.ReportViewModel).CurrentColumnIndex = -1;
            };
            if (dep is DataGridCell)
            {
                colindex = ((DataGridCell)dep).Column.DisplayIndex;
                DataGridCell cell = dep as DataGridCell;
                while ((dep != null) && !(dep is DataGridRow))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }
                DataGridRow row = dep as DataGridRow;
                rowindex = FindRowIndex(row);

            //    (DataContext as ViewModels.ReportViewModel).CurrentRowIndex = rowindex;
              //  (DataContext as ViewModels.ReportViewModel).CurrentColumnIndex = colindex;
            }
          //  e.Handled = true;

            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(MouseGridCellMoveProperty);

            e.Handled = true;
            command.Execute(rowindex.ToString() + "," + colindex.ToString());
        }
        private static int FindRowIndex(DataGridRow row)
        {
            DataGrid dataGrid = ItemsControl.ItemsControlFromItemContainer(row) as DataGrid;
            int index = dataGrid.ItemContainerGenerator.IndexFromContainer(row);
            return index;
        }
    }
    
    public static class FocusAdvancement
    {
        public static bool GetAdvancesByEnterKey(DependencyObject obj)
        {
            return (bool)obj.GetValue(AdvancesByEnterKeyProperty);
        }

        public static void SetAdvancesByEnterKey(DependencyObject obj, bool value)
        {
            obj.SetValue(AdvancesByEnterKeyProperty, value);
        }

        public static readonly DependencyProperty AdvancesByEnterKeyProperty =
            DependencyProperty.RegisterAttached("AdvancesByEnterKey", typeof(bool), typeof(FocusAdvancement),
            new UIPropertyMetadata(OnAdvancesByEnterKeyPropertyChanged));

        static void OnAdvancesByEnterKeyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UIElement element = d as UIElement;
            if (element == null) return;

            if ((bool)e.NewValue)
                element.KeyDown += Keydown;
            else
                element.KeyDown -= Keydown;
        }

        static void Keydown(object sender, KeyEventArgs e)
        {
            if (!e.Key.Equals(Key.Enter)) return;

            if (sender is UIElement element)
                element.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
    }
       
    //obtain reference to current view for messagebox owner
    public static class MyDependencyProperty
    {
        public static readonly DependencyProperty ReferenceProperty = DependencyProperty.RegisterAttached("Reference", typeof(object), typeof(MyDependencyProperty), new FrameworkPropertyMetadata("",FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, ReferenceChanged));
        public static void SetReference(DependencyObject target, object value)
        {
            target.SetValue(ReferenceProperty, value);
        }      

        private static void ReferenceChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Window window)
            {
                if (e.NewValue == null)
                    window.Loaded += OnWindowLoaded;
                else
                    if (e.NewValue != null)
                    window.Loaded -= OnWindowLoaded;
            }
        }

        private static void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Window window = sender as Window;
            SetReference(window, window);
        }

    }
    
    public class WindowBehavior
    {
        public static DependencyProperty WindowClosingProperty = DependencyProperty.RegisterAttached("WindowClosing", typeof(ICommand), typeof(WindowBehavior), new PropertyMetadata(WindowClosing));

        public static void SetWindowClosing(DependencyObject target, ICommand value)
        {
            target.SetValue(WindowClosingProperty, value);
        }
                   
        private static void WindowClosing(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is Window control)
            {
                if ((e.NewValue != null) && (e.OldValue == null))
                    control.Closing += Control_Closing;
                else if ((e.NewValue == null) && (e.OldValue != null))
                    control.Closing -= Control_Closing;
            }
        }

        private static void Control_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Control control = sender as Control;
            ICommand command = (ICommand)control.GetValue(WindowClosingProperty);                     
            if (command.CanExecute(null))
            {
                command.Execute(null);                
            }
        }
        
    }
}


