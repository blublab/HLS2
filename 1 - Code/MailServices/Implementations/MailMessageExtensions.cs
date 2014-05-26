using System.IO;
using System.Net.Mail;

namespace Util.MailServices.Implementations
{
    internal static class MailMessageExtensions
    {
        public static void AddAttachment(this MailMessage m, string path, string name = null)
        {
            if (name == null)
            {
                name = Path.GetFileName(path);
            }
            m.Attachments.Add(new Attachment(File.OpenRead(path), name));
        }
    }
}
