using PTR.ViewModels;
using PTR.Models;
using System.Windows;
using System.Windows.Controls;

namespace PTR.Controls
{
    /// <summary>
    /// Interaction logic for ActivitiesControl.xaml
    /// </summary>
    public partial class ActivitiesControl : UserControl
    {
        public ActivitiesControl()
        {
            InitializeComponent();
            stats.DataContext = new ActivitiesViewModel();
            ((ActivitiesViewModel)(stats.DataContext)).CanSave += ActivitiesControl_CanSave;

           // Window w = (Window)this.Parent;
        }

	//outgoing event ... tell parent control if it is Ok to save
        private void ActivitiesControl_CanSave(bool param)
        {
            CanSave = param;
        }

        public ProjectReportSummary SelectedProjectItem
        {
            get { return (ProjectReportSummary)GetValue(SelectedProjectItemProperty); }
            set { SetValue(SelectedProjectItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedProjectItemProperty =
            DependencyProperty.Register("SelectedProjectItem", typeof(ProjectReportSummary), typeof(ActivitiesControl), new PropertyMetadata(null, ProjectChanged));

        private static void ProjectChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is ActivitiesControl ctrl)
            {
                ((ActivitiesViewModel)((ActivitiesControl)target).stats.DataContext).SelectedProjectItem = (ProjectReportSummary) e.NewValue;
            }
        }
        
        public static DependencyProperty UpdateMonthlyActivitiesProperty = DependencyProperty.RegisterAttached("UpdateMonthlyActivities", typeof(bool), 
            typeof(ActivitiesControl), new UIPropertyMetadata(false, CommandChanged));
       
        public static void SetUpdateMonthlyActivities(DependencyObject target, bool value)
        {
            target.SetValue(UpdateMonthlyActivitiesProperty, null);
        }

        public static bool GetUpdateMonthlyActivities(DependencyObject target)
        {
            return (bool)target.GetValue(UpdateMonthlyActivitiesProperty);
        }

        private static void CommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {            
            if((bool)e.NewValue == true)
            {
                ((ActivitiesViewModel)((ActivitiesControl)target).stats.DataContext).UpdateMonthlyActivities();
            }
        }
        
        public static DependencyProperty ClearActivitiesProperty = DependencyProperty.RegisterAttached("ClearActivities", typeof(bool), typeof(ActivitiesControl), 
            new UIPropertyMetadata(false, ClearCommandChanged));

        public static void SetClearActivities(DependencyObject target, bool value)
        {
            target.SetValue(ClearActivitiesProperty, value);
        }

        public static bool GetClearActivities(DependencyObject target)
        {
            return (bool)target.GetValue(ClearActivitiesProperty);
        }

        private static void ClearCommandChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == true)
            {
                ((ActivitiesViewModel)((ActivitiesControl)target).stats.DataContext).ClearActivities();
            }                       
        }


        public bool CanSave
        {
            get { return (bool)GetValue(CanSaveProperty); }
            set { SetValue(CanSaveProperty, value); }
        }

        public static readonly DependencyProperty CanSaveProperty =
            DependencyProperty.Register("CanSave", typeof(bool), typeof(ActivitiesControl), new UIPropertyMetadata(false));


    }
}

