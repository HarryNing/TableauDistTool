using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading;

namespace TableauDistTool.Util
{
    public static class EmailHelper
    {
        public static void SendMail(string to, string subject, string reportLink, string fileName)
        {
            var message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress("dw.e1support@ef.com");
            message.To.Add(to);
            message.Bcc.Add("admin.e1@ef.com");
            message.Subject = subject;
            string body = string.Format(@"You can check the online version here: <a href={0}>{0}</a> <br><p>", reportLink);
            message.Body = body + @"This e-mail and any attachments may contain confidential and privileged information. If you are not the intended recipient, please notify the sender immediately by return e-mail, delete this e-mail and destroy any copies. Any dissemination or use of this information by a person other than the intended recipient is unauthorized and may be illegal.";
            message.Attachments.Add(new Attachment(fileName));
            new RetryableSmtpClient(3).Send(message);
        }
    }

    public class RetryableSmtpClient
    {
        private readonly int _retryAttempts;

        public RetryableSmtpClient(int retryAttempts)
        {
            _retryAttempts = retryAttempts;
        }

        public void Send(MailMessage message)
        {
            for (var i = 0; i < _retryAttempts; i++)
            {
                try
                {
                    DoSend(message);
                    i = _retryAttempts;
                }
                catch (SmtpException)
                {
                    i++;
                    if (i == _retryAttempts) throw;

                    Thread.Sleep(1000);
                }
            }
        }

        private void DoSend(MailMessage message)
        {
            new SmtpClient().Send(message);
        }
    }
}