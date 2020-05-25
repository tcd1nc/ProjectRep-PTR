using PTR.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Data;

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
            ((ActivitiesViewModel)stats.DataContext).CanSave += ActivitiesControl_CanSave;
            ((ActivitiesViewModel)stats.DataContext).IsDirtyData += ActivitiesControl_IsDirtyData;
            ((ActivitiesViewModel)stats.DataContext).ProjectStatus += ActivitiesControl_ProjectStatus;           
        }              


        private void ActivitiesControl_ProjectStatus(object sender, ActivitiesViewModel.ProjectStatusEventArgs e)
        {
            ProjectStatus = e.ProjectStatus;
        }

        private void ActivitiesControl_IsDirtyData(object sender, ActivitiesViewModel.IsDirtyDataEventArgs e)
        {
            IsDirtyData = e.IsDirtyData;
        }

        private void ActivitiesControl_CanSave(object sender, ActivitiesViewModel.CanSaveEventArgs e)
        {
            CanSave = e.CanSave;
        }
              

        public static readonly DependencyProperty SelectedProjectItemProperty =
         DependencyProperty.RegisterAttached("SelectedProjectItem", typeof(DataRowView), typeof(ActivitiesControl),
             new FrameworkPropertyMetadata(null, ProjectChanged));

        public static void SetSelectedProjectItem(DependencyObject target, DataRowView value)
        {
            target.SetValue(SelectedProjectItemProperty, null);
        }

        public static DataRowView GetSelectedProjectItem(DependencyObject target)
        {
            return (DataRowView)target.GetValue(SelectedProjectItemProperty);
        }
               
        private static void ProjectChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            if (target is ActivitiesControl ctrl)
            {
                if(e.NewValue != null)
                    ((ActivitiesViewModel)((ActivitiesControl)target).stats.DataContext).SelectedProjectItem = (DataRowView) e.NewValue;
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

        public static readonly DependencyProperty CanSaveProperty = DependencyProperty.Register("CanSave", typeof(bool), typeof(ActivitiesControl), new UIPropertyMetadata(false));


        public bool IsDirtyData
        {
            get { return (bool)GetValue(IsDirtyDataProperty); }
            set { SetValue(IsDirtyDataProperty, value); }
        }

        public static readonly DependencyProperty IsDirtyDataProperty = DependencyProperty.Register("IsDirtyData", typeof(bool), typeof(ActivitiesControl), new UIPropertyMetadata(false));

        public ProjectStatusType ProjectStatus
        {
            get { return (ProjectStatusType)GetValue(ProjectStatusProperty); }
            set { SetValue(ProjectStatusProperty, value); }
        }

        public static readonly DependencyProperty ProjectStatusProperty =
            DependencyProperty.Register("ProjectStatus", typeof(ProjectStatusType), typeof(ActivitiesControl), new UIPropertyMetadata(ProjectStatusType.Active));





    }
}

