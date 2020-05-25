using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;

namespace PTR
{
    public static class UpdateVersion
    {

        private static string installerexe;
       
        public static void Update(string installerlocation, string executablename)
        {
            try
            {
                
                App.splashScreen.AddMessage("This version has expired.\nDownloading and Updating now.", 3000);
                //download & run new installer
                System.Reflection.Assembly asmly = System.Reflection.Assembly.GetExecutingAssembly();
                installerexe = Path.GetDirectoryName(asmly.Location) + @"\" + executablename;

                if (File.Exists(installerexe))                
                    File.Delete(installerexe);
                
                WebClient wc = new WebClient();                
                wc.DownloadFileCompleted += new AsyncCompletedEventHandler(WebClient_DownloadFileCompleted);
                wc.DownloadFileAsync(new Uri(installerlocation), installerexe);
                wc.Dispose();
                wc = null;
            }
            catch
            {
                try
                {
                    App.splashScreen.AddMessage("Update error.\nUpdate cancelled", 3000);
                    App.splashScreen?.LoadComplete();
                    App.CloseProgram();
                }
                catch
                {
                    Environment.Exit(-1);
                }
            }
        }

        private static void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                if (e.Error == null)                                    
                    Process.Start(installerexe);                                                                                                                 
                else
                    App.splashScreen.AddMessage("Download unsuccessful.\nUpdate cancelled", 3000);
              
                //close current instance
                App.splashScreen?.LoadComplete();
                App.CloseProgram();                               
            }
            catch (FileNotFoundException ex)
            {
                try
                {
                    App.splashScreen.AddMessage("Downloaded file not found.\nUpdate cancelled", 3000);
                    App.splashScreen?.LoadComplete();
                    App.CloseProgram();
                }
                catch
                {
                    Environment.Exit(-1);
                }               
            }
            catch
            {
                Environment.Exit(-1);
            }            
        }

    }
}
