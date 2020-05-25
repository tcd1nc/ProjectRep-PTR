using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace PTR
{
    public static class ItemsControlHelper
    {
        public static readonly DependencyProperty ScrollToLastItemProperty =  DependencyProperty.RegisterAttached("ScrollToLastItem",
                typeof(bool), typeof(ItemsControlHelper), new FrameworkPropertyMetadata(false, OnScrollToLastItemChanged));

        public static void SetScrollToLastItem(UIElement sender, bool value)
        {
            sender.SetValue(ScrollToLastItemProperty, value);
        }

        public static bool GetScrollToLastItem(UIElement sender)
        {
            return (bool)sender.GetValue(ScrollToLastItemProperty);
        }

        private static void OnScrollToLastItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
                itemsControl.ItemContainerGenerator.StatusChanged += (s, a) => OnItemsChanged(itemsControl, s, a);
        }

        static void OnItemsChanged(ItemsControl itemsControl, object sender, EventArgs e)
        {
            var generator = sender as ItemContainerGenerator;
            if (generator.Status == GeneratorStatus.ContainersGenerated)
            {
                if (itemsControl.Items.Count > 0)                
                    ScrollIntoView(itemsControl, itemsControl.Items[itemsControl.Items.Count - 1]);                
            }
        }

        private static void ScrollIntoView(ItemsControl itemsControl, object item)
        {
            if (itemsControl.ItemContainerGenerator.Status == GeneratorStatus.ContainersGenerated)            
                OnBringItemIntoView(itemsControl, item);            
            else
            {
                Func<object, object> onBringIntoView = (o) => OnBringItemIntoView(itemsControl, item);
                itemsControl.Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                      new DispatcherOperationCallback(onBringIntoView));
            }
        }

        private static object OnBringItemIntoView(ItemsControl itemsControl, object item)
        {
            var element = itemsControl.ItemContainerGenerator.ContainerFromItem(item) as FrameworkElement;
            //if (element != null)
           // {
                element?.BringIntoView();
            //}
            return null;
        }
    }


    public static class LBScrollHelper
    {
        public static readonly DependencyProperty ScrollToLastItemProperty = DependencyProperty.RegisterAttached("ScrollToLastItem",
                typeof(bool), typeof(LBScrollHelper), new FrameworkPropertyMetadata(false, OnScrollToLastItemChanged));

        public static void SetScrollToLastItem(UIElement sender, bool value)
        {
            sender.SetValue(ScrollToLastItemProperty, value);
        }

        public static bool GetScrollToLastItem(UIElement sender)
        {
            return (bool)sender.GetValue(ScrollToLastItemProperty);
        }

        private static void OnScrollToLastItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)
                MoveToLastItem(itemsControl);
        }

        static void MoveToLastItem(ItemsControl itemsControl)
        {
            (itemsControl as ListBox).SelectedItem = (itemsControl as ListBox).Items[itemsControl.Items.Count - 1];
            (itemsControl as ListBox).SelectedIndex = itemsControl.Items.Count - 1;
            (itemsControl as ListBox).ScrollIntoView(itemsControl.Items[itemsControl.Items.Count - 1]);
        }


        public static readonly DependencyProperty ScrollToSelectedItemProperty = DependencyProperty.RegisterAttached("ScrollToSelectedItem",
                typeof(int), typeof(LBScrollHelper), new FrameworkPropertyMetadata(-1, OnScrollToSelectedItemChanged));

        public static void SetScrollToSelectedItem(UIElement sender, int value)
        {
            sender.SetValue(ScrollToSelectedItemProperty, value);
        }

        public static int GetScrollToSelectedItem(UIElement sender)
        {
            return (int)sender.GetValue(ScrollToSelectedItemProperty);
        }

        private static void OnScrollToSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is ItemsControl itemsControl)            
                if (itemsControl.Items.Count > 0)
                    MoveToSelectedItem(itemsControl, (int)e.NewValue);
            
        }

        static void MoveToSelectedItem(ItemsControl itemsControl, int index)
        {
            (itemsControl as ListBox).SelectedItem = (itemsControl as ListBox).Items[index];
            (itemsControl as ListBox).SelectedIndex = index;
            (itemsControl as ListBox).ScrollIntoView(itemsControl.Items[index]);          
        }

    }


    //public static class GridScrollHelper
    //{
    //    public static readonly DependencyProperty ScrollToLastItemProperty = DependencyProperty.RegisterAttached("ScrollToLastItem",
    //            typeof(bool), typeof(GridScrollHelper), new FrameworkPropertyMetadata(false, OnScrollToLastItemChanged));

    //    public static void SetScrollToLastItem(UIElement sender, bool value)
    //    {
    //        sender.SetValue(ScrollToLastItemProperty, value);
    //    }

    //    public static bool GetScrollToLastItem(UIElement sender)
    //    {
    //        return (bool)sender.GetValue(ScrollToLastItemProperty);            
    //    }

    //    private static void OnScrollToLastItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (sender is ExtendedGrid itemsControl)
    //            MoveToLastItem(itemsControl);
    //    }

    //    static void MoveToLastItem(ExtendedGrid itemsControl)
    //    {
    //        (itemsControl as ExtendedGrid).SelectedItem = (itemsControl as ExtendedGrid).Items[itemsControl.Items.Count - 1];
    //        (itemsControl as ExtendedGrid).SelectedIndex = itemsControl.Items.Count - 1;
    //        (itemsControl as ExtendedGrid).ScrollIntoView(itemsControl.Items[itemsControl.Items.Count - 1]);            
    //    }


    //    public static readonly DependencyProperty ScrollToSelectedItemProperty = DependencyProperty.RegisterAttached("ScrollToSelectedItem",
    //            typeof(int), typeof(GridScrollHelper), new FrameworkPropertyMetadata(-1, OnScrollToSelectedItemChanged));

    //    public static void SetScrollToSelectedItem(UIElement sender, int value)
    //    {
    //        sender.SetValue(ScrollToSelectedItemProperty, value);
    //    }

    //    public static int GetScrollToSelectedItem(UIElement sender)
    //    {
    //        return (int)sender.GetValue(ScrollToSelectedItemProperty);
    //    }

    //    private static void OnScrollToSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (sender is ItemsControl itemsControl)            
    //            if (itemsControl.Items.Count > 0)
    //                MoveToSelectedItem(itemsControl, (int)e.NewValue);            
    //    }

    //    static void MoveToSelectedItem(ItemsControl itemsControl, int index)
    //    {
    //        (itemsControl as ExtendedGrid).SelectedItem = (itemsControl as ExtendedGrid).Items[index];
    //        (itemsControl as ExtendedGrid).SelectedIndex = index;
    //        (itemsControl as ExtendedGrid).ScrollIntoView(itemsControl.Items[index]);
    //    }

    //}

}
