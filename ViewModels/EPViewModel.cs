using System.Windows.Input;
using PTR.Models;
using System;
using System.Text;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class EPViewModel : ViewModelBase
    {
        private const string title = "Evaluation Plan";
        public ICommand CopyEP { get; set; }
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;

        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        
        public EPViewModel(int id, int projectid)
        {
            if (id == 0)
            {
                EP = new EPModel()
                {                    
                    Description = string.Empty,
                    Created = DateTime.Now,
                    Objectives = string.Empty,
                    Strategy = string.Empty,
                    ProjectID = projectid
                };
                IsEnabled = true;
            }
            else
            {
                EP = GetEvaluationPlan(id);
                SetUserAccessExistingEP(ep.CustomerID);
            }
            cancleardate = EP.Discussed != null;
            EP.PropertyChanged += EP_PropertyChanged;
                     
            if (id == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (ID: " + id.ToString() + ", Project ID: " + projectid.ToString() + ")";


            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            CopyEP = new RelayCommand(ExecuteCopyEP, CanExecuteCopyEP);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
        }

        #region Event Handlers

        private void EP_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            isdirty = true;
            if (e.PropertyName == "Discussed")
                cancleardate = true;
        }

        #endregion

        #region Properties 
                
        EPModel ep;
        public EPModel EP
        {
            get { return ep; }
            set { SetField(ref ep, value); }
        }

        string windowtitle;
        public string WindowTitle
        {
            get { return windowtitle; }
            set { SetField(ref windowtitle, value); }
        }

        bool isenabled;
        public bool IsEnabled
        {
            get { return isenabled; }
            set { SetField(ref isenabled, value); }
        }

        bool returncode = false;
        public bool ReturnObject
        {
            get { return returncode; }
            set { SetField(ref returncode, value); }
        }
        
        #endregion

        #region Private functions
        
        private void SetUserAccessExistingEP(int customerid)
        {
            int accessid = StaticCollections.GetUserCustomerAccess(customerid);
            if (accessid == (int)UserPermissionsType.FullAccess)
                IsEnabled = true;
            else
            if (accessid == (int)UserPermissionsType.ReadOnly)
                IsEnabled = false;
            else
                IsEnabled = false;

            canexecutesave = IsEnabled;
        }

        private void SetClipboard(EPModel ep)
        {
            StringBuilder sbhtml = new StringBuilder();
            sbhtml.Append("<p style='font-size:18px;font-family:Arial'><b>");
            sbhtml.Append("Proposal:</b></p>");
            sbhtml.Append("<p style='font-size:14px;font-family:Arial'>");
            sbhtml.Append(ep.Description);
            sbhtml.Append("</p><br/>");

            sbhtml.Append("<p style='font-size:18px;font-family:Arial'><b>");
            sbhtml.Append("Objectives:</b></p>");
            sbhtml.Append("<p style='font-size:14px;font-family:Arial'>");
            sbhtml.Append(ep.Objectives);
            sbhtml.Append("</p><br/>");

            sbhtml.Append("<p style='font-size:18px;font-family:Arial'><b>");
            sbhtml.Append("Strategy:</b></p>");
            sbhtml.Append("<p style='font-size:14px;font-family:Arial'>");
            sbhtml.Append(ep.Strategy);
            sbhtml.Append("</p>");

            StringBuilder sbtext = new StringBuilder();
            sbtext.Append("Proposal:\n");            
            sbtext.Append(ep.Description);
            sbtext.Append("\n\n");
            sbtext.Append("Objectives:\n");            
            sbtext.Append(ep.Objectives);
            sbtext.Append("\n\n");
            sbtext.Append("Strategy:\n");            
            sbtext.Append(ep.Strategy);

            ClipboardHelper.CopyToClipboard(sbhtml.ToString(),sbtext.ToString());

        }
        #endregion

        #region Commands
               
        private bool CanExecuteSave(object obj)
        {
            if (!isdirty)
                return false;

            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveEP();
            ReturnObject = true;
            CloseWindow();
        }

        private void SaveEP()
        {
            if (isdirty)
            {
                if (EP.ID > 0)
                    UpdateEvaluationPlan(EP);
                else
                    EP.ID = AddEvaluationPlan(EP);
                isdirty = false;
            }
        }
                      
        ICommand cleardate;
        bool cancleardate = true;
        private bool CanClearDate(object obj)
        {
            return cancleardate;
        }
         
        public ICommand ClearDate
        {
            get
            {
                if (cleardate == null)
                    cleardate = new DelegateCommand(CanClearDate, ExClearDate);
                return cleardate;
            }
        }

        private void ExClearDate(object parameter)
        {           
            EP.Discussed = null;
            cancleardate = false;                                 
        }


        bool cancopyep = true;
        public bool CanExecuteCopyEP(object parameter)
        {
            return cancopyep;
        }

        private void ExecuteCopyEP(object parameter)
        {
            SetClipboard(EP);
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        private void ExecuteClosing(object parameter)
        {
        }

        ICommand windowclosing;

        private bool CanCloseWindow(object obj)
        {
            if (isdirty)
            {                
                IMessageBoxService msg = new MessageBoxService();
                var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveEP();
                    ReturnObject = true;
                    return true;
                }
                else                
                    return true;                                              
            }
            else
                return true;
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExecuteClosing);
                return windowclosing;
            }
        }

        #endregion


    }

}
