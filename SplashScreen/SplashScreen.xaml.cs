using System;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace PTR.SplashScreen
{
    /// <summary>
    /// Interaction logic for SplashScreen.xaml
    /// </summary>
    public partial class SplashScreen : Window, ISplashScreen
    {
        public SplashScreen()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// Keyboard key trapping
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            //prevent alt-F4 from closing dialog
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
        }


        public void AddMessage(string message, int sleeptime)
        {
            Dispatcher.Invoke((Action)delegate ()
            {
                this.msg.Text = message;
                //this.progbar.Tag = message;
            });
            Thread.Sleep(sleeptime);
        }

        public void AddVersion(string message)
        {
            Dispatcher.Invoke(delegate ()
            {
                this.version.Text = message;
            });
        }

        public void LoadComplete()
        {           
            Dispatcher.Invoke(delegate ()
            {
                this.Close();
            });
            Dispatcher.InvokeShutdown();
        }
    }

    public interface ISplashScreen
    {
        void AddMessage(string message, int sleeptime);
        void AddVersion(string message);
        void LoadComplete();
    }
}
