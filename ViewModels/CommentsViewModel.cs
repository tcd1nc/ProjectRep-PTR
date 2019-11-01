using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class CommentsViewModel :ViewModelBase
    {
        FullyObservableCollection<Models.CommentsModel> _comments = new FullyObservableCollection<Models.CommentsModel>();


        public CommentsViewModel(FullyObservableCollection<Models.CommentsModel> _comments)
        {
            Comments = _comments;
            
        }

        public FullyObservableCollection<Models.CommentsModel> Comments
        {
            get { return _comments; }
            set { SetField(ref _comments, value); }
        }

        bool? _blnsave;
        public bool? SaveFlag
        {
            get { return _blnsave; }
            set { SetField(ref _blnsave, value); }
        }

        #region Commands

        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecute, ExecuteClose);
                return _saveandclose;
            }
        }

        private void ExecuteClose(object parameter)
        {
            SaveFlag = true;
            CloseWindow();
        }

        ICommand _cancelandclose;
        public ICommand Cancel
        {
            get
            {
                if (_cancelandclose == null)
                    _cancelandclose = new DelegateCommand(CanExecute, ExecuteCancelAndClose);
                return _cancelandclose;
            }
        }

        private void ExecuteCancelAndClose(object parameter)
        {
            SaveFlag = false;         
            CloseWindow();
        }

        #endregion
    }

}
