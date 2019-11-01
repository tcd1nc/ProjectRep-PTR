using System;
using PTR.TVViewModels;
//using AssetManager.DragDrop;

namespace PTR.TVViewModels
{
    public class TVOPCOViewModel : TreeViewItemViewModel //, IDropable
    {
        Models.OperatingCompanyModel _opco;

        public TVOPCOViewModel(Models.OperatingCompanyModel _parentopco) : base(null, true)
        {
            OPCO = _parentopco;
            IsExpanded = false;
            IsSelected = false;
        }

        protected override void LoadChildren()
        {
            FullyObservableCollection<TVAssetViewModel> _countries = DatabaseQueries.GetTVCountries(OPCO.ID);
            foreach (TVAssetViewModel cm in _countries)
            {
                base.Children.Add(new TVAssetViewModel(cm, null));
            }
        }
        
        public Models.OperatingCompanyModel OPCO
        {
            get { return _opco; }
            set { SetField(ref _opco, value); }
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
        //    TVAssetViewModel source = data as TVAssetViewModel;
        //    if (source != null)
        //    {
        //        if (source.Asset.ParentAssetID == 0 && Customer.ID == source.Asset.CustomerID) //if dragged and dropped yourself, don't need to do anything
        //            return;

        //        DatabaseQueries.UpdateParentAssetID(source.Asset.AssetID, 0, Customer.ID);
        //        ViewModels.AssetTreeExViewModel.MoveAsset(source.Asset.AssetID, 0, Customer.ID);

        //    }
        //}

        #endregion

    }
}
