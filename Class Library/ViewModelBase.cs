using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows;
using System;

namespace PTR
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        ICommand closewindowcommand;
        ICommand shutdowncommand;                          

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string caller = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(caller));
        }

        protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        //public static Collection<int> dirtyprojects  = new Collection<int>(); 
        //public static void AddChangedProject(int id)
        //{       
        //    if(!dirtyprojects.Contains(id))    
        //       dirtyprojects.Add(id);                          
        //}

        //public static void ClearDirtyProjectList()
        //{
        //    dirtyprojects?.Clear();
        //}
        
        public ICommand CloseWindowCommand
        {
            get
            {
                if (closewindowcommand == null)                
                    closewindowcommand = new DelegateCommand(CanExecute, ExecuteCloseWindow);
                
                return closewindowcommand;
            }
        }

        public void ExecuteCloseWindow(object parameter)
        {
            var wnd = parameter as Window;
            //if (wnd != null)
                wnd?.Close();
        }
               
        public ICommand Shutdown
        {
            get
            {
                if (shutdowncommand == null)                
                    shutdowncommand = new DelegateCommand(CanExecute, ExecuteShutdown);
                
                return shutdowncommand;
            }
        }

        private void ExecuteShutdown(object parameter)
        {
            Application.Current.Shutdown();
        }


        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            throw new NotImplementedException();
        }


        bool? closewindowflag;
        public bool? CloseWindowFlag
        {
            get { return closewindowflag; }
            set { SetField(ref closewindowflag, value); }
        }

        public virtual void CloseWindow(bool? result = true)
        {
            Application.Current.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Background, new Action(() =>
            {
                CloseWindowFlag = CloseWindowFlag == null ? true : !CloseWindowFlag;
            }));
        }

        ICommand close;
        public ICommand Close
        {
            get
            {
                if (close == null)
                    close = new DelegateCommand(CanExecute, ExecuteClose);
                return close;
            }
        }

        private void ExecuteClose(object parameter)
        {
            CloseWindow();
        }

    }

    public class DelegateCommand : ICommand
    {
        #region Commands


        Predicate<object> canExecute;
        Action<object> execute;

        public DelegateCommand(Predicate<object> canexecuteobj, Action<object> executeobj) : this()
        {
            canExecute = canexecuteobj;
            execute = executeobj;
        }

        public DelegateCommand()
        {
        }

        public bool CanExecute(object parameter)
        {
            return canExecute == null ? true : canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }


        public void Execute(object parameter)
        {
            execute(parameter);
        }

        #endregion

    }

}
