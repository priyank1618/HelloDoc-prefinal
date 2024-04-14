

using System.Net.Mail;

namespace BAL.Interface
{
    public interface IEmailService
    {
        public void SendEmail(string email, string subject, string message, List<Attachment> attachments = null);
        public void EmailLog(string template, string subject, string email, int requestid = 0, int adminId = 0, int physicianId = 0, string confirmationnum = null, string filePath = null);
    }
}
