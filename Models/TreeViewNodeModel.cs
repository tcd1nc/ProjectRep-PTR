namespace PTR.Models
{
    public class TreeViewNodeModel : ViewModelBase
    {
        int parentid;
        public int ParentID
        {
            get { return parentid; }
            set { SetField(ref parentid, value); }
        }

        int id;
        public int ID
        {
            get { return id; }
            set { SetField(ref id, value); }
        }
               
        string name;
        public string Name
        {
            get { return name; }
            set { SetField(ref name, value); }
        }

        bool ischecked;
        public bool IsChecked
        {
            get { return ischecked; }
            set { SetField(ref ischecked, value); }
        }

        bool isselected;
        public bool IsSelected
        {
            get { return isselected; }
            set { SetField(ref isselected, value); }
        }
        
        bool isexpanded;
        public bool IsExpanded
        {
            get { return isexpanded; }
            set { SetField(ref isexpanded, value); }
        }

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        int accessid;
        public int AccessID
        {
            get { return accessid; }
            set { SetField(ref accessid, value); }
        }

        bool isro;
        public bool IsROChecked
        {
            get { return isro; }
            set { SetField(ref isro, value); }
        }

        bool isfa;
        public bool IsFAChecked
        {
            get { return isfa; }
            set { SetField(ref isfa, value); }
        }

        bool iseditact;
        public bool IsEditActChecked
        {
            get { return iseditact; }
            set { SetField(ref iseditact, value); }
        }


        int nodetypeid=0;
        public int NodeTypeID
        {
            get { return nodetypeid; }
            set { SetField(ref nodetypeid, value); }
        }

        FullyObservableCollection<TreeViewNodeModel> children;
        public FullyObservableCollection<TreeViewNodeModel> Children
        {
            get { return children; }
            set { SetField(ref children, value); }
        }
        
    }
   
}
