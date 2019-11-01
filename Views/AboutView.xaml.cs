using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows;

namespace PTR.Views
{
    /// <summary>
    /// Interaction logic for AboutView.xaml
    /// </summary>
    public partial class AboutView : Window
    {
        public AboutView()
        {
            InitializeComponent();
            //Use File Version in Application | Assembly Information            
            version.Text = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            YearBuilt.Text = DateTime.Now.Year.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
