using System;
using System.Net.Mail;
using static PTR.StaticCollections;

namespace PTR
{
    public static class Mail
    {

        public static bool SendMail(string sender, string toaddresses, string subject, string bodystdcontent, string body)
        {
            bool success = false;
            try
            {

                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(sender),
                    Subject = subject,
                    IsBodyHtml = Config.IsBodyHtml,
                    Body = bodystdcontent + body                 
                };
          
                mail.To.Add(toaddresses);
                
                using (SmtpClient client = new SmtpClient())
                {
                    client.Port = Config.Port;
                    client.Host = Config.SMTP;
                    client.EnableSsl = Config.EnableSSL;
                    client.UseDefaultCredentials = Config.UseDefaultCredentials;
                   // client.TargetName = Config.TargetName;
                    if (Config.UseExtEMCredentials)
                        client.Credentials = new System.Net.NetworkCredential(Config.EMUser, Config.EMPWD);

                    client.DeliveryMethod = SmtpDeliveryMethod.Network;                
                    client.Send(mail);
                }
                success = true;
            }
            catch //(Exception e)
            {
                //System.Diagnostics.Debug.Print(e.Message.ToString());
            }
            return success;            
        }

    }
}
