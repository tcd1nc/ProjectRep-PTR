using System;
using System.Windows.Input;

namespace PTR.ViewModels
{
    public class ObjectCRUDViewModel : ViewModelBase
    {
        public bool canexecutesave = true;
        public bool canexecutedelete = true;
        public bool canexecuteadd = true;
        public ICommand AddNew { get; set; }
        public ICommand Cancel { get; set; }       
        public ICommand Delete { get; set; }
        public ICommand Save { get; set; }

        public Action<object> ExCloseWindow { get; set; }

        #region Properties

        bool duplicatename;
        public bool DuplicateName
        {
            get { return duplicatename; }
            set { SetField(ref duplicatename, value); }
        }

        int scrolltoselecteditem;
        public int ScrollToSelectedItem
        {
            get { return scrolltoselecteditem; }
            set { SetField(ref scrolltoselecteditem, value); }
        }

        bool duplicatedate;
        public bool DuplicateDate
        {
            get { return duplicatedate; }
            set { SetField(ref duplicatedate, value); }
        }



        ICommand windowclosing;
        bool canclosewindow = true;
        private bool CanCloseWindow(object obj)
        {
            return canclosewindow;
        }

        public ICommand WindowClosing
        {
            get
            {
                if (windowclosing == null)
                    windowclosing = new DelegateCommand(CanCloseWindow, ExCloseWindow);
                return windowclosing;
            }
        }


        #endregion
    }
}
