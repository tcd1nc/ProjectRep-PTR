using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class CorporationViewModel :ViewModelBase
    {
        FullyObservableCollection<Models.CorporationModel> _corporations;
        public CorporationViewModel()
        {
            _corporations = DatabaseQueries.GetCorporations();


        }


        string _corporationname;
        public string CustomerName
        {
            get { return _corporationname; }
            set
            {
                if (IsValidCorporationName(value))
                    SetField(ref _corporationname, value);
                else
                {
                    IMessageBoxService _msg = new MessageBoxService();
                    _msg.ShowMessage("Corporation names must be unique", "Corporation Name Already Exists", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Asterisk);
                }
            }
        }


        #region Commands

       
        ICommand _saveandclose;
        public ICommand SaveAndClose
        {
            get
            {
                if (_saveandclose == null)
                    _saveandclose = new DelegateCommand(CanExecute, ExecuteSaveAndClose);
                return _saveandclose;
            }
        }

        private void ExecuteSaveAndClose(object parameter)
        {
            IMessageBoxService _msgboxcommand = new MessageBoxService();

            if (!IsValidCorporationName(_corporationname))
                _msgboxcommand.ShowMessage("Corporation Name is invalid", "Corporation Name Error", GenericMessageBoxButton.OK, GenericMessageBoxIcon.Error);
            else
            {
                Models.CorporationModel _newcorp = new Models.CorporationModel();
                _newcorp.GOM.Name = _corporationname;
              
                DatabaseQueries.AddNewCorporation(_newcorp);

                CloseWindow();
            }
        }

        private bool IsValidCorporationName(string _name)
        {
            if (string.IsNullOrEmpty(_name))
                return false;

            foreach (Models.CorporationModel am in _corporations)
            {
                if (am.GOM.Name == _name)
                    return false;
            }
            return true;
        }

    }

    #endregion
}
