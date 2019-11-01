using System;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Windows;
using System.Windows.Markup;
using static PTR.StaticCollections;
using System.Threading;
using System.Reflection;

namespace PTR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDisposable
    {
        public static SplashScreen.ISplashScreen splashScreen;
        public static ManualResetEvent ResetSplashCreated;
        public static Thread SplashThread;

        void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An Application error has occurred " + e.Exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            e.Handled = true;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            
            CloseProgram();
        }

        public static void KillProcessChildren()
        {
            Process CurrentProcess = Process.GetCurrentProcess();
            int pid = CurrentProcess.Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select * From Win32_Process Where ParentProcessID=" + pid);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
                try
                {
                    Process proc = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                    proc.Kill();
                }
                catch (ArgumentException)
                {
                    // Process already exited.
                }
        }

        private static void SetupSplash()
        {
            SplashThread = new Thread(ShowSplash);
            SplashThread.SetApartmentState(ApartmentState.STA);
            SplashThread.IsBackground = true;
            SplashThread.Name = "Splash Screen";
            SplashThread.Start();
            
        }

        protected override void OnStartup(StartupEventArgs e)
        {

//#if !DEBUG
            Current.Dispatcher.UnhandledException += Dispatcher_SystemException;
//#endif

            ResetSplashCreated = new ManualResetEvent(false);
            SetupSplash();

            ResetSplashCreated.WaitOne();
            base.OnStartup(e);

            splashScreen.AddVersion(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion);

            //Set application startup culture based on config settings
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement), new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));
            //NetworkStatus.AvailabilityChanged += new NetworkStatusChangedHandler(DoAvailabilityChanged);
                       
            if (IsNetworkAvailable() && LoadConfigFileData())
            {                
                InitializeApp();

                this.ShutdownMode = ShutdownMode.OnMainWindowClose;
                this.StartupUri = new Uri("Views/PTMainView.xaml", UriKind.Relative);                                                                      
            }
            else
            {
                this.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                CloseProgram();

                //Shutdown();
                return;
            }
        }

        private static void ShowSplash()
        {
            SplashScreen.SplashScreen splashscreen = new SplashScreen.SplashScreen();
            splashScreen = splashscreen;
            splashscreen.Show();
            ResetSplashCreated.Set();
            System.Windows.Threading.Dispatcher.Run();
        }
               
        public static void Dispatcher_SystemException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if(e.Exception.Message != "Unknown User" 
                && e.Exception.Message != "Expired Version" 
                && e.Exception.Message != "Load Settings Error"
                && e.Exception.Message != "SQL Error")

                MessageBox.Show("A program error has occurred \n" + e.Exception.Message, "Error Details", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            CloseProgram();
        }
        
        public static void DoAvailabilityChanged(object sender, NetworkStatusChangedArgs e)
        {
            if (!NetworkStatus.IsAvailable)
            {
                MessageBox.Show("The Internet connection has been lost. Project Tracker will now close.\nUnsaved changes will be lost", "Unable to run - no Internet connection", MessageBoxButton.OK, MessageBoxImage.Error);
                //CloseProgram();
                Environment.Exit(0);
            }
        }

        private bool IsNetworkAvailable()
        {
            bool networkavailable = true;
            if (!NetworkStatus.IsAvailable)
            {
                splashScreen.AddMessage("Internet is not connected!",2000);
                splashScreen?.LoadComplete();
                networkavailable = false;
            }
            return networkavailable;
        }

        public bool LoadConfigFileData()
        {
            //Get network file location of db connection string from config.json file
            //Get connection string from that file
            //Decrypt the connection string
            bool isok = false;
            const string errormsg = "Database configuration file not found";
            try
            {
                string sqlconnfile = ConfigMgr.ReadJsonValue(ConfigMgr.GetLocalJSONPath() + @"\Config.JSON", "SQL Conn");
                if(!string.IsNullOrEmpty(sqlconnfile))
                {
                    string sqlconnstr = Crypto.DecryptString(ConfigMgr.ReadJsonValue(sqlconnfile, "ConnectionString"), "tcdj1");                
                    Current.Resources["DatabaseConnectionString"] = sqlconnstr;
                    isok = true;
                }                                          
            }
            catch {}
            if (!isok)
            {
                splashScreen.AddMessage(errormsg, 2000);
                splashScreen?.LoadComplete();       
            }
            return isok;        
        }

        private static void CloseSplash()
        {
            try
            {               
                if (splashScreen != null)
                {
                    //splashScreen.LoadComplete();
                    splashScreen = null;
                }               
                SplashThread = null;
            }
            catch{ }
        }

        public static void CloseProgram()
        {
            try
            {
                if (Conn != null)                
                    if (Conn?.State == ConnectionState.Open)
                        Conn?.Close();                
            }
            catch {}
            //close splashscreen and splashscreen thread           
            CloseSplash();
                      
            //remove event handlers
            try
            {
                Current.Dispatcher.UnhandledException -= Dispatcher_SystemException;
                NetworkStatus.AvailabilityChanged -= new NetworkStatusChangedHandler(DoAvailabilityChanged);
            }
            catch { }
            finally {
                Environment.Exit(0);
            }
        }
        


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (ResetSplashCreated != null)
                    {
                        ResetSplashCreated.Dispose();
                        ResetSplashCreated = null;
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~App() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}

