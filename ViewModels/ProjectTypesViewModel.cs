using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using PTR.Models;
using static PTR.DatabaseQueries;

namespace PTR.ViewModels
{
    public class ProjectTypesViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecuteadd = true;
        bool canexecutedelete = false;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }
        public ICommand Save { get; set; }

        bool isdirty = false;
        FullyObservableCollection<ProjectTypeModel> projecttypes = new FullyObservableCollection<ProjectTypeModel>();

        public ProjectTypesViewModel()
        {
            Save = new RelayCommand(ExecuteSave, CanExecuteSave);
            Cancel = new RelayCommand(ExecuteCancel, CanExecute);
            AddNew = new RelayCommand(ExecuteAddNew, CanExecuteAddNew);
            GetProjectTypes();
            canexecutesave = false;
            Colours = GetColours();
        }

        public Collection<string> Colours { get; private set; }

        public FullyObservableCollection<ProjectTypeModel> ProjectTypes
        {
            get { return projecttypes; }
            set { SetField(ref projecttypes, value); }
        }

        bool invalidfield;
        public bool InvalidField
        {
            get { return invalidfield; }
            set { SetField(ref invalidfield, value); }
        }

        string datamissing;
        public string DataMissingLabel
        {
            get { return datamissing; }
            set { SetField(ref datamissing, value); }
        }

        bool isselected = false;
        public bool IsSelected
        {
            get { return isselected; }
            set { SetField(ref isselected, value); }
        }

        private void GetProjectTypes()
        {
            ProjectTypes = DatabaseQueries.GetProjectTypes();           
            ProjectTypes.ItemPropertyChanged += ProjectTypes_ItemPropertyChanged;
        }

        private void ProjectTypes_ItemPropertyChanged(object sender, ItemPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "IsChecked")
            {
                CheckValidation();
                isdirty = true;
            }
            IsSelected = ProjectTypes.Where(x => x.IsChecked).Count() > 0;
        }

        private void CheckValidation()
        {            
            bool TypeRequired = IsTypeMissing();
            bool ColourRequired = IsColourMissing();
            bool DuplicateType = IsDuplicateType();
            bool DescriptionRequired = DescriptionMissing();
           
            InvalidField = (DuplicateType || TypeRequired || ColourRequired || DescriptionRequired );

            if (TypeRequired)
                DataMissingLabel = "Name Missing";
            else
            if (DuplicateType)
                DataMissingLabel = "Duplicate Name";
            else
            if (ColourRequired)
                DataMissingLabel = "Colour Missing";
            else
            if (DescriptionRequired)
                DataMissingLabel = "Description Missing";
           
        }

        private bool IsDuplicateType()
        {
            var query = ProjectTypes.GroupBy(x => x.Name.Trim().ToUpper())
             .Where(g => g.Count() > 1)
             .Select(y => y.Key)
             .ToList();
            return (query.Count > 0);
        }

        private bool IsTypeMissing()
        {
            int nummissing = ProjectTypes.Where(x => string.IsNullOrEmpty(x.Name.Trim())).Count();
            return (nummissing > 0);
        }

        private bool DescriptionMissing()
        {
            int nummissing = ProjectTypes.Where(x => string.IsNullOrEmpty(x.Description.Trim())).Count();
            return (nummissing > 0);
        }

        private bool IsColourMissing()
        {
            int nummissing = ProjectTypes.Where(x => string.IsNullOrEmpty(x.Colour.Trim())).Count();
            return (nummissing > 0);
        }


        //private bool IsInvalidActivityCodes()
        //{
        //    Regex rx;
        //    rx = new Regex("^([0-9]+,)*[0-9]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
            
        //    foreach(ProjectTypeModel ptm in ProjectTypes)
        //    {
        //        Match match = rx.Match(ptm.ActivityCodes);
        //        if (!match.Success)                
        //            return true;               
        //    }            
        //    return false;
        //}

        #region Commands

        private bool CanExecuteAddNew(object obj)
        {
            if (InvalidField)
                return false;
            return canexecuteadd;
        }

        private void ExecuteAddNew(object parameter)
        {
            ProjectTypes.Add(new ProjectTypeModel()
            {
                ID = 0,
                Name = string.Empty,
                Description = string.Empty,
                Colour = new SolidColorBrush(Colors.AliceBlue).ToString(),
                IsEnabled = true,
                IsChecked = false
            });
            CheckValidation();
        }

        private void ExecuteCancel(object parameter)
        {
            CloseWindow();
        }

        ICommand delete;
        public ICommand Delete
        {
            get
            {
                if (delete == null)
                    delete = new DelegateCommand(CanExecuteDelete, ExecuteDelete);
                return delete;
            }
        }

        
        private bool CanExecuteDelete(object obj)
        {
            if (IsSelected)
                return true;
            return canexecutedelete;
        }

        Collection<ProjectTypeModel> deleteditems = new Collection<ProjectTypeModel>();

        private void ExecuteDelete(object parameter)
        {
            IMessageBoxService msg = new MessageBoxService();
            string title = "Deleting Project Type";
            string confirmtxt = "Do you want to delete the selected item";
            if (ProjectTypes.Where(x => x.IsChecked).Count() > 1)
            {
                title = title + "s";
                confirmtxt = confirmtxt + "s";
            }
            if (msg.ShowMessage(confirmtxt + "?", title, GenericMessageBoxButton.OKCancel, GenericMessageBoxIcon.Question).Equals(GenericMessageBoxResult.OK))
            {
                foreach (ProjectTypeModel si in ProjectTypes)
                {
                    if (si.IsChecked )
                    {
                        if(si.ID > 0)
                            DeleteProjectType(si.ID);
                        deleteditems.Add(si);
                    }                    
                }
    
                foreach(ProjectTypeModel pm in deleteditems)
                {
                    ProjectTypes.Remove(pm);
                }
                deleteditems.Clear();
                CheckValidation();
            }
            msg = null;
        }
               

        //save
        private bool CanExecuteSave(object obj)
        {
            if (InvalidField)
                return false;
            if (isdirty)
                return true;
            return canexecutesave;
        }

        private void ExecuteSave(object parameter)
        {
            SaveAll();
        }

        private void SaveAll()
        {
            if (isdirty)
            {
                foreach (ProjectTypeModel am in ProjectTypes)
                {
                    if (am.ID == 0)
                        am.ID = AddProjectType(am);
                    else
                        UpdateProjectType(am);
                }
                isdirty = false;
            }
        }

        private void ExecuteClosing(object parameter)
        {

            //if (isdirty && !InvalidField)
            //{
            //    IMessageBoxService msg = new MessageBoxService();
            //    GenericMessageBoxResult result = msg.ShowMessage("There are unsaved changes. Do you want to save these?", "Unsaved Changes", GenericMessageBoxButton.YesNo, GenericMessageBoxIcon.Question);
            //    msg = null;
            //    if (result.Equals(GenericMessageBoxResult.Yes))
            //        SaveAll();
            //}
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

        #endregion
    }
}
