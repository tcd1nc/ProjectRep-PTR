
using System.Windows.Input;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using PTR.Models;

namespace PTR.ViewModels
{
    public class SetupViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;

        FullyObservableCollection<ModelBaseVM> defaultprojstatuses;

        public SetupViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            GetSetUp();
            GetDefaultPBSalesStatuses();
            
            canexecutesave = false;
        }
                

        private void GetSetUp()
        {
            SetUp = DatabaseQueries.GetSetup();
            SetUp.PropertyChanged += SetUp_PropertyChanged;
        }

        private void SetUp_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            CheckFieldValidation();
            isdirty = true;
        }


        #region Properties

        Models.SetupModel setup;
        public Models.SetupModel SetUp
        {
            get {return setup; }
            set { SetField(ref setup, value); }
        }


        bool projectNameLengthError;
        public bool ProjectNameLengthError
        {
            get { return projectNameLengthError; }
            set
            {
                SetField(ref projectNameLengthError, value);
                CheckFieldValidation();
            }
        }
                
        bool defaultTrialStatusIDError;
        public bool DefaultTrialStatusIDError
        {
            get { return defaultTrialStatusIDError; }
            set
            {
                SetField(ref defaultTrialStatusIDError, value);
                CheckFieldValidation();
            }
        }
                       
        bool statusIDforTrialsError;
        public bool StatusIDforTrialsError
        {
            get { return statusIDforTrialsError; }
            set
            {
                SetField(ref statusIDforTrialsError, value);
                CheckFieldValidation();
            }
        }

        bool invalidfield;
        public bool InvalidField
        {
            get { return invalidfield; }
            set { SetField(ref invalidfield, value); }
        }

        string datamissing;
        public string DataErrorLabel
        {
            get { return datamissing; }
            set { SetField(ref datamissing, value); }
        }

        public FullyObservableCollection<ModelBaseVM> DefaultPBSalesStatuses
        {
            get { return defaultprojstatuses; }
            set { SetField(ref defaultprojstatuses, value); }
        }


        #endregion

        #region Private Functions

        private void CheckFieldValidation()
        {            
            bool DomainRequired = string.IsNullOrEmpty(SetUp.Domain);
            bool EmailformatRequired= string.IsNullOrEmpty(SetUp.Emailformat);
                                   
            InvalidField = (DomainRequired || EmailformatRequired);

            if (DomainRequired)
                DataErrorLabel = "Domain Missing";
            else
                if (EmailformatRequired)
                DataErrorLabel = "Email format Missing";           
        }             

        
        private void GetDefaultPBSalesStatuses()
        {
            List<string> defaultprojstatuses = SetUp.DefaultSalesStatuses.Split(',').ToList();
            Collection<ModelBaseVM> salesstatuses = DatabaseQueries.GetSalesStatuses();
            FullyObservableCollection<ModelBaseVM> SalesStatusesColl = new FullyObservableCollection<ModelBaseVM>();            
            foreach (ModelBaseVM sd in salesstatuses)
            {
                SalesStatusesColl.Add(new ModelBaseVM
                {
                    ID = sd.ID,
                    IsChecked = defaultprojstatuses.Contains(sd.ID.ToString()),
                    Name = sd.Name                   
                });
            }
            DefaultPBSalesStatuses = SalesStatusesColl;
            DefaultPBSalesStatuses.ItemPropertyChanged += DefaultPBSalesStatuses_ItemPropertyChanged;
        }

        private void DefaultPBSalesStatuses_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            isdirty = true;
        }


        //save
        private bool CanExecuteSave(object obj)
        {
            if (InvalidField)
                return false;
            if (!isdirty)
                return false;
            return true;
        }

        private void ExecuteSave(object parameter)
        {
            SaveAll();
        }

        private void SaveAll()
        {
            if (isdirty)
            {
                var q = from a in DefaultPBSalesStatuses
                        where a.IsChecked == true
                        select a.ID;
                SetUp.DefaultSalesStatuses = string.Join(",", q.ToList());
                DatabaseQueries.UpdateSetup(SetUp);
                StaticCollections.Config = DatabaseQueries.GetSetup();
                isdirty = false;
            }
        }

        #endregion

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
                if (!InvalidField)
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                    {
                        SaveAll();
                        return true;
                    }
                    else                    
                        return true;                    
                }
                else
                {
                    IMessageBoxService msg = new MessageBoxService();
                    var result = msg.ShowMessage("There are unsaved changes with errors. Do you want to correct and then save these?", "Unsaved Changes with Errors", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
                    msg = null;
                    if (result.Equals(GenericMessageBoxResult.Yes))
                        return false;
                    else                    
                        return true;                    
                }
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


    }
}
