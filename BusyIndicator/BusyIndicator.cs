using System.Threading;
using System.Windows;

namespace PTR
{

    public static class BusyIndicator
    {
        static Busy b;       

        public static void ShowBusy()
        {
            Thread backgroundUi = new Thread(() =>           
            {
                b = new Busy();
                b.Show();
                System.Windows.Threading.Dispatcher.Run();
            });
            backgroundUi.SetApartmentState(ApartmentState.STA);
            backgroundUi.Start();
        }

        public static void KillBusy()
        {
           
        }

        public static void CloseBusy()
        {
            try
            {                
                    //this delay is a bad hack
                    Thread.Sleep(1000);
             //     b.Dispatcher.InvokeShutdown();

                    //new
                   b.Dispatcher.BeginInvokeShutdown(System.Windows.Threading.DispatcherPriority.Normal);                
            }
            catch {
                //new
                App.KillProcessChildren();
                Application.Current.Shutdown(1);

            }
        }
        //put in long task if display of numbers or text is required
        // Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,  new ThreadStart(DoEvents_DoNothing));
        private static void DoEvents_DoNothing()
        {
            //Just a wrapper for the DoEvents method ...             
        }
               
        public static void ShowBusy(string content)
        {
            Thread backgroundUi = new Thread(() =>           
            {
                b = new Busy(content);
                b.Show();               
                System.Windows.Threading.Dispatcher.Run();
               
            });
            backgroundUi.SetApartmentState(ApartmentState.STA);
            backgroundUi.Start();

        }
    }

}
