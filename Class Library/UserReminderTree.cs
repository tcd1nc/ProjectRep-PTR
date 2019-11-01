using System.Windows;
using System.Windows.Controls;

namespace PTR.Models
{

    public class ClassTreeItem : ViewModelBase
    {
        public string Name { get; set; }
        public FullyObservableCollection<MaintenanceModel> Items { get; set; }
        public FullyObservableCollection<ClassTreeItem> SubItems { get; set; }
    }

    public class ToDoListTree : ViewModelBase
    {
        ViewModels.PTMainViewModel PTMVM;

        public ToDoListTree(ViewModels.PTMainViewModel ptmvm)
        {
            PTMVM = ptmvm;
            TDLTree = GetToDoListTree();
        }

        FullyObservableCollection<ClassTreeItem> todolisttree;
        public FullyObservableCollection<ClassTreeItem> TDLTree
        {
            get { return todolisttree; }
            set { SetField(ref todolisttree, value); }
        }

        private FullyObservableCollection<ClassTreeItem> GetToDoListTree()
        {
            FullyObservableCollection<ClassTreeItem> allItems = new FullyObservableCollection<ClassTreeItem>();

            ClassTreeItem newctiMissing = new ClassTreeItem
            {
                Name = "Missing EPs",
                Items = PTMVM.MissingEPs
            };

            ClassTreeItem newctiIncomplete = new ClassTreeItem
            {
                Name = "Incomplete EPs",
                Items = PTMVM.IncompleteEPs
            };

            ClassTreeItem newctiparent = new ClassTreeItem();
            if(newctiMissing.Items.Count > 0 && newctiIncomplete.Items.Count > 0)
            {                
                newctiparent.Name = "Evaluation Plans";
                newctiparent.SubItems = new FullyObservableCollection<ClassTreeItem>() { newctiMissing, newctiIncomplete };
            }
            else
            {
                if(newctiMissing.Items.Count > 0)
                {
                    newctiparent.Name = newctiMissing.Name;
                    newctiparent.Items = newctiMissing.Items;
                }
                else
                if(newctiIncomplete.Items.Count > 0)
                {
                    newctiparent.Name = newctiIncomplete.Name;
                    newctiparent.Items = newctiIncomplete.Items;
                }
            }

            if(!string.IsNullOrEmpty(newctiparent.Name))
                allItems.Add(newctiparent);

            ClassTreeItem reqcomp = new ClassTreeItem()
            {
                Name = "Requiring Completion",
                Items = PTMVM.RequiringCompletion
            };
            if (reqcomp.Items.Count > 0)
                allItems.Add(reqcomp);

            ClassTreeItem milestones = new ClassTreeItem()
            {
                Name = "Milestones Due",
                Items = PTMVM.MilestonesDue
            };
            if (milestones.Items.Count > 0)
                allItems.Add(milestones);

            ClassTreeItem activitydue = new ClassTreeItem()
            {
                Name = "Actions Required",
                Items = PTMVM.OverdueActivities
            };
            if (activitydue.Items.Count > 0)
                allItems.Add(activitydue);
            
            return allItems;
        }         
    }

    public class HierarchyDataTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            DataTemplate retval = null;
            FrameworkElement element = container as FrameworkElement;
            
            if (element != null && item != null && item is ClassTreeItem)
            {               
                ClassTreeItem hierarchyItem = item as ClassTreeItem;
                if (!string.IsNullOrEmpty(hierarchyItem.Name))
                {
                    if (hierarchyItem.Name == "Evaluation Plans")                                           
                        retval = element.FindResource("EPTemplate") as DataTemplate;                    
                    else                                          
                        retval = element.FindResource("MaintenanceTemplate") as DataTemplate;                    
                }                
            }
                       
            return retval;
        }
    }
}
