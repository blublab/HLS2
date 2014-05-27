using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace Util.MailServices.Interfaces
{
    public interface IMailServices
    {
        /// <summary>
        /// Sendet die angegebene Email.
        /// </summary>
        /// <throws>ArgumentNullException, message == null.</throws>
        /// <throws>SmtpException, Fehler während des Sendens.</throws>
        void SendMail(MailMessage message);

        /// <summary>
        /// Sendet jede in der Collection enthaltene Email.
        /// </summary>
        /// <throws>ArgumentNullException, messages == null.</throws>
        /// <throws>SmtpException, Fehler während des Sendens.</throws>
        void SendMails(IEnumerable<MailMessage> messages);

        void SetCredentials(NetworkCredential nc);
    }
}
