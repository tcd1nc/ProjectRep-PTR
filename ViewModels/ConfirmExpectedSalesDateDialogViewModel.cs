using System;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class ConfirmExpectedSalesDateDialogViewModel : ViewModelBase
    {
        public ConfirmExpectedSalesDateDialogViewModel(DateTime? value)
        {                         
            EstDateFirstSales = (DateTime)value;
        }

        #region Properties


        DateTime? returnobject = null;
        public DateTime? ReturnObject
        {
            get { return returnobject; }
            set { SetField(ref returnobject, value); }
        }

        bool? blnsave;
        public bool? SaveFlag
        {
            get { return blnsave; }
            set { SetField(ref blnsave, value); }
        }

        DateTime? estdatefirstsales;
        public DateTime? EstDateFirstSales
        {
            get { return estdatefirstsales; }
            set { SetField(ref estdatefirstsales, value); }
        }

        #endregion

        #region Commands

        ICommand saveandclose;
        bool canexecutesave = true;
        private bool CanExecuteSave(object obj)
        {
            return canexecutesave;
        }

        public ICommand SaveAndClose
        {
            get
            {
                if (saveandclose == null)
                    saveandclose = new DelegateCommand(CanExecuteSave, ExecuteClose);
                return saveandclose;
            }
        }

        private void ExecuteClose(object parameter)
        {         
            ReturnObject = EstDateFirstSales;
            SaveFlag = true;
            CloseWindow();
        }

        ICommand cancelandclose;
        public ICommand CancelConfirmation
        {
            get
            {
                if (cancelandclose == null)
                    cancelandclose = new DelegateCommand(CanExecute, ExecuteCancelAndClose);
                return cancelandclose;
            }
        }

        private void ExecuteCancelAndClose(object parameter)
        {
            ReturnObject = null;
            SaveFlag = false;                                            
            CloseWindow();
        }

        #endregion
    }
}
