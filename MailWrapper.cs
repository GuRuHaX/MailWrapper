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
        /// <param name="addAttachment">Value for the presence of a attachment.</param>
        /// <param name="attachmentPath">The path for the attachment.</param>
        /// <param name="sender">The sender's Email address.</param>
        /// <param name="password">The sender's Email password.</param>
        /// <param name="receiver">The receivers's Email address.</param>
        public static async Task MailAsync(string subject, string content, string attachmentPath, string sender, string password, string receiver)
        {
            // this call will await the Mail function but asynchronously 
            // so that your application can continue working while its being send.
            await Task.Run((() => Mail(subject, content, attachmentPath, sender, password, receiver)));
        }

        /// <summary>
        /// Function to send an Email.
        /// </summary>
        /// <param name="subject">The email subject.</param>
        /// <param name="content">The email content.</param>
        /// <param name="attachmentPath">The path for the attachment.</param>
        /// <param name="sender">The sender's Email address.</param>
        /// <param name="password">The sender's Email password.</param>
        /// <param name="receiver">The receivers's Email address.</param>
        public static void Mail(string subject, string content, string attachmentPath, string sender, string password, string receiver)
        {
            // If using zipfiles as attachement uncomment this =>
            // ContentType ct = new ContentType(".zip"); 

            using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
            {
                smtp.EnableSsl = true; // use ssl
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network; // use network , IIS || external app/service
                smtp.UseDefaultCredentials = false; // We are providing the credentials 
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

                        if (attachmentPath != string.Empty)
                        {
                            Attachment attachment = new Attachment(attachmentPath);
                            message.Attachments.Add(attachment); // Add attachment to mail 
                        }

                        smtp.Send(message); // you can guess that i hope :)
                    }
                    Succes = true; // the mail has been sended succesfully
                }
                catch (SmtpException smtpException)
                {
                    if (smtpException.GetType().IsAssignableFrom(typeof(SmtpStatusCode)))
                    {
                        throw smtpException;
                    }                                     
                    }
                    Succes = false;
                }
            catch(Exception exception)
            {        
                // Another exception has been throwed , 
                // the app could have been accessed externally 
                // something i did not mentioned maybe ... 
                   throw new Exception(exception.InnerException);
            }  
        }
    }
}


        /* https://msdn.microsoft.com/en-us/library/ms173163.aspx
         
           The following list identifies practices to avoid when throwing exceptions:

           Exceptions should not be used to change the flow of a program as part of ordinary execution. Exceptions should only be used to report and handle error conditions.
           Exceptions should not be returned as a return value or parameter instead of being thrown.
           Do not throw System.Exception, System.SystemException, System.NullReferenceException, or System.IndexOutOfRangeException intentionally from your own source code.
           Do not create exceptions that can be thrown in debug mode but not release mode. To identify run-time errors during the development phase, use Debug Assert instead.
        */

    }
}
