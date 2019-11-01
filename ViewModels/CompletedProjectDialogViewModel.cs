using System;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class CompletedProjectDialogViewModel : ViewModelBase
    {
        public CompletedProjectDialogViewModel(object[] values)
        {                         
            ActualSalesForecast = (decimal)values[0];
            EstDateFirstSales = (DateTime)values[1];
            CultureCode = (string)values[2];

            this.PropertyChanged += CompletedProjectDialogViewModel_PropertyChanged;
        }

        #region Event Handlers

        private void CompletedProjectDialogViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == "ActualSalesForecast")                            
                cansave = (ActualSalesForecast > 0);            
        }

        #endregion

        #region Properties

        object[] returnobject = new object[2];
        public object[] ReturnObject
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

        decimal actualsalesforecast;
        public decimal ActualSalesForecast
        {
            get { return actualsalesforecast; }
            set { SetField(ref actualsalesforecast, value); }
        }

        DateTime estdatefirstsales;
        public DateTime EstDateFirstSales
        {
            get { return estdatefirstsales; }
            set { SetField(ref estdatefirstsales, value); }
        }

        string culturecode;
        public string CultureCode
        {
            get { return culturecode; }
            set { SetField(ref culturecode, value); }
        }

        #endregion

        #region Commands

        bool cansave = true;
        public bool CanExecuteSave(object parameter)
        {
            return cansave;
        }

        ICommand saveandclose;
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
            this.PropertyChanged -= CompletedProjectDialogViewModel_PropertyChanged;

            ReturnObject[0] = ActualSalesForecast;
            ReturnObject[1] = EstDateFirstSales;

            SaveFlag = true;
            CloseWindow();
        }

        ICommand cancelandclose;
        public ICommand CancelProjectCompletion
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
            this.PropertyChanged -= CompletedProjectDialogViewModel_PropertyChanged;

            ReturnObject = null; 
            SaveFlag = false;                      
            CloseWindow();
        }

        #endregion
    }
}
