using System;
using System.Linq;
using static PTR.DatabaseQueries;
using PTR.Models;
using System.Collections.ObjectModel;

namespace PTR.TV
{
    public class TreeViewViewModel :ViewModelBase
    {

        public TreeViewViewModel()
        {
            BuildPartialTree();
        }
        
        FullyObservableCollection<TreeViewNodeModel> nodes;
        FullyObservableCollection<TreeViewNodeModel> partialtreenodes;

        public FullyObservableCollection<TreeViewNodeModel> Nodes
        {
            get { return nodes; }
            set { SetField(ref nodes, value); }
        }

        private void BuildPartialTree()
        {
            FullyObservableCollection<TreeViewNodeModel> opconodes = GetTVOperatingCompanies();
            FullyObservableCollection<TreeViewNodeModel> countrynodes = GetTVCountries(0);
            FullyObservableCollection<TreeViewNodeModel> salesregionnodes = GetTVSalesRegions(0);

            foreach (TreeViewNodeModel opconode in opconodes)
            {
                var countries = (from node in countrynodes where node.ParentID == opconode.ID select node).ToList();
                opconode.Children = new FullyObservableCollection<TreeViewNodeModel>();
                foreach (TreeViewNodeModel countrynode in countries)
                {
                    opconode.Children.Add(countrynode);
                    var salesregions = (from node in salesregionnodes where node.ParentID == countrynode.ID select node).ToList();
                    countrynode.Children = new FullyObservableCollection<TreeViewNodeModel>();
                    foreach (TreeViewNodeModel salesregionnode in salesregions)
                    {
                        countrynode.Children.Add(salesregionnode);
                    }
                    countrynode.Children.ItemPropertyChanged += SalesRegion_ItemPropertyChanged;
                }
                opconode.Children.ItemPropertyChanged += Country_ItemPropertyChanged;
            }
            partialtreenodes = opconodes;
            partialtreenodes.ItemPropertyChanged += Nodes_ItemPropertyChanged;
        }

        FullyObservableCollection<TreeViewNodeModel> customernodes;
        public FullyObservableCollection<TreeViewNodeModel> CustomerNodes
        {
            get { return customernodes; }
            set { SetField(ref customernodes, value); }
        }

        public void LoadTree(int userid)
        {
            CustomerNodes = GetTVCustomers(userid);
            foreach (TreeViewNodeModel opconode in partialtreenodes)
            {
                opconode.IsChecked = false;
                foreach (TreeViewNodeModel countrynode in opconode.Children)
                {
                    countrynode.IsChecked = false;
                    foreach (TreeViewNodeModel salesregionnode in countrynode.Children)
                    {
                        salesregionnode.IsChecked = false;
                        salesregionnode.Children = new FullyObservableCollection<TreeViewNodeModel>();
                        salesregionnode.Children.ItemPropertyChanged += Customer_ItemPropertyChanged; 
                        var customers = (from node in CustomerNodes where node.ParentID == salesregionnode.ID select node).ToList();
                        foreach (TreeViewNodeModel customer in customers)
                        {                            
                            salesregionnode.Children.Add(customer);                             
                        }                     
                    }
                }
            }
            Nodes = partialtreenodes;
        }
                
        public void DestroyEventHandlers()
        {
            if (Nodes != null)
            {
                foreach (TreeViewNodeModel opconode in Nodes)
                {
                    foreach (TreeViewNodeModel countrynode in opconode.Children)
                    {
                        foreach (TreeViewNodeModel salesregionnode in countrynode.Children)
                        {
                            salesregionnode.Children.ItemPropertyChanged -= Customer_ItemPropertyChanged;
                        }
                        countrynode.Children.ItemPropertyChanged -= SalesRegion_ItemPropertyChanged;
                    }
                    opconode.Children.ItemPropertyChanged -= Country_ItemPropertyChanged;
                }
                Nodes.ItemPropertyChanged -= Nodes_ItemPropertyChanged;
            }
        }
        
        private void Nodes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            Collection<int> custnodes = new Collection<int>();
            foreach (TreeViewNodeModel country in (sender as FullyObservableCollection<TreeViewNodeModel>)[e.CollectionIndex].Children)
            {                
                foreach (TreeViewNodeModel salesregion in country.Children)
                    foreach(TreeViewNodeModel customer in salesregion.Children)
                        custnodes.Add(customer.ID);
            }
        }

