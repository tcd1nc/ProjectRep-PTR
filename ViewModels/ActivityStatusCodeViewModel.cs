using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class ActivityStatusCodeViewModel : ViewModelBase
    {
        FullyObservableCollection<Models.ActivityStatusCodesModel> _activitystatuscodes;
        Models.ActivityStatusCodesModel _activitystatuscode;
        bool _isediting;

        public ActivityStatusCodeViewModel()
        {
            _activitystatuscodes = new FullyObservableCollection<Models.ActivityStatusCodesModel>();
            _activitystatuscodes = DatabaseQueries.GetActivityStatusCodes();
            //populate from database

            _activitystatuscode = new Models.ActivityStatusCodesModel();
            _isediting = true;
            _scrolltolastitem = false;
            ScrollToSelectedItem = 0;

        }


        public FullyObservableCollection<Models.ActivityStatusCodesModel> ActivityStatusCodes
        {
            get { return _activitystatuscodes; }
            set { SetField(ref _activitystatuscodes, value); }
        }

        public Models.ActivityStatusCodesModel ActivityStatusCode
        {
            get { return _activitystatuscode; }
            set {

                if (value != null)
                    SetField(ref _activitystatuscode, value); }
        }

        #region Commands

        ICommand _addnew;
        public ICommand AddNew
        {
            get
            {
                if (_addnew == null)
                    _addnew = new DelegateCommand(CanExecuteAddNew, ExecuteAddNew);
                return _addnew;
            }
        }

        bool _canexecuteadd = true;
        private bool CanExecuteAddNew(object obj)
        {
            string error = (_activitystatuscode as IDataErrorInfo)["Description"];
            if (!string.IsNullOrEmpty(error))
                return false;
            string error2 = (_activitystatuscode as IDataErrorInfo)["Status"];
            if (!string.IsNullOrEmpty(error2))
                return false;

            return _canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            _canexecuteadd = false;

            ActivityStatusCodes.Add(new Models.ActivityStatusCodesModel());
            ScrollToLastItem = true;
            ActivityStatusCodesListEnabled = false;
        }

        public bool ActivityStatusCodesListEnabled
        {
            get { return _isediting; }
            set { SetField(ref _isediting, value); }
        }

        bool _scrolltolastitem;
        public bool ScrollToLastItem
        {
            get { return _scrolltolastitem; }
            set { SetField(ref _scrolltolastitem, value); }
        }

        int _scrolltoselecteditem;
        public int ScrollToSelectedItem
        {
            get { return _scrolltoselecteditem; }
            set { SetField(ref _scrolltoselecteditem, value); }
        }


        //save
        ICommand _cancel;
        public ICommand Cancel
        {
            get
            {
                if (_cancel == null)
                    _cancel = new DelegateCommand(CanExecute, ExecuteCancel);
                return _cancel;
            }
        }

        private void ExecuteCancel(object parameter)
        {
            _canexecuteadd = true;
            CloseWindow();
        }

        bool _canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
            string error = (_activitystatuscode as IDataErrorInfo)["Description"];
            if (!string.IsNullOrEmpty(error))
                return false;
            string error2 = (_activitystatuscode as IDataErrorInfo)["Status"];
            if (!string.IsNullOrEmpty(error2))
                return false;
            return _canexecutesave;
        }

        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecuteSave, ExecuteSaveAndClose);
                return _saveandclose;
            }
        }

        private void ExecuteSaveAndClose(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();
            _canexecuteadd = true;

            if (!_isediting)
            {
                Models.ActivityStatusCodesModel _newactivitystatuscode = new Models.ActivityStatusCodesModel();
                _newactivitystatuscode.GOM.Description = ActivityStatusCode.GOM.Description;

                DatabaseQueries.AddNewActivityStatusCode(_newactivitystatuscode);
            }
            else
            {
                DatabaseQueries.UpdateActivityStatusCode(_activitystatuscode);
            }
            CloseWindow();
        }


        #endregion


    }
}
