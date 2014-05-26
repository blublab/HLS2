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

        public SmtpMailServices(NetworkCredential credentials)
        {
            client = new SmtpClient(Settings.Default.SMTPHost, Settings.Default.SMTPPort);
            client.Credentials = credentials;
        }

        public void SendMail(MailMessage message)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
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
    }
}