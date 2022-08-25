using System;
using System.Net;
using System.Net.Mail;

namespace AbleCheckbook.Logic
{
    public static class Emailer
    {
        /// <summary>
        /// [static] Send a support e-mail
        /// </summary>
        /// <param name="subject">topic</param>
        /// <param name="body">the message itself</param>
        /// <param name="pathToAttachment">Attachment file path, null if none</param>
        public static bool SendEmail(string subject, string body, string pathToAttachment)
        {
            bool success = false; 
            string server = Configuration.Instance.SmtpServer;
            string address = Configuration.Instance.SupportEmail;
            MailMessage email = new MailMessage(address, address, subject, body);
            email.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            email.IsBodyHtml = false;
            if (pathToAttachment != null)
            {
                email.Attachments.Add(new Attachment(pathToAttachment));
            }
            SmtpClient client = new SmtpClient(server);
            // client.Port = 587; // TLS, 465 = SSL
            client.Credentials = CredentialCache.DefaultNetworkCredentials;
            try
            {
                client.Send(email);
                success = true;
            }
            catch (Exception ex)
            {
                Logger.Warn("Cannot send email to " + address + ", subject: " + subject, ex);
            }
            return success;
        }

    }

}