        private void Country_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            Collection<int> custnodes = new Collection<int>();            
            foreach (TreeViewNodeModel salesregion in (sender as FullyObservableCollection<TreeViewNodeModel>)[e.CollectionIndex].Children)
            {
                salesregion.IsChecked = (sender as FullyObservableCollection<TreeViewNodeModel>)[e.CollectionIndex].IsChecked;              
            }          
        }

        private void SalesRegion_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {                     
            foreach (TreeViewNodeModel customer in (sender as FullyObservableCollection<TreeViewNodeModel>)[e.CollectionIndex].Children)
            {              
                customer.IsChecked = (sender as FullyObservableCollection<TreeViewNodeModel>)[e.CollectionIndex].IsChecked;                      
            }                   
        }

        private void Customer_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            RaiseNodeChangeEvent((sender as FullyObservableCollection<TreeViewNodeModel>)[e.CollectionIndex].ID);
        }

        public delegate void CustomerNodeChangeEventHandler(object sender, NodeChangeEventArgs e);
        public event CustomerNodeChangeEventHandler NodeChangeEvent;
        protected virtual void RaiseNodeChangeEvent(int nodeid)
        {
            // Raise the event by using the () operator.
            NodeChangeEvent?.Invoke(this, new NodeChangeEventArgs(nodeid));
        }
        public class NodeChangeEventArgs
        {
            public NodeChangeEventArgs(int s) { ID = s; }
            public int ID { get; } // readonly
        }
        
      
    }

    public class CustomerTreeViewViewModel : ViewModelBase
    {              
        private FullyObservableCollection<TreeViewNodeModel> partialtreenodes;

        public CustomerTreeViewViewModel()
        {           
            BuildPartialTree();
        }

        FullyObservableCollection<TreeViewNodeModel> nodes;
        public FullyObservableCollection<TreeViewNodeModel> Nodes
        {
            get { return nodes; }
            set { SetField(ref nodes, value); }
        }

        private void BuildPartialTree()
        {
            FullyObservableCollection<TreeViewNodeModel> opconodes = GetAllTVOperatingCompanies();
            FullyObservableCollection<TreeViewNodeModel> countrynodes = GetAllTVCountries();
            FullyObservableCollection<TreeViewNodeModel> salesregionnodes = GetAllTVSalesRegions();

            foreach (TreeViewNodeModel opconode in opconodes)
            {
                var countries = (from node in countrynodes where node.ParentID == opconode.ID select node).ToList();
                opconode.Children = new FullyObservableCollection<TreeViewNodeModel>();
                foreach (TreeViewNodeModel countrynode in countries)
                {
                    opconode.Children.Add(countrynode);                               
                    var salesregions = (from node in salesregionnodes where node.ParentID == countrynode.ID select node).ToList();
                    countrynode.Children = new FullyObservableCollection<TreeViewNodeModel>();
                    foreach (TreeViewNodeModel salesregionnode in salesregions)
                    {
                        countrynode.Children.Add(salesregionnode);                        
                    }
                }                                    
            }
            partialtreenodes = opconodes;
        }

        public void LoadCustomerTree()
        {
            FullyObservableCollection<TreeViewNodeModel> customernodes = GetAllTVCustomers();
            foreach (TreeViewNodeModel opconode in partialtreenodes)
            {                       
                foreach (TreeViewNodeModel countrynode in opconode.Children)
                {                    
                    foreach (TreeViewNodeModel salesregionnode in countrynode.Children)
                    {
                        salesregionnode.Children = new FullyObservableCollection<TreeViewNodeModel>();
                        var customers = (from node in customernodes where node.ParentID == salesregionnode.ID select node).ToList();
                        foreach (TreeViewNodeModel customer in customers)
                        {
                            salesregionnode.Children.Add(customer);
                        }                      
                    }
                }
            }
            Nodes = partialtreenodes;
        }

    }

    public class MenuTreeViewViewModel : ViewModelBase
    {
        public MenuTreeViewViewModel() { }

        FullyObservableCollection<Models.TreeViewNodeModel> nodes;
        public FullyObservableCollection<Models.TreeViewNodeModel> Nodes
        {
            get { return nodes; }
            set { SetField(ref nodes, value); }
        }

        public void LoadMenuTree()
        {
            Nodes?.Clear();
            FullyObservableCollection<Models.TreeViewNodeModel> opconodes = new FullyObservableCollection<Models.TreeViewNodeModel>();
            opconodes = GetTVOperatingCompanies();

            foreach (Models.TreeViewNodeModel opconode in opconodes)
            {
                opconode.Children = GetTVCountries(opconode.ID);
                foreach (Models.TreeViewNodeModel countrynode in opconode.Children)
                {
                    countrynode.Children = GetTVSalesRegions(countrynode.ID);
                    if(countrynode.Children.Count>0)
                        countrynode.Children.ItemPropertyChanged += SalesRegion_ItemPropertyChanged;
                }
                if(opconode.Children.Count>0)
                    opconode.Children.ItemPropertyChanged += Country_ItemPropertyChanged;
            }
            Nodes = opconodes;
            if(Nodes.Count>0)
                Nodes.ItemPropertyChanged += Nodes_ItemPropertyChanged;
        }

        private void Nodes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            RaiseNodeChangeEvent("OPCO");
        }

        private void Country_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            foreach (Models.TreeViewNodeModel n in (sender as FullyObservableCollection<Models.TreeViewNodeModel>)[e.CollectionIndex].Children)
                n.IsChecked = (sender as FullyObservableCollection<Models.TreeViewNodeModel>)[e.CollectionIndex].IsChecked;

            RaiseNodeChangeEvent("Country");
        }

        private void SalesRegion_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            foreach (Models.TreeViewNodeModel n in (sender as FullyObservableCollection<Models.TreeViewNodeModel>)[e.CollectionIndex].Children)
                n.IsChecked = (sender as FullyObservableCollection<Models.TreeViewNodeModel>)[e.CollectionIndex].IsChecked;

            RaiseNodeChangeEvent("Sales Region");
        }

        public delegate void CustomerNodeChangeEventHandler(object sender, NodeChangeEventArgs e);
        public event CustomerNodeChangeEventHandler NodeChangeEvent;
        protected virtual void RaiseNodeChangeEvent(string nodename)
        {
            // Raise the event by using the () operator.
            NodeChangeEvent?.Invoke(this, new NodeChangeEventArgs(nodename));
        }
        public class NodeChangeEventArgs
        {
            public NodeChangeEventArgs(string s) { Text = s; }
            public String Text { get; } // readonly
        }

        public void DestroyEventHandlers()
        {
            if (Nodes != null)
            {
                foreach (Models.TreeViewNodeModel opconode in Nodes)
                {
                    foreach (Models.TreeViewNodeModel countrynode in opconode.Children)
                    {
                        countrynode.Children.ItemPropertyChanged -= SalesRegion_ItemPropertyChanged;
                    }
                    opconode.Children.ItemPropertyChanged -= Country_ItemPropertyChanged;
                }
                Nodes.ItemPropertyChanged -= Nodes_ItemPropertyChanged;
            }
        }
    }

}
