using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using MailServices.Properties;
using Util.MailServices.Interfaces;

namespace Util.MailServices.Implementations
{
    public class SmtpMailServices : IMailServices
    {
        private SmtpClient client;

        public SmtpMailServices()
        {
            client = new SmtpClient(Settings.Default.SMTPHost, Settings.Default.SMTPPort);
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
        }

        public void SendMail(MailMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            message.Sender = new MailAddress("yavuz.arslan@haw-hamburg.de");
            message.From = new MailAddress("yavuz.arslan@haw-hamburg.de");
            message.To.Add(new MailAddress("torben.koenke@haw-hamburg.de"));
            client.Send(message);
        }

        public void SendMails(IEnumerable<MailMessage> messages)
        {
            if (messages == null)
            {
                throw new ArgumentException("messages");
            }
            foreach (var message in messages)
            {
                SendMail(message);
            }
        }

        public void SetCredentials(NetworkCredential nc)
        {
            client.Credentials = nc;
        }
    }
}