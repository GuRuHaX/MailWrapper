using System;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

/*
=========---- GuRuHaX Productions ----========== 

    Some side notes :

    You should NEVER EVER hardcode your Email credentials inside ANY application.
    It is recommended to at least Encrypt them , or do it serverside with appropiate security measures.

    I have made this to make it easyer for beginners to use the email functions , 
    feel free to add/remove anything to it.


    If you are using GMAIL as SMTP make sure to have correct settings in 
    your account for it to work properly.

    Follow these steps :

    1) Allow less secure apps
    https://support.google.com/a/answer/6260879?hl=en

    2) Allow access to your account
    https://accounts.google.com/ServiceLogin?passive=1209600&continue=https%3A%2F%2Faccounts.google.com%2FDisplayUnlockCaptcha&followup=https%3A%2F%2Faccounts.google.com%2FDisplayUnlockCaptcha#identifier


===========================================================================================================================================================================================================
*/

namespace GuruMail
{
    public class Mailer
    {
       /// <summary>
        /// Boolean property that indicates the succes of sending the email.
        /// </summary>
        public static bool Succes { get; private set; }

        /// <summary>
        /// Function to send an Email.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="content">The email content.</param>
        /// <param name="attachmentType">Extension type for the attachment.</param>
        /// <param name="attachmentPath">The path for the attachment.</param>
        /// <param name="sender">The sender's Email address.</param>
        /// <param name="password">The sender's Email password.</param>
        /// <param name="receiver">The receivers's Email address.</param>
        public static async Task MailAsync(string subject, string content, string attachmentType, string attachmentPath, string sender, string password, string receiver)
        {
            await Task.Run((() => Mail(subject, content, attachmentType, attachmentPath, sender, password, receiver)));
        }

        /// <summary>
        /// Function to send an Email.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="content">The email content.</param>
        /// <param name="attachmentType">Extension type for the attachment.</param>
        /// <param name="attachmentPath">The path for the attachment.</param>
        /// <param name="sender">The sender's Email address.</param>
        /// <param name="password">The sender's Email password.</param>
        /// <param name="receiver">The receivers's Email address.</param>
        public static void Mail(string subject, string content, string attachmentType, string attachmentPath, string sender, string password, string receiver)
        {
            attachmentType = checkAttachmentExtension(attachmentType);
            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(sender, password);
                try
                {
                    using (MailMessage message = new MailMessage(new MailAddress(sender), new MailAddress(receiver)))
                    {
                        message.IsBodyHtml = true;
                        //message.SubjectEncoding = Encoding.UTF8;  Not necessary
                        message.Subject = subject;

                        //message.BodyEncoding = Encoding.UTF8;  Not necessary
                        message.Body = Environment.NewLine + content;

                        if (isValidPath(attachmentPath) && !string.IsNullOrEmpty(attachmentType))
                        {
                            Attachment attachment = new Attachment(attachmentPath, attachmentType);
                            message.Attachments.Add(attachment);
                        }

                        smtp.Send(message);
                        Succes = true;
                    }
                }
                catch
                {
                    Succes = false;
                }
            }
        }
        
        /// <summary>
        /// Checks if the attachement is in a correct format.
        /// </summary>
        /// <param name="extension">The attachement value that has to be checked</param>
        /// <returns>A string in a correct format or null if parameter is empty or null</returns>
        private static string checkAttachmentExtension(string extension)
        {
            if (!string.IsNullOrEmpty(extension))
            {
                extension = (extension.Contains(".")) ? extension : "." + extension;
                extension = (extension[0] == '.') ? extension : "." + extension;
            }
            else { return null; }
            return extension;
        }
        
        /// <summary>
        /// Checks if the path of given file exists or not.
        /// </summary>
        /// <param name="path">The path that has to be checked</param>
        /// <returns>True of false</returns>
        public static bool isValidPath(string path)
        {
            FileInfo fileInfo = null;
            try
            {
                fileInfo = new FileInfo(path);
            }
            catch (ArgumentException) { }
            catch (PathTooLongException) { }
            catch (NotSupportedException) { }

            if (ReferenceEquals(fileInfo, null))
            {
                // file name is not valid
                return false;
            }
            else
            {
                // file name is valid...
                // check for existence 
                if (fileInfo.Exists) { return true; }
                return false;
            }
        }
    }
       
    }
}
