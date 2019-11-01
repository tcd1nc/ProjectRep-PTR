using System.Windows.Input;
using PTR.Models;
using System;
using System.Text;

namespace PTR.ViewModels
{
    public class EPViewModel : ObjectCRUDViewModel
    {
        private const string title = "Evaluation Plan";
        public ICommand CopyEP { get; set; }
        bool isdirty = false;

        public EPViewModel(int id, int projectid)
        {
            ExCloseWindow = ExecuteClosing;
            if (id == 0)
            {
                EP = new EPModel()
                {
                    GOM = new GenericObjModel()
                    {
                        Description = string.Empty
                    },
                    Created = DateTime.Now,
                    Objectives = string.Empty,
                    Strategy = string.Empty,
                    ProjectID = projectid
                };
                IsEnabled = true;
            }
            else
            {
                EP = DatabaseQueries.GetEvaluationPlan(id);
                SetUserAccessExistingEP(ep.CustomerID);
            }
            cancleardate = EP.Discussed != null;
            EP.PropertyChanged += EP_PropertyChanged;
            ep.GOM.PropertyChanged += EP_PropertyChanged;

            Save = new RelayCommand(ExecuteSave, CanExecuteSave);

            if (id == 0)
                WindowTitle = title;
            else
                WindowTitle = title + " (ID: " + id.ToString() + ", Project ID: " + projectid.ToString() + ")";

            CopyEP = new RelayCommand(ExecuteCopyEP, CanExecuteCopyEP);
        }

        #region Event Handlers

        private void EP_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            isdirty = true;
        }

        #endregion

        #region Properties 

        string selecteddate;
        public string SelectedDate
        {
            get { return selecteddate; }
            set
            {
                cancleardate = !string.IsNullOrEmpty(value);
                SetField(ref selecteddate, value);
            }
        }

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
        
        bool? blnsave;
        public bool? SaveFlag
        {
            get { return blnsave; }
            set { SetField(ref blnsave, value); }
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
            sbhtml.Append(ep.GOM.Description);
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
            sbtext.Append(ep.GOM.Description);
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
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveEP();

            ReturnObject = true;
            SaveFlag = true;
            CloseWindow();
        }

        private void SaveEP()
        {
            if (isdirty)
            {
                if (EP.GOM.ID > 0)
                    DatabaseQueries.UpdateEvaluationPlan(EP);
                else
                    DatabaseQueries.AddEvaluationPlan(EP);
                isdirty = false;
            }
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
            ReturnObject = false;
            SaveFlag = false;                                            
            CloseWindow();
        }

        //==============================

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

        private void ExecuteClosing(object parameter)
        {
            if (canexecutesave)
            {
                IMessageBoxService msg = new MessageBoxService();
                GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                msg = null;
                if (result.Equals(GenericMessageBoxResult.Yes))
                {
                    SaveEP();
                }
            }
        }


        #endregion


    }

}
