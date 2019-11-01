using System;
//using AssetManager.DragDrop;

namespace PTR.TVViewModels
{
    public class TVAssetViewModel : TreeViewItemViewModel //, IDropable, IDragable
    {
        Models.TreeViewNodeModel _asset;
     
        public TVAssetViewModel(TVAssetViewModel _parentasset) : base(_parentasset, true)
        {
            Asset = _parentasset.Asset;
        }

        public TVAssetViewModel(Models.TreeViewNodeModel _asset, TVAssetViewModel _assetvm) : base(_assetvm, true)
        {
            Asset = _asset;
            IsExpanded = false;
            IsSelected = false;
        }

        protected override void LoadChildren()
        {
            FullyObservableCollection<Models.TreeViewNodeModel> _assets = DatabaseQueries.GetChildAssets(Asset.AssetID);
            foreach (Models.TreeViewNodeModel am in _assets)
                base.Children.Add(new TVAssetViewModel(am, this));
        }

        public Models.TreeViewNodeModel Asset
        {
            get { return _asset; }
            set { SetField(ref _asset, value); }
        }


        //#region IDropable Members
        ///// <summary>
        ///// Only TVAssetViewModel can be dropped
        ///// </summary>
        //Type IDropable.DataType
        //{
        //    get { return typeof(TVAssetViewModel); }
        //}

        ///// <summary>
        ///// Drop data into this ViewModel
        ///// </summary>
        //void IDropable.Drop(object data, int index)
        //{
        //    //if moving within customer, reassign the children to the 
        //    //level above first
        //    TVAssetViewModel source = data as TVAssetViewModel;
           
        //    if (source != null)
        //    {
        //        if (source.Asset.AssetID == Asset.AssetID || source.Asset.ParentAssetID == Asset.AssetID) //if dragged and dropped yourself, don't need to do anything
        //            return;
        //        DatabaseQueries.UpdateParentAssetID(source.Asset.AssetID, Asset.AssetID, Asset.CustomerID);
        //        ViewModels.AssetTreeExViewModel.MoveAsset(source.Asset.AssetID, Asset.AssetID, Asset.CustomerID);
               
        //    }                     
        //}

        //#endregion

        //#region IDragable Members

        ///// <summary>
        ///// Only TVAssetViewModel can be dragged
        ///// </summary>
        //Type IDragable.DataType
        //{
        //    get
        //    {
        //        return typeof(TVAssetViewModel);
        //    }
        //}

        ///// <summary>
        ///// Remove this TVAssetViewModel from the 
        ///// TVAssetViewModel
        ///// </summary>
        ///// <param name="i"></param>
        //void IDragable.Remove(object data)
        //{

        //}

        //#endregion


    }
}
